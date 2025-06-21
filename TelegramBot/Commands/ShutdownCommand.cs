using System.Diagnostics;
using TelegramBot.Models;

namespace TelegramBot.Commands;

/// <summary>
/// Command to shutdown pc (Correctly work only on windows)
/// </summary>
public class ShutdownCommand : ICommand
{   
    private readonly IConfiguration _configuration;

    public ShutdownCommand(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public CommandInfo Info => new()
    {
        Name = "shutdown",
        Description = "Выключение компьютера",
        EmojiIcon = "🔌",
        Category = "System",
        Parameters = new List<ParameterInfo>
        {
            new()
            {
                Name = "delay",
                Description = "Задержка в секундах",
                Type = ParamType.Number,
                DefaultValue = 30,
                Validator = value => (int)value > 0,
                ValidationMessage = "Задержка должна быть положительным числом"
            }
        }
    };

    public async Task<CommandResult> ExecuteAsync(UserRequest request)
    {
        var terminal = _configuration["System:Terminal"];
        
        if (OperatingSystem.IsWindows())
        {
            var delay = request.GetParam("delay", 30);
            
            var delayParam = delay > 0 ? $"/t {delay}" : "";

            var process = new ProcessStartInfo
            {
                FileName = terminal,
                Arguments = $"shutdown /s {delayParam} /f"
            };

            Process.Start(process);

        }
        else
        {
            var delay = request.GetParam("delay", 30);
            
            var delayParam = delay > 0 ? $"+{delay / 60}" : "-c";

            var process = new ProcessStartInfo
            {
                FileName = "sudo",
                Arguments = $"shutdown {delayParam}"
            };
            
            Process.Start(process);
        }
        
        return new CommandResult($"🖥️ Компьютер будет выключен через {request.GetParam("delay", 30)} секунд");
    }
}