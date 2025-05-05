using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using SamplePlugin.Windows;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Dalamud.Interface.ImGuiNotification;


namespace SamplePlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static INotificationManager myNotificationManager { get; private set; } = null!;



    private const string MainCommandName = "/rphighlight";
    private const string ToggleCommandName = "/rph";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("RpHighLight");
    private ConfigWindow ConfigWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();


        ConfigWindow = new ConfigWindow(this, DataManager);

        WindowSystem.AddWindow(ConfigWindow);

        CommandManager.AddHandler(MainCommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "To Open the Config"
        });
        CommandManager.AddHandler(ToggleCommandName, new CommandInfo(OnToggleCommand)
        {
            HelpMessage = "To toggle the plugin on and off"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        //Subscribe to chat messages and use our method to handle the event
        ChatGui.ChatMessage += OnChatMessage;
        Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
        
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        ConfigWindow.Dispose();

        //Unsubscribe from chat messages when we dispose of plugin
        ChatGui.ChatMessage -= OnChatMessage;

        CommandManager.RemoveHandler(MainCommandName);
        CommandManager.RemoveHandler(ToggleCommandName);
    }

    public void ShowNotification(string customTitle, string customMessage, NotificationType notifType)
    {
        string[] words = customMessage.Split(' '); // Split the string into words
        string miniMessage = string.Join(" ", words);

        var notification = new Notification
        {
            Title = customTitle,
            Content = customMessage,
            MinimizedText = miniMessage,
            Type = notifType // You can also use Warning, Error, etc.
        };

        // Show the notification
        myNotificationManager.AddNotification(notification);
    }






    private void OnCommand(string command, string args)
    {
        ToggleMainUI();
    }
    private void OnToggleCommand(string command, string args)
    {
            Configuration.pluginToggle = !Configuration.pluginToggle;
            ShowNotification("Toggled Plugin", $"Toggled Plugin:{Configuration.pluginToggle}", NotificationType.Warning);
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => ConfigWindow.Toggle();

    //Method called on the Subscribed event chat message!

    
    private static readonly List<XivChatType> BLACKLISTCHATTYPES = new List<XivChatType>
    {
        XivChatType.CustomEmote,
        XivChatType.SystemMessage,
        XivChatType.SystemError,
        XivChatType.Echo,
        XivChatType.Notice,
        XivChatType.RetainerSale,
        XivChatType.NPCDialogueAnnouncements,
        XivChatType.NoviceNetwork,
        XivChatType.None,
        XivChatType.Debug,
        XivChatType.GatheringSystemMessage
    };
    
    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if(BLACKLISTCHATTYPES.Contains(type) || !Configuration.pluginToggle)
            return;


        string rawText = message.TextValue;

        // Create regex patterns to match asterisks and parentheses
        var asteriskMatches = Regex.Matches(rawText, @"\*(.+?)\*");
        var parenthesisMatches = Regex.Matches(rawText, @"\((.+?)\)");

        var payloads = new List<Payload>();
        int lastIndex = 0;

        // Process asterisk text
        foreach (Match match in asteriskMatches)
        {
            // Add text before the asterisk
            if (match.Index > lastIndex)
            {
                payloads.Add(new TextPayload(rawText.Substring(lastIndex, match.Index - lastIndex)));
            }

            payloads.Add(new UIForegroundPayload(Configuration.rpEmoteColorID));
            payloads.Add(new TextPayload(Configuration.UseAstrisk ? $"*{match.Groups[1].Value}*" : match.Groups[1].Value));
            payloads.Add(UIForegroundPayload.UIForegroundOff);

            lastIndex = match.Index + match.Length;
        }

        // Process parenthesis text
        foreach (Match match in parenthesisMatches)
        {
            // Add text before the parentheses
            if (match.Index > lastIndex)
            {
                payloads.Add(new TextPayload(rawText.Substring(lastIndex, match.Index - lastIndex)));
            }

            // Process text inside parentheses (e.g., apply different formatting)
            payloads.Add(new UIForegroundPayload(Configuration.OOCColorID)); // Could be a different color
            payloads.Add(new TextPayload(Configuration.UseParenthesis ? $"({match.Groups[1].Value})" : match.Groups[1].Value));
            payloads.Add(UIForegroundPayload.UIForegroundOff);

            lastIndex = match.Index + match.Length;
        }

        // Add any remaining text after the last processed chunk
        if (lastIndex < rawText.Length)
        {
            payloads.Add(new TextPayload(rawText.Substring(lastIndex)));
        }

        // Set the processed message with the mixed payloads
        message = new SeString(payloads);
    }

    
    
    private void OLDOnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        string rawText = message.TextValue;

        if (!rawText.Contains('*')) return;

        var matches = Regex.Matches(rawText, @"\*(.+?)\*");
        if (matches.Count == 0) return;

        var payloads = new List<Payload>();
        int lastIndex = 0;


        foreach (Match match in matches)
        {
            // Add normal text before *
            if (match.Index > lastIndex)
            {
                payloads.Add(new TextPayload(rawText.Substring(lastIndex, match.Index - lastIndex)));
            }
            // Add colored text
            payloads.Add(new UIForegroundPayload(Configuration.rpEmoteColorID));
            payloads.Add(new TextPayload(match.Groups[1].Value));
            payloads.Add(UIForegroundPayload.UIForegroundOff);

            lastIndex = match.Index + match.Length;
        }

        // Add any remaining text
        if (lastIndex < rawText.Length)
        {
            payloads.Add(new TextPayload(rawText.Substring(lastIndex)));
        }

        message = new SeString(payloads);
    }
}
