using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterValues : MonoBehaviour
{
    private bool _isOnNodeJump;
    private bool _isOnNodeDrop;

    public bool OnNodeJump => _isOnNodeJump;
    public bool OnNodeDrop => _isOnNodeDrop;

    private void Awake() 
    {
        PathfindingEvents.OnNodeJumpUpdate += NodeJumpUpdate;
        PathfindingEvents.OnNodeDropUpdate += NodeJumpDrop;
    }

    private void OnDestroy() 
    {
        PathfindingEvents.OnNodeJumpUpdate -= NodeJumpUpdate;
        PathfindingEvents.OnNodeDropUpdate -= NodeJumpDrop;
    }

    private void NodeJumpUpdate(GameObject character, bool status)
    {
        if (character != this.gameObject) return;
        _isOnNodeJump = status;
    }

    private void NodeJumpDrop(GameObject character, bool status)
    {
        if (character != this.gameObject) return;
        _isOnNodeDrop = status;
    }
}
