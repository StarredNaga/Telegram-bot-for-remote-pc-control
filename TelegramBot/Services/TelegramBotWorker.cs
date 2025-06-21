using Telegram.Bot;
using Telegram.Bot.Polling;

namespace TelegramBot.Services;

public class TelegramBotWorker : BackgroundService
{
    private readonly ILogger<TelegramBotWorker> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly UpdateHandler _updateHandler;

    public TelegramBotWorker(
        ILogger<TelegramBotWorker> logger,
        ITelegramBotClient botClient,
        UpdateHandler updateHandler)
    {
        _logger = logger;
        _botClient = botClient;
        _updateHandler = updateHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = []
        };
        
        _botClient
            .StartReceiving(
                receiverOptions: receiverOptions,
                updateHandler: _updateHandler.HandleUpdateAsync,
                errorHandler:_updateHandler.HandleErrorAsync,
                cancellationToken: stoppingToken);
        
        _logger.LogInformation("Starting bot...");
    }
}