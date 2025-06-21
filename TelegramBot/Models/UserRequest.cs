namespace TelegramBot.Models;

/// <summary>
/// User request from telegram
/// </summary>
public class UserRequest
{
    /// <summary>
    /// Id of chat
    /// </summary>
    public long ChatId { get; set; }
    
    /// <summary>
    /// Parameters
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    /// <summary>
    /// Returns parameter by name
    /// </summary>
    /// <param name="name">Parameter name</param>
    /// <param name="defaultValue">Default value if the parameter is not found</param>
    /// <typeparam name="T">Type of parameter</typeparam>
    /// <returns>Parameter or default value of it type</returns>
    public T GetParam<T>(string name, T defaultValue = default!)
    {
        if (Parameters.TryGetValue(name, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }
}