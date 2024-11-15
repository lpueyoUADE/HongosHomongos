using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class TestDebugBox : MonoBehaviour
{
    [Header("References")]
    private ScrollRect _scroll;
    public TextMeshProUGUI _content;

    public static Action<string> OnUpdateDebugBoxText;

    private void Awake()
    {
        _scroll = GetComponent<ScrollRect>();
        OnUpdateDebugBoxText += AddDebugText;
    }

    private void OnDestroy()
    {
        OnUpdateDebugBoxText -= AddDebugText;
    }

    public void OnTextUpdate(Vector2 pos)
    {

    }

    private void AddDebugText(string newLine)
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
