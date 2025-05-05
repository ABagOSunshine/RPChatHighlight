# RPChatHighlights

[![Use This Template badge](https://img.shields.io/badge/Use%20This%20Template-0?logo=github&labelColor=grey)][new-repo]


This is a plugin made for Dalumud SDk for the XIVLauncher.

The plugin is made to allow the user to have text surrounded by Astrisks and Parenthasis Colored in a different way than the main text.

<img width="302" alt="Example" src="https://github.com/user-attachments/assets/f8c3f5fd-a732-492d-a93c-f82056bba34b" />

## Main Points

* Small-ish Plugin

  * /RPH - To Toggle the plugin ON/OFF
  * /rphighlight - To Open The Config Window
* Action/Emote Color highlights
* Choose Whether To Keep In Astrisks and Parenthasis Separately
 
I haven't really done too much coding so I'm not sure of the full impact/stability of the plugin but I've tried my best!

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the full path to the `SamplePlugin.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and the `SamplePlugin` should be visible. Enable it.
3. You should now be able to use `/pmycommand` (chat) or `pmycommand` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.


### Building From Code
1. Open up `SamplePlugin.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `SamplePlugin/bin/x64/Debug/SamplePlugin.dll` (or `Release` if appropriate.)
