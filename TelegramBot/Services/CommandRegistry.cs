using TelegramBot.Commands;

namespace TelegramBot.Services;

/// <summary>
/// List of all commands
/// </summary>
public class CommandRegistry
{
    /// <summary>
    /// Dictionary with command name and link on command
    /// </summary>
    private readonly Dictionary<string, ICommand> _commands;
    private readonly IServiceProvider _serviceProvider;

    public CommandRegistry(IServiceProvider serviceProvider, IEnumerable<ICommand> commands)
    {
        _serviceProvider = serviceProvider;
        _commands = commands.ToDictionary(
            c => c.Info.Name.ToLower(), 
            c => c);
    }
    
    /// <summary>
    /// Get command by name
    /// </summary>
    /// <param name="name">Command name to search</param>
    /// <returns>Nullable command</returns>
    public ICommand? GetCommand(string name)
    {
        return _commands.TryGetValue(name.ToLower(), out var command) ? command : null;
    }
    
    /// <summary>
    /// Get all commands
    /// </summary>
    /// <returns>List of all commands</returns>
    public IEnumerable<ICommand> GetAllCommands()
    {
        return _commands.Values;
    }
}