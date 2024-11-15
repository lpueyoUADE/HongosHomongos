using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntroductionSequence : MonoBehaviour
{
    private Camera _camera;
    private List<BaseCharacter> _charactersList;

    private int _index = 0;
    private bool _doTransition = false;
    private Coroutine _transitionTimer;
    private Coroutine _characterWaitTimer;
    private Coroutine _delayBeforeStartingGame;

    public static Action<Camera, List<BaseCharacter>> OnStartIntroductionSequence;

    private void Awake() 
    {
        OnStartIntroductionSequence += IntroductionSequence;

    }

    private void OnDestroy() 
    {
        OnStartIntroductionSequence -= IntroductionSequence;
    }

    private void FixedUpdate() 
    {
        if (!_doTransition) return;
        
        Vector3 camPos = _camera.transform.position;
        Vector3 charPos = _charactersList[_index].transform.position;
        charPos.z = camPos.z;

        Vector3 lerpedPos = Vector3.LerpUnclamped(camPos, charPos, Time.fixedDeltaTime * GameManagerEvents.ModeSettings.CameraTransitionTime);
        _camera.transform.position = lerpedPos;

        if (Vector3.Distance(camPos, charPos) <= 0.5f) _doTransition = false;
    }

    private void IntroductionSequence(Camera cameraObject, List<BaseCharacter> charactersList)
    {
        _camera = cameraObject;
        _charactersList = charactersList;

        // Start timers
        _transitionTimer = StartCoroutine(StartTransition(GameManagerEvents.ModeSettings.CameraTransitionTime));
    }

    IEnumerator StartTransition(float time)
    {
        string text = (_charactersList[_index].CharacterData.IsAIControlled ? "(Enemy) " : "(You) ") + _charactersList[_index].CharacterName;

        InGameUIEvents.OnUpdateTurnTime?.Invoke(text, false);
        InGameUIEvents.OnPlayUISound?.Invoke(GameManagerEvents.ModeSettings.CharacterIntroductionAudioClip);
        InGameUIEvents.OnPortraitUpdate(_index, PortraitStatus.CurrentTurn);

        _doTransition = true;
        yield return new WaitForSeconds(time);
        _characterWaitTimer = StartCoroutine(StartCharacterTime(GameManagerEvents.ModeSettings.DelayBetweenCharacters));
    }

    IEnumerator StartCharacterTime(float time)
    {
        yield return new WaitForSeconds(time);

        InGameUIEvents.OnPortraitUpdate(_index, PortraitStatus.Idle);
        _index++;

        if (_index < _charactersList.Count) _transitionTimer = StartCoroutine(StartTransition(GameManagerEvents.ModeSettings.CameraTransitionTime));
        else _delayBeforeStartingGame = StartCoroutine(StartDelayToGame(GameManagerEvents.ModeSettings.DelayBeforeStartingGame));
    }

    IEnumerator StartDelayToGame(float time)
    {
        _index = 0;
        _doTransition = true;
        
        InGameUIEvents.OnUpdateTurnTime?.Invoke("Get ready!", false);
        InGameUIEvents.OnPlayUISound?.Invoke(GameManagerEvents.ModeSettings.CharacterIntroductionAudioClip);
        yield return new WaitForSeconds(time);

        GameManagerEvents.OnIntroductionSequenceEnded?.Invoke();
    }
}
