using UnityEngine;

public class BaseSapo : BaseCharacter, ISapo
{
# if UNITY_EDITOR
    private AICharacterConfig AIData => (AICharacterConfig)CharacterData;
    private TestAIStatsWidget widget;
# endif

    private void Start()
    {
        transform.rotation = new Quaternion(0, 90, 0, 0);

# if UNITY_EDITOR
        widget = Instantiate(AIData.developerStatsPrefab);
        widget.UpdateStats(AIData);
        widget.transform.SetParent(transform, false);
# endif
    }
}
