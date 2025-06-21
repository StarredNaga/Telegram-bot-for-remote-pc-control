using System.Collections.Concurrent;
using TelegramBot.Models;

namespace TelegramBot.Services;

/// <summary>
/// State machine to get user input
/// </summary>
public class StateManager
{
    private readonly ConcurrentDictionary<long, UserState> _userStates = new();

    public void SetState(long userId, UserState state)
    {
        _userStates[userId] = state;
    }

    public UserState? GetState(long userId)
    {
        return _userStates.TryGetValue(userId, out var state) ? state : null;
    }

    public void ClearState(long userId)
    {
        _userStates.TryRemove(userId, out _);
    }
}