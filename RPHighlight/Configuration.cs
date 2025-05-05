using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Numerics;

namespace SamplePlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    //General Config Variables
    public bool pluginToggle { get; set; } = true;
    public int Version { get; set; } = 0;
    public bool UseAstrisk { get; set; } = true;
    public bool UseParenthesis { get; set; } = true;



    //Color Config Variables
    public ushort rpEmoteColorID { get; set; } = 56;
    public Vector4 rpEmoteColorRGBA { get; set; } = new Vector4(1.0f,1.0f,1.0f,1.0f);
    public ushort OOCColorID { get; set;} = 32;
    public Vector4 OOCColorRGBA { get; set;}

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
