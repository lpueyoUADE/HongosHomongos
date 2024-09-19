using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEvents : MonoBehaviour
{
    public static Action<BaseCharacter> OnCharacterDeath;
}
