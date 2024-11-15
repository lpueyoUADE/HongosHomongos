using System;
using UnityEngine;

public class GameManagerEvents : MonoBehaviour
{
    private static GameModeGeneralSettings _modeSettings;
    public static GameModeGeneralSettings ModeSettings => _modeSettings;

    public static Action<BaseCharacter> OnCharacterDeath;
    public static Action OnIntroductionSequenceEnded;


    public static UserConfigData UserConfig;

    public static void UpdateModeSettings(GameModeGeneralSettings newSettings)
    {
        _modeSettings = newSettings;
    }

    public static void UpdateUserConfigs(UserConfigData newUserConfig)
    {
        UserConfig = newUserConfig;
    }
}
