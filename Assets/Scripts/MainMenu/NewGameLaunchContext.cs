using UnityEngine;

/// <summary>
/// Holds the one-time choice made in the main menu while the Intro scene loads.
/// It deliberately is not saved: every New Game button press makes a new choice.
/// </summary>
public static class NewGameLaunchContext
{
    private static bool _hasPendingNewGame;
    private static bool _playOpeningCinematic;

    public static void Begin(bool playOpeningCinematic)
    {
        _hasPendingNewGame = true;
        _playOpeningCinematic = playOpeningCinematic;
    }

    public static bool TryConsume(out bool playOpeningCinematic)
    {
        playOpeningCinematic = _playOpeningCinematic;

        if (!_hasPendingNewGame)
            return false;

        _hasPendingNewGame = false;
        return true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        _hasPendingNewGame = false;
        _playOpeningCinematic = false;
    }
}
