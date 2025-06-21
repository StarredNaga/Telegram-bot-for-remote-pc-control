using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Services;

/// <summary>
/// Telegram update handler
/// </summary>
public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly CommandHandler _commandHandler;
    private readonly StateManager _stateManager;
    private readonly SecurityService _securityService;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        ITelegramBotClient botClient,
        CommandHandler commandHandler,
        StateManager stateManager,
        SecurityService securityService,
        ILogger<UpdateHandler> logger)
    {
        _botClient = botClient;
        _commandHandler = commandHandler;
        _stateManager = stateManager;
        _securityService = securityService;
        _logger = logger;
    }
    
    /// <summary>
    /// First validation of update
    /// </summary>
    /// <param name="botClient">Telegram bot</param>
    /// <param name="update">Telegram update</param>
    /// <param name="cancellationToken"></param>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // if update is not message return
        if (update is { Message.Type: not MessageType.Text }
            or { Type: not UpdateType.Message }) return;

        try
        {
            await HandleMessageAsync(update, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling update");
        }
    }
    
    /// <summary>
    /// Handle message input
    /// </summary>
    /// <param name="update">Telegram update</param>
    /// <param name="cancellationToken"></param>
    public async Task HandleMessageAsync(Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        var chatId = message.Chat.Id;
        var text = message.Text;
        
        // if user can't use commands return
        if (!_securityService.IsUserAllowed(chatId))
        {
            await _botClient.SendMessage(
                chatId,
                "⛔ Доступ запрещен",
                cancellationToken: cancellationToken);
            
            return;
        }
        
        // If input is '/cancel' clear states
        if (text.Equals("/cancel", StringComparison.OrdinalIgnoreCase))
        {
            _stateManager.ClearState(chatId);
            
            await _botClient.SendMessage(
                chatId,
                "❌ Операция отменена",
                cancellationToken: cancellationToken);
            
            return;
        }
        
        // Get current state
        var state = _stateManager.GetState(chatId);
        
        // If it null handle parameter input
        if (state != null)
        {
            await _commandHandler.HandleParameterInputAsync(chatId, text, state);
            
            return;
        }
        
        // If input isn't command return
        if (string.IsNullOrEmpty(text) || !text.StartsWith("/"))
        {
            await _botClient.SendMessage(
                chatId,
                "ℹ️ Используйте команды для управления ПК.\nВведите /commands для получения списка всех команд.",
                cancellationToken: cancellationToken);
            
            return;
        }
        
        await _commandHandler.HandleCommandAsync(chatId, text);
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }
}