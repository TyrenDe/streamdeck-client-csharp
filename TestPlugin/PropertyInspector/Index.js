var websocket = null,
    uuid = null,
    actionInfo = {},
    inInfo = {},
    runningApps = [],
    isQT = navigator.appVersion.includes('QtWebEngine'),
    onchangeevt = 'onchange';

function connectSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
    uuid = inUUID;
    actionInfo = JSON.parse(inActionInfo); // cache the info
    inInfo = JSON.parse(inInfo);
    websocket = new WebSocket('ws://localhost:' + inPort);

    addDynamicStyles(inInfo.colors);
    initPropertyInspector();

    websocket.onopen = function () {
        var json = {
            event: inRegisterEvent,
            uuid: inUUID
        };

        websocket.send(JSON.stringify(json));

        // Notify the plugin that we are connected
        sendValueToPlugin('propertyInspectorConnected', 'property_inspector');
    };

    websocket.onmessage = function (evt) {
        // Received message from Stream Deck
        var jsonObj = JSON.parse(evt.data);
        var event = jsonObj['event'];

        // TODO: Process and handle events
        console.log(evt.data);
    };
}

function initPropertyInspector() {
    prepareDOMElements(document);
}

function revealSdpiWrapper() {
    const el = document.querySelector('.sdpi-wrapper');
    el && el.classList.remove('hidden');
}

// our method to pass values to the plugin
function sendValueToPlugin(value, param) {
    if (websocket && (websocket.readyState === 1)) {
        const json = {
            'action': actionInfo['action'],
            'event': 'sendToPlugin',
            'context': uuid,
            'payload': {
                [param]: value
            }
        };
        websocket.send(JSON.stringify(json));
    }
}

if (!isQT) {
    document.addEventListener('DOMContentLoaded', function () {
        initPropertyInspector();
    });
}

window.addEventListener('beforeunload', function (e) {
    e.preventDefault();

    // Notify the plugin we are about to leave
    sendValueToPlugin('propertyInspectorWillDisappear', 'property_inspector');

    // Don't set a returnValue to the event, otherwise Chromium with throw an error.
});

/** the pagehide event is fired, when the view disappears */
/*
window.addEventListener('pagehide', function (event) {
    console.log('%c%s','background: green; font-size: 22px; font-weight: bold;','window --->> pagehide.');
    sendValueToPlugin('propertyInspectorPagehide', 'property_inspector');

});
*/

/** the unload event is fired, when the PI will finally disappear */
/*
window.addEventListener('unload', function (event) {
    console.log('%c%s','background: orange; font-size: 22px; font-weight: bold;','window --->> onunload.');
    sendValueToPlugin('propertyInspectorDisconnected', 'property_inspector');
});
*/

/** CREATE INTERACTIVE HTML-DOM
 * where elements can be clicked or act on their 'change' event.
 * Messages are then processed using the 'handleSdpiItemClick' method below.
 */

function prepareDOMElements(baseElement) {
    baseElement = baseElement || document;
    Array.from(baseElement.querySelectorAll('.sdpi-item-value')).forEach((el, i) => {
        const elementsToClick = ['BUTTON', 'OL', 'UL', 'TABLE', 'METER', 'PROGRESS', 'CANVAS'].includes(el.tagName);
        const evt = elementsToClick ? 'onclick' : onchangeevt || 'onchange';
        // console.log(el.type, el.tagName, elementsToClick, el, evt);

        /** Look for <input><span> combinations, where we consider the span as label for the input
         * we don't use `labels` for that, because a range could have 2 labels.
        */
        const inputGroup = el.querySelectorAll('input, span');
        if (inputGroup.length === 2) {
            const offs = inputGroup[0].tagName === 'INPUT' ? 1 : 0;
            inputGroup[offs].innerText = inputGroup[1 - offs].value;
            inputGroup[1 - offs]['oninput'] = function () {
                inputGroup[offs].innerText = inputGroup[1 - offs].value;
            };
        }
        /** We look for elements which have an 'clickable' attribute
         * we use these e.g. on an 'inputGroup' (<span><input type="range"><span>) to adjust the value of
         * the corresponding range-control
          */
        Array.from(el.querySelectorAll('.clickable')).forEach((subel, subi) => {
            subel['onclick'] = function (e) {
                handleSdpiItemClick(e.target, subi);
            };
        });
        el[evt] = function (e) {
            handleSdpiItemClick(e.target, i);
        };
    });

    baseElement.querySelectorAll('textarea').forEach(e => {
        const maxl = e.getAttribute('maxlength');
        e.targets = baseElement.querySelectorAll(`[for='${e.id}']`);
        if (e.targets.length) {
            let fn = () => {
                for (let x of e.targets) {
                    x.innerText = maxl ? `${e.value.length}/${maxl}` : `${e.value.length}`;
                }
            };
            fn();
            e.onkeyup = fn;
        }
    });
}

