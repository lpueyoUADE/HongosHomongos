using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManagerEvents 
{
    public static Action<List<BaseCharacter>> OnUpdateAICharacters;
    public static Action<IControlleable, bool> OnCharacterControlUpdate; 
}
