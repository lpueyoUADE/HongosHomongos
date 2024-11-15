using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UserConfigData userConfigs;
    [SerializeField] private GameModeGeneralSettings _modeSettings;
    [SerializeField] private List<BaseCharacter> _characters = new();

    [Header("Session status")]
    public static List<BaseCharacter> _playerAliveCharacters = new();
    public List<BaseCharacter> _aiAliveCharacters = new();

    [Header("Other")]
    public bool byPassIntroduction = false;

    private void Awake()
    {
        GameManagerEvents.UpdateModeSettings(_modeSettings);
        GameManagerEvents.UpdateUserConfigs(userConfigs);

        _playerAliveCharacters.Clear();
        GameManagerEvents.OnCharacterDeath += OnCharacterDeath;
        GameManagerEvents.OnIntroductionSequenceEnded += IntroductionSequenceFinished;
    }

    private void Start()
    {
        GenerateNames();
        if (!byPassIntroduction) GameIntroductionSequence.OnStartIntroductionSequence?.Invoke(CameraEvents.Cam, _characters); // Introduce characters
        else IntroductionSequenceFinished();
    }

    private void OnDestroy()
    {
        GameManagerEvents.OnCharacterDeath -= OnCharacterDeath;
        GameManagerEvents.OnIntroductionSequenceEnded -= IntroductionSequenceFinished;
    }

    private void GenerateNames()
    {
        int chCount = _characters.Count;
        List<string> _hongoNames = _modeSettings.CharacterNamesDatabase.GetRandomNames(true, chCount);
        List<string> _sapoNames = _modeSettings.CharacterNamesDatabase.GetRandomNames(false, chCount);

        foreach (BaseCharacter character in _characters)
        {
            character.TryGetComponent(out IHongo hongo);

            if (hongo != null)
            {
                _playerAliveCharacters.Add(character);
                string name = _hongoNames[0];
                _hongoNames.RemoveAt(0);
                character.UpdateName(name);
            }

            else
            {
                _aiAliveCharacters.Add(character);
                string name = _sapoNames[0];
                _sapoNames.RemoveAt(0);
                character.UpdateName(name);
            }

            InGameUIEvents.OnAddCharacterPortrait?.Invoke(character);
        }
    }

    private void IntroductionSequenceFinished()
    {
        GameManagerEvents.OnIntroductionSequenceEnded -= IntroductionSequenceFinished;

# if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"Introduction sequence finished.");
# endif

        GameTurnEvents.OnCharactersListUpdate(_characters);
        AIManagerEvents.OnUpdateAICharacters(_aiAliveCharacters);
    }

    private void OnCharacterDeath(BaseCharacter character)
    {
        if (_playerAliveCharacters.Contains(character)) _playerAliveCharacters.Remove(character);
        else if (_aiAliveCharacters.Contains(character)) _aiAliveCharacters.Remove(character);

        if (_playerAliveCharacters.Count == 0 || _aiAliveCharacters.Count == 0)
        {
            if (_playerAliveCharacters.Count == 0) Debug.Log("Player Lost.");
            else Debug.Log("Player Wins.");

            GameTurnEvents.OnGameEnded?.Invoke();
        }
    }

    public static BaseCharacter GetRandomPlayerCharacterAlive()
    {
        List<BaseCharacter> aliveCharacters = new List<BaseCharacter>();

        foreach (BaseCharacter item in _playerAliveCharacters)
        {
            if (item.IsDead) continue;
            aliveCharacters.Add(item);
        }

        return aliveCharacters[Random.Range(0, aliveCharacters.Count)];
    }
}
