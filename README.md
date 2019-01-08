[![NuGet](https://img.shields.io/nuget/v/streamdeck-client-csharp.svg?style=flat)](https://www.nuget.org/packages/streamdeck-client-csharp)

# streamdeck-client-csharp
A C# client library for the Elgato Stream Deck device.

## What is this?
Elgato just released their [Official Stream Deck SDK](https://developer.elgato.com/documentation/stream-deck/sdk/overview/), but it only supported Javascript, C++, and Objective-C.  I was frustrated as I prefer C# as my go-to language.  So, I wrote this wrapper to ease writing plugins.

## Requirements
This library uses the native WebSocket support found in Windows 8 & higher. This means that any application that uses this library must be running Windows 8 or higher.

## Current functionality
The library should support all features documented by the SDK:
- Connection and Disconnection
- Sending messages
- Receiving events

## Feature roadmap
At this point, we should only need to monitor the Official SDK for new features and implement them.

## How do I get started using it?
Download the NuGet package by searching for "streamdeck-client-csharp" or downloading it manually from https://www.nuget.org/packages/streamdeck-client-csharp.  If you clone the repository, you will see the TestPlugin C# project. When this is compiled it creates a folder in the output directory `com.tyren.testplugin.sdPlugin`.  If you copy this into your Stream Deck's [Plugin Folder](https://developer.elgato.com/documentation/stream-deck/sdk/create-your-own-plugin/), and launch Stream Deck, you should see the `C# Test Plugin` category show up with some test actions inside.

## I found a bug, who do I contact?
Just head over to the https://github.com/TyrenDe/streamdeck-client-csharp/issues page and create a new issue.

## License
MIT License

Copyright (c) 2019 Shane DeSeranno

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
