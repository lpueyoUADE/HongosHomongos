using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnEvents : MonoBehaviour
{
    public static Action OnTurnStart;
    public static Action<IProjectile> OnTurnEnd;
    public static Action OnProjectileDeath;
    public static Action<List<BaseCharacter>> OnCharactersListUpdate;
    public static Action OnGameEnded;
    
}
