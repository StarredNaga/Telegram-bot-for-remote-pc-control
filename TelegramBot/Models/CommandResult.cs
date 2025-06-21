namespace TelegramBot.Models;

/// <summary>
/// Result of command
/// </summary>
/// <param name="Message">Text message</param>
/// <param name="FileData">File data as byte array</param>
/// <param name="FileName">File name (with path)</param>
public record class CommandResult(
    string Message,
    byte[]? FileData = null,
    string? FileName = null);