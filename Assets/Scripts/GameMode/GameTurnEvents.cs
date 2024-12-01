using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnEvents : MonoBehaviour
{
    private static bool _isGameFinished = false;
    public static bool IsGameFinished => _isGameFinished;

    private static bool _isGamePaused = false;
    public static bool IsGamePaused => _isGamePaused;

    public static Action OnTurnStart;
    public static Action OnTurnEndManager;
    public static Action<IProjectile> OnTurnEnd;
    public static Action OnProjectileDeath;
    public static Action<List<BaseCharacter>> OnCharactersListUpdate;
    public static Action OnGameEnded;
    
    public static void UpdateGameFinished(bool isFinished)
    {
        _isGameFinished = isFinished;
    }

    public static void UpdateGamePaused(bool isPaused)
    {
        _isGamePaused = isPaused;
    }
}
