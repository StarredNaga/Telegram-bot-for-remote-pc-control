namespace TelegramBot.Models;

/// <summary>
/// Info of command
/// </summary>
public class CommandInfo
{
    /// <summary>
    /// Name of command
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of command
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Command category
    /// </summary>
    public string Category { get; set; } = "General";
    
    /// <summary>
    /// Emoji icon of command (idk why)
    /// </summary>
    public string EmojiIcon { get; set; } = "⚙️";
    
    /// <summary>
    /// Parameters of command (specify an empty list if they are not required)
    /// </summary>
    public List<ParameterInfo> Parameters { get; set; } = new();
}