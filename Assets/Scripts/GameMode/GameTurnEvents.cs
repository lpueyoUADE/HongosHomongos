using System;
using UnityEngine;

public class GameTurnEvents : MonoBehaviour
{
    public static Action<IProjectile> OnTurnEnd;
    public static Action OnProjectileDeath;
}
