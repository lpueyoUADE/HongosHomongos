using TMPro;
using UnityEngine;

public class TestAIStatsWidget : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Vector3 offset = new(0, 0, 10);

    public void UpdateStats(AICharacterConfig inputData)
    {
        string widgetText = $"chances idle = {inputData.ChancesIdle}\n";
        widgetText += $"chances swap = {inputData.ChancesSwapTarget}\n";
        widgetText += $"chances move = {inputData.ChancesMove}\n";
        widgetText += $"chances aim = {inputData.ChancesAim}\n";

        text.text = widgetText;
    }

    private void FixedUpdate() 
    {
        transform.rotation = new Quaternion(0, 0, 0, 90);
        transform.position = transform.parent.position + offset;
    }
}
