using Telegram.Bot;
using TelegramBot.Commands;
using TelegramBot.Services;

var builder = Host.CreateApplicationBuilder(args);

// Add app configurations
builder.Configuration.AddJsonFile("Configurations.json");

// Get bot token
var token = builder.Configuration["TelegramBot:Token"];

// Throw extension if token is null
if (string.IsNullOrEmpty(token)) throw new ArgumentException("TelegramBot token is required");

// Add telegram bot
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));

// Add state manager
builder.Services.AddSingleton<StateManager>();

// Add security service
builder.Services.AddSingleton<SecurityService>();

// Add all implementations of ICommand
builder.Services.Scan(scan => scan
    .FromAssemblyOf<ICommand>()
    .AddClasses(classes => classes.AssignableTo<ICommand>())
    .AsImplementedInterfaces()
    .WithTransientLifetime());

// Add command registry
builder.Services.AddSingleton<CommandRegistry>();

// Add command handler
builder.Services.AddSingleton<CommandHandler>();

// Add update handler
builder.Services.AddSingleton<UpdateHandler>();

builder.Services.AddHostedService<TelegramBotWorker>();

var host = builder.Build();
host.Run();