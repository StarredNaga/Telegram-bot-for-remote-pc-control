namespace TelegramBot.Services;

/// <summary>
/// Service to control pc security
/// </summary>
public class SecurityService
{
    private readonly HashSet<long> _allowedUsers = new();
    private readonly Dictionary<string, string[]> _commandPermissions = new();

    public SecurityService(IConfiguration config)
    {
        // Get allowed users from configurations
        var allowedUsers = config.GetSection("Security:AllowedUsers").Get<long[]>();
        
        foreach (var userId in allowedUsers)
        {
            _allowedUsers.Add(userId);
        }

        // Get commands permissions
        var permissions = config.GetSection("Security:CommandPermissions").Get<Dictionary<string, long[]>>();
        
        foreach (var (command, users) in permissions)
        {
            _commandPermissions[command] = users.Select(u => u.ToString()).ToArray();
        }
    }

    /// <summary>
    /// Return if user can use commands
    /// </summary>
    /// <param name="userId">Id of chat with user</param>
    /// <returns>true - if can use, false - if cannot use</returns>
    public bool IsUserAllowed(long userId) => _allowedUsers.Contains(userId);
    
    /// <summary>
    /// Return if user can use command
    /// </summary>
    /// <param name="userId">Chat id with user</param>
    /// <param name="commandName">Name of the command</param>
    /// <returns>true - if can use, false if cannot use</returns>
    public bool CanUserExecuteCommand(long userId, string commandName)
    {   
        // return true if command doesnt have permissions
        if (!_commandPermissions.TryGetValue(commandName, out var allowedUsers))
            return true;

        return allowedUsers.Contains(userId.ToString());
    }
}