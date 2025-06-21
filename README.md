# Remote PC Control Telegram Bot

![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)
![.NET 9](https://img.shields.io/badge/.NET-9-512BD4?logo=dotnet&logoColor=white)
![Telegram](https://img.shields.io/badge/Telegram-2CA5E0?logo=telegram&logoColor=white)

A secure Telegram bot for remote PC management built with C# .NET 9. Operates as a background service with an extensible command architecture centered around the `ICommand` interface pattern.

## Key Features
- 🔒 **Role-Based Security** - Whitelisted user access with granular permissions
- 🧩 **Extensible Command System** - Modular architecture via `ICommand` interface
- ⚙️ **Background Service** - Runs continuously as Windows/Linux service
- 📦 **DI Container Support** - Clean dependency management
- 🚀 **.NET 9 Optimization** - Leverages latest runtime enhancements

## Supported Commands
| Command          | Description                                  |
|------------------|----------------------------------------------|
| `/shutdown`     | Initiates system shutdown (with optional delay) |
| `/commands`     | Lists all available commands                 |
| *[Your Command]* | *[Add custom commands implement ICommand interface]* |

## Getting Started

### Basic Usage
1. Launch the background service
2. Send commands to your bot in format: `/command`
3. Commands execute immediately or prompt for required parameters

### Adding Custom Commands
1. Create new class implementing `ICommand` interface (for example shutdown command)
2. The framework automatically discovers and registers your command at startup

## Configuration

Modify Configurations.json to customize behavior:

### Core Settings

```json
"System": {
    "Terminal": "path to terminal"
    // Linux application paths (e.g., "nmap": "/usr/bin/nmap")
}
```
### Telegram Settings

```json
"TelegramBot": {
    "Token": "Bot token here"
  }
```

### Security Policies

```json
"Security": {

    "AllowedUsers": [chat id's here],

    "CommandPermissions": {
      "shutdown": [ chat id's here ]
    }
  }
```
* **User Permissions**: Add users by Chat ID with specific command access
* **Command Security**: Unlisted commands are executable by all authorized users
* **If the command does not have user IDs specified, then anyone who is allowed to use the command can use it**