function handleSdpiItemClick(e, idx) {
    /** Following items are containers, so we won't handle clicks on them */
    if (['OL', 'UL', 'TABLE'].includes(e.tagName)) { return; }
    // console.log('--- handleSdpiItemClick ---', e, `type: ${e.type}`, e.tagName, `inner: ${e.innerText}`);

    /** SPANS are used inside a control as 'labels'
     * If a SPAN element calls this function, it has a class of 'clickable' set and is thereby handled as
     * clickable label.
     */

    if (e.tagName === 'SPAN') {
        const inp = e.parentNode.querySelector('input');
        if (e.getAttribute('value')) {
            return inp && (inp.value = e.getAttribute('value'));
        }
    }

    const selectedElements = [];
    const isList = ['LI', 'OL', 'UL', 'DL', 'TD'].includes(e.tagName);
    const sdpiItem = e.closest('.sdpi-item');
    const sdpiItemGroup = e.closest('.sdpi-item-group');
    let sdpiItemChildren = isList ? sdpiItem.querySelectorAll((e.tagName === 'LI' ? 'li' : 'td')) : sdpiItem.querySelectorAll('.sdpi-item-child > input');

    if (isList) {
        const siv = e.closest('.sdpi-item-value');
        if (!siv.classList.contains('multi-select')) {
            for (let x of sdpiItemChildren) x.classList.remove('selected');
        }
        if (!siv.classList.contains('no-select')) {
            e.classList.toggle('selected');
        }
    }

    if (sdpiItemGroup && !sdpiItemChildren.length) {
        for (let x of ['input', 'meter', 'progress']) {
            sdpiItemChildren = sdpiItemGroup.querySelectorAll(x);
            if (sdpiItemChildren.length) break;
        }
    };

    if (e.selectedIndex) {
        idx = e.selectedIndex;
    } else {
        sdpiItemChildren.forEach((ec, i) => {
            if (ec.classList.contains('selected')) {
                selectedElements.push(ec.innerText);
            }
            if (ec === e) idx = i;
        });
    };

    const returnValue = {
        key: e.id || sdpiItem.id,
        value: isList ? e.innerText : (e.value ? (e.type === 'file' ? decodeURIComponent(e.value.replace(/^C:\\fakepath\\/, '')) : e.value) : e.getAttribute('value')),
        group: sdpiItemGroup ? sdpiItemGroup.id : false,
        index: idx,
        selection: selectedElements,
        checked: e.checked
    };

    /** Just simulate the original file-selector:
     * If there's an element of class '.sdpi-file-info'
     * show the filename there
     */
    if (e.type === 'file') {
        const info = sdpiItem.querySelector('.sdpi-file-info');
        if (info) {
            const s = returnValue.value.split('/').pop();
            info.innerText = s.length > 28 ? s.substr(0, 10) + '...' + s.substr(s.length - 10, s.length) : s;
        }
    }

    sendValueToPlugin(returnValue, 'sdpi_collection');
}

function addDynamicStyles(clrs) {
    const node = document.getElementById('#sdpi-dynamic-styles') || document.createElement('style');
    if (!clrs.mouseDownColor) clrs.mouseDownColor = fadeColor(clrs.highlightColor, -100);
    const clr = clrs.highlightColor.slice(0, 7);
    const clr1 = fadeColor(clr, 100);
    const clr2 = fadeColor(clr, 60);
    const metersActiveColor = fadeColor(clr, -60);

    node.setAttribute('id', 'sdpi-dynamic-styles');
    node.innerHTML = `

    input[type="radio"]:checked + label span,
    input[type="checkbox"]:checked + label span {
        background-color: ${clrs.highlightColor};
    }

    input[type="radio"]:active:checked + label span,
    input[type="radio"]:active + label span,
    input[type="checkbox"]:active:checked + label span,
    input[type="checkbox"]:active + label span {
      background-color: ${clrs.mouseDownColor};
    }

    input[type="radio"]:active + label span,
    input[type="checkbox"]:active + label span {
      background-color: ${clrs.buttonPressedBorderColor};
    }

    td.selected,
    td.selected:hover,
    li.selected:hover,
    li.selected {
      color: white;
      background-color: ${clrs.highlightColor};
    }

    .sdpi-file-label > label:active,
    .sdpi-file-label.file:active,
    label.sdpi-file-label:active,
    label.sdpi-file-info:active,
    input[type="file"]::-webkit-file-upload-button:active,
    button:active {
      background-color: ${clrs.buttonPressedBackgroundColor};
      color: ${clrs.buttonPressedTextColor};
      border-color: ${clrs.buttonPressedBorderColor};
    }

    ::-webkit-progress-value,
    meter::-webkit-meter-optimum-value {
        background: linear-gradient(${clr2}, ${clr1} 20%, ${clr} 45%, ${clr} 55%, ${clr2})
    }

    ::-webkit-progress-value:active,
    meter::-webkit-meter-optimum-value:active {
        background: linear-gradient(${clr}, ${clr2} 20%, ${metersActiveColor} 45%, ${metersActiveColor} 55%, ${clr})
    }
    `;
    document.body.appendChild(node);
};

/** UTILITIES */

/*
    Quick utility to lighten or darken a color (doesn't take color-drifting, etc. into account)
    Usage:
    fadeColor('#061261', 100); // will lighten the color
    fadeColor('#200867'), -100); // will darken the color
*/
function fadeColor(col, amt) {
    const min = Math.min, max = Math.max;
    const num = parseInt(col.replace(/#/g, ''), 16);
    const r = min(255, max((num >> 16) + amt, 0));
    const g = min(255, max((num & 0x0000FF) + amt, 0));
    const b = min(255, max(((num >> 8) & 0x00FF) + amt, 0));
    return '#' + (g | (b << 8) | (r << 16)).toString(16).padStart(6, 0);
}
