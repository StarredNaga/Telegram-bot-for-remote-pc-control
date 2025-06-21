using TelegramBot.Models;

namespace TelegramBot.Commands;

/// <summary>
/// Base interface of every command
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Command info (Name, description, parameters etc)
    /// </summary>
    CommandInfo Info { get; }
    
    /// <summary>
    /// Execute command
    /// </summary>
    /// <param name="request">Parameters for command (if needed)</param>
    /// <returns>Result of command (Message or file with path)</returns>
    Task<CommandResult> ExecuteAsync(UserRequest request);
}