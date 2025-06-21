namespace TelegramBot.Models;

/// <summary>
/// Current state of chat with user
/// </summary>
public class UserState
{
    /// <summary>
    /// CHat id
    /// </summary>
    public long ChatId { get; set; }
    
    /// <summary>
    /// The name of the command the user entered
    /// </summary>
    public string CommandName { get; set; } = string.Empty;
    
    /// <summary>
    /// Current index of command parameter
    /// </summary>
    public int CurrentParameterIndex { get; set; }
    
    /// <summary>
    /// All parameters of command
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}