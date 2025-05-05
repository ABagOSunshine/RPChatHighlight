# RPChatHighlights
This is a plugin made with the [![Dalamud SDK](https://img.shields.io/badge/Dalamud%20SDK-eb4c42?style=flat)](https://dalamud.dev) for the [![XIVLauncher](https://img.shields.io/badge/XIVLauncher-007acc?style=flat)](https://goatcorp.github.io)

The plugin is made to allow the user to have text surrounded by Astrisks and Parenthesis Colored in a different way other than the main text.

<img width="302" alt="Example" src="Example.PNG" />

## Main Points

* Small-ish Plugin

  * /RPH - To Toggle the plugin ON/OFF
  * /rphighlight - To Open The Config Window
* Choose Whether To Keep In Astrisks and Parenthasis Separately
* Choose any Color From List Of FFXIV Compatible Colors!
 
I haven't really done too much coding so I'm not sure of the full impact/stability of the plugin but I've tried my best!

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `ESC` and select `Dalamud Settings` to open the settings.
    * In here, go to `Experimental`, and add the full path to the `SamplePlugin.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and the `SamplePlugin` should be visible. Enable it.
3. You should now be able to use `/pmycommand` (chat) or `pmycommand` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.


### Building From Code
1. Open up `SamplePlugin.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `SamplePlugin/bin/x64/Debug/SamplePlugin.dll` (or `Release` if appropriate.)

This plugin is built off of [![Sample Plugin](https://img.shields.io/badge/Sample%20Plugin-43b581?style=flat)](https://github.com/goatcorp/SamplePlugin) by the AWESOME Goatcorp
