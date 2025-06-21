using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Models;

namespace TelegramBot.Services;

/// <summary>
/// Handler for commands
/// </summary>
public class CommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly StateManager _stateManager;
    private readonly CommandRegistry _commandRegistry;
    private readonly SecurityService _securityService;

    public CommandHandler(
        ITelegramBotClient botClient,
        StateManager stateManager,
        CommandRegistry commandRegistry,
        SecurityService securityService)
    {
        _botClient = botClient;
        _stateManager = stateManager;
        _commandRegistry = commandRegistry;
        _securityService = securityService;
    }
    
    /// <summary>
    /// Handle command
    /// </summary>
    /// <param name="chatId">Id of chat with user</param>
    /// <param name="commandText">User entered text</param>
    public async Task HandleCommandAsync(long chatId, string commandText)
    {
        var commandName = commandText
            .TrimStart('/')
            .Split(' ')[0]
            .ToLower();
        
        // Get command by name
        var command =  _commandRegistry.GetCommand(commandName);
        
        // If command with that name doesn't exist return
        if (command is null)
        {
            await _botClient.SendMessage(
                chatId,
                "❌ Команда не найдена");

            return;
        }
        
        // Id user cant use command return
        if (!_securityService.CanUserExecuteCommand(chatId, commandName))
        {
            await _botClient.SendMessage(
                chatId,
                "⛔ Недостаточно прав");
            
            return;
        }
        
        // If command doesnt have parameters execute immediately
        if (!command.Info.Parameters.Any())
        {
            await ExecuteCommandAsync(chatId,
                command,
                new Dictionary<string, object>());
            
            return;
        }
        
        // Create state for command
        var state = new UserState
        {
            CommandName = commandName,
            CurrentParameterIndex = 0,
            Parameters = new Dictionary<string, object>()
        };
        
        // Set current state
        _stateManager.SetState(chatId, state);
        
        // Get first command parameter
        var firstParameter = command.Info.Parameters.First();
        
        // Request parameter
        await RequestParameterAsync(chatId, firstParameter);
    }
    
    private async Task RequestParameterAsync(long chatId, ParameterInfo parameter)
    {
        var message = $"Введите {parameter.Description}:";
        
        if (parameter.DefaultValue != null)
            message += $"\n(По умолчанию: {parameter.DefaultValue})";

        await _botClient.SendMessage(chatId, message);
    }

    private object ConvertParameter(string input, ParamType type)
    {
        return type switch
        {
            ParamType.Text => input,
            ParamType.Number => int.TryParse(input, out int num) ? num : 
                throw new FormatException("Требуется число"),
            ParamType.Boolean => input.ToLower() switch
            {
                "да" or "yes" or "true" or "1" => true,
                "нет" or "no" or "false" or "0" => false,
                _ => throw new FormatException("Требуется Да/Нет")
            },
            ParamType.FilePath => input,
            _ => input
        };
    }
    
    public async Task HandleParameterInputAsync(long chatId, string input, UserState state)
    {   
        // Get current command by name from the state
        var command = _commandRegistry.GetCommand(state.CommandName);
        
        // Get parameters from state
        var parameters = command!.Info.Parameters;
        
        // Get current parameter index
        var currentParamIndex = state.CurrentParameterIndex;
        
        // Get current parameter
        var currentParam = parameters[currentParamIndex];

        try
        {
            // Validate and convert input
            var value = ConvertParameter(input, currentParam.Type);
            ValidateParameter(value, currentParam);
            
            // Save data
            state.Parameters[currentParam.Name] = value;
            
            // Check if any parameters left
            if (currentParamIndex + 1 < parameters.Count)
            {
                state.CurrentParameterIndex++;
                _stateManager.SetState(chatId, state);
                await RequestParameterAsync(chatId, parameters[state.CurrentParameterIndex]);
            }
            else
            {
                // Execute
                _stateManager.ClearState(chatId);
                await ExecuteCommandAsync(chatId, command, state.Parameters);
            }
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(
                chatId, 
                $"❌ Ошибка: {ex.Message}\nПожалуйста, введите значение еще раз:");
        }
    }
    
    private void ValidateParameter(object value, ParameterInfo param)
    {
        if (param.Validator != null && !param.Validator(value))
            throw new ArgumentException(param.ValidationMessage ?? "Недопустимое значение");
    }

    private async Task ExecuteCommandAsync(long chatId, ICommand command, Dictionary<string, object> parameters)
    {
        try
        {
            var result = await command.ExecuteAsync(new UserRequest
            {
                ChatId = chatId,
                Parameters = parameters
            });

            await SendCommandResult(chatId, result);
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(
                chatId, 
                $"⚠️ Ошибка выполнения команды: {ex.Message}");
        }
    }

    private async Task SendCommandResult(long chatId, CommandResult result)
    {
        // If result is file
        if (result.FileData is { Length: > 0 })
        {
            await using var stream = new MemoryStream(result.FileData);
            
            await _botClient.SendDocument(
                chatId: chatId,
                document: InputFile.FromStream(stream, result.FileName ?? "file"),
                caption: result.Message);
        }
        // If result is just text
        else
        {
            await _botClient.SendMessage(chatId, result.Message);
        }
    }
}