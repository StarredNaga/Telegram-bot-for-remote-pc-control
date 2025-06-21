using System.Text;
using TelegramBot.Models;

namespace TelegramBot.Commands;

/// <summary>
/// Command to get all commands
/// </summary>
public class CommandListCommand : ICommand
{
    private readonly IServiceProvider _services;

    public CommandListCommand(IServiceProvider services)
    {
        _services = services;
    }
    
    public CommandInfo Info => new()
    {
        Name = "commands",
        Description = "Список всех команд",
        EmojiIcon = "🔌",
        Category = "System",
        Parameters = []
    };
    
    public async Task<CommandResult> ExecuteAsync(UserRequest request)
    {
        var commands = _services.GetServices<ICommand>();

        var builder = new StringBuilder();

        foreach (var command in commands)
        {
            builder.Append($"/{command.Info.Name} - {command.Info.Description}\n");
        }
        
        return new CommandResult(builder.ToString());
    }
}