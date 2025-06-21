namespace TelegramBot.Models;

/// <summary>
/// Command parameter info
/// </summary>
public class ParameterInfo
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Parameter description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Parameter type
    /// </summary>
    public ParamType Type { get; set; } = ParamType.Text;
    
    /// <summary>
    /// Default value
    /// </summary>
    public object? DefaultValue { get; set; }
    
    /// <summary>
    /// Is the parameter required
    /// </summary>
    public bool IsRequired { get; set; } = true;
    
    /// <summary>
    /// Parameter validator (look at shutdown parameter)
    /// </summary>
    public Func<object, bool>? Validator { get; set; }
    
    /// <summary>
    /// Message if error in validation
    /// </summary>
    public string? ValidationMessage { get; set; }
}