using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class TestAIStates : MonoBehaviour
{
    [Header("References")]
    private ScrollRect _scroll;
    public TextMeshProUGUI _content;

    public static Action<string> OnUpdateAIDebug;

    private void Awake()
    {
        _scroll = GetComponent<ScrollRect>();
        OnUpdateAIDebug += AddAIStateDebugText;
    }

    private void OnDestroy()
    {
        OnUpdateAIDebug -= AddAIStateDebugText;
    }

    public void OnTextUpdate(Vector2 pos)
    {

    }

    private void AddAIStateDebugText(string newLine)
    {
        _content.text += newLine + "\n";
        StartCoroutine(WaitToScroll());
    }

    IEnumerator WaitToScroll()
    {
        yield return new WaitForEndOfFrame();
        _scroll.verticalNormalizedPosition = 0;
    }
}
