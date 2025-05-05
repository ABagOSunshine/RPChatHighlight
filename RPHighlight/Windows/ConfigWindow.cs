using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Lumina.Excel.Sheets;
using Dalamud.Interface.ImGuiNotification;


namespace SamplePlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    private Plugin myPlugin;
    private readonly IDataManager myDataManager;

    private Dictionary<ushort, Vector4> foregroundColors = new();

    private bool tempConfigUseAstrisk = true;

    private bool tempConfigUseParenthesis = true;

    private ushort tempConfigMyColorID = new ushort();
    private Vector4 tempConfigMyColorRGBA = new Vector4(0f,0f,0f,0f);

    private ushort tempConfigOOCColorID = new ushort();
    private Vector4 tempConfigOOCColorRGBA = new Vector4(0f,0f,0f,0f);


    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin, IDataManager dataManager) : base("RolePlay Action Highlight Settings###With a constant ID")
    {
        myDataManager = dataManager;
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(600, 350);
        SizeCondition = ImGuiCond.Once; // Only apply the initial size once

        myPlugin = plugin;
        Configuration = myPlugin.Configuration;

        tempConfigUseAstrisk = Configuration.UseAstrisk;
        tempConfigUseParenthesis = Configuration.UseParenthesis;

        tempConfigMyColorID = Configuration.rpEmoteColorID;
        tempConfigMyColorRGBA = Configuration.rpEmoteColorRGBA;
        tempConfigOOCColorID = Configuration.OOCColorID;
        tempConfigOOCColorRGBA = Configuration.OOCColorRGBA;
    }

    public void Dispose() { }


    private void LoadColors()
    {
        var colorSheet = myDataManager.GameData.GetExcelSheet<UIColor>();
        foregroundColors = new Dictionary<ushort, Vector4>(colorSheet.Count);
        if (colorSheet.Equals(null))
        {
            myPlugin.ShowNotification("Colorsheet Not Found","I couldn't find the default colorsheet for FFXIV Dalamud!",NotificationType.Error);
            return;
        }
        foreach (var color in colorSheet)
        {
            var fa = color.Dark & 255;
            if (fa > 0)
            {
                var fb = (color.Dark >> 8) & 255;
                var fg = (color.Dark >> 16) & 255;
                var fr = (color.Dark >> 24) & 255;
                foregroundColors[(ushort)color.RowId] = new Vector4(fr / 255f, fg / 255f, fb / 255f, fa / 255f);
            }
        }
    }


    private void DrawColorPickerPopup( ref ushort selectedColorId, ref Vector4 selectedColor, Dictionary<ushort, Vector4> colorOptions, string popupName)
    {
        const int columns = 4;
        const float tableHeight = 400f;
        const float tableWidth = 400f;

        if (ImGui.BeginPopup(popupName))
        {
            ImGui.BeginChild($"{popupName}_Child", new Vector2(tableWidth, tableHeight), true, ImGuiWindowFlags.AlwaysVerticalScrollbar);

            if (ImGui.BeginTable($"{popupName}_Table", columns, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
            {
                int index = 0;
                foreach (var colorPair in colorOptions)
                {
                    if (index % columns == 0)
                        ImGui.TableNextRow();

                    ImGui.TableSetColumnIndex(index % columns);
                    if (ImGui.ColorButton($"{popupName}_Color_{colorPair.Key}", colorPair.Value, ImGuiColorEditFlags.NoAlpha, new Vector2(30, 30)))
                    {
                        selectedColorId = colorPair.Key;
                        selectedColor = colorPair.Value;
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.TextUnformatted($"ID {colorPair.Key}");

                    index++;
                }

                ImGui.EndTable();
            }

            ImGui.EndChild();

            // Show preview of the selected color
            ImGui.ColorButton($"{popupName}_SelectedColor", selectedColor, ImGuiColorEditFlags.NoAlpha, new Vector2(30, 30));
            ImGui.SameLine();
            ImGui.Text($"Selected ID: {selectedColorId}");

            ImGui.EndPopup();
        }
    }

    //public override void Draw()
    public override void Draw()
    {
        if (foregroundColors == null || foregroundColors.Count == 0)
        {
            LoadColors();
        }

        ImGui.Text("Choose A Color For Rp Emotes/Actions");

        ImGui.PushStyleColor(ImGuiCol.Button, tempConfigMyColorRGBA);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, tempConfigMyColorRGBA);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, tempConfigMyColorRGBA);
        if (ImGui.Button($"##{tempConfigMyColorRGBA}", new Vector2(30, 30)))
        {
            ImGui.OpenPopup("ColorPickerPopup");
        }
        ImGui.SameLine();
        ImGui.Text($"Color Selected: {tempConfigMyColorID}");
        ImGui.PopStyleColor(3);

        DrawColorPickerPopup(ref tempConfigMyColorID, ref tempConfigMyColorRGBA, foregroundColors, "ColorPickerPopup");



        ImGui.Spacing();
        ImGui.Separator();//--------------------------------------
        ImGui.Spacing();


        ImGui.Text("Choose Color For OOC Speaking");

        ImGui.PushStyleColor(ImGuiCol.Button, tempConfigOOCColorRGBA);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, tempConfigOOCColorRGBA);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, tempConfigOOCColorRGBA);
        if (ImGui.Button($"##{tempConfigOOCColorRGBA}", new Vector2(30, 30)))
        {
            ImGui.OpenPopup("OOCColorPickerPopup");
        }
        ImGui.SameLine();
        ImGui.Text($"Color Selected: {tempConfigOOCColorID}");
        ImGui.PopStyleColor(3);

        DrawColorPickerPopup(ref tempConfigOOCColorID, ref tempConfigOOCColorRGBA, foregroundColors, "OOCColorPickerPopup");

        ImGui.Spacing();
        ImGui.Separator();//--------------------------------------
        ImGui.Spacing();
        ImGui.Spacing();

        var localPlayer = Plugin.ClientState.LocalPlayer;
        if (localPlayer == null)
        {
            ImGui.TextUnformatted("Our local player is currently not loaded.");
            return;
        }
        else
        {
            ImGui.Text("Roleplay Will Appear Like this:");
            //ImGui.ColorButton("", Configuration.myColorRGBA, ImGuiColorEditFlags.NoAlpha, new Vector2(30, 30));

            ImGui.BeginChild("RPChatPreviewBox", new Vector2(ImGui.GetWindowSize().X - 20, 35), true);
            ImGui.BeginGroup();
            //{localPlayer.Name}
            ImGui.Text($"Player Name: Hello there Stranger!");

            ImGui.SameLine();

            //(Configuration.UseParenthesis ? "Did I Do That Right?" : "(Did I Do That Right?)")
            ImGui.TextColored(tempConfigMyColorRGBA, !tempConfigUseAstrisk ? "Raises My Hand And Waves" : "*Raises My Hand And Waves*");

            ImGui.SameLine();

            ImGui.TextColored(tempConfigOOCColorRGBA, !tempConfigUseParenthesis ? "Did I Do That Right?" : "(Did I Do That Right?)");

            ImGui.EndGroup();
            ImGui.EndChild();
        }

        ImGui.Spacing();
        ImGui.Separator();//--------------------------------------
        ImGui.Spacing();
        ImGui.Spacing();

        if (ImGui.RadioButton("Keep Astrisks In Messages?", tempConfigUseAstrisk))
        {
            tempConfigUseAstrisk = !tempConfigUseAstrisk;
        }
        if (ImGui.RadioButton("Keep Parenthesis In Messages?", tempConfigUseParenthesis))
        {
            tempConfigUseParenthesis = !tempConfigUseParenthesis;
        }


        if (ImGui.Button("Save Settings"))
        {
            Configuration.rpEmoteColorID = tempConfigMyColorID;
            Configuration.rpEmoteColorRGBA = tempConfigMyColorRGBA;
            Configuration.OOCColorID = tempConfigOOCColorID;
            Configuration.OOCColorRGBA = tempConfigOOCColorRGBA;

            Configuration.UseAstrisk = tempConfigUseAstrisk;
            Configuration.UseParenthesis = tempConfigUseParenthesis;
            
            Configuration.Save();
            myPlugin.ShowNotification("Config Saved!","Saved Your Settings!",NotificationType.Success);
            
        }




        
        
    }





}
