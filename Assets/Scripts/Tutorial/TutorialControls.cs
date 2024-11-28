using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialControls : MonoBehaviour
{
    public TutorialPlayerControls playerTut;
    public SapoTutorial sapo;
    public GameObject turnManager;
    public AudioClip nextStepSound;
    public AudioClip endStepSound;
    public int currentStepIndex = 0;
    public float timeBeforeNextStep = 3.5f;
    
    [Header("UI ref")]
    public GameObject info;

    public GameObject timerHelpStart;
    public GameObject timerHelpEnd;

    [Header("Movement")]
    public GameObject movementKeys;
    public TextMeshProUGUI moveA;
    public TextMeshProUGUI moveD;

    [Header("Jump")]
    public GameObject jumpKeys;
    public GameObject worldJumpKeys;
    public TextMeshProUGUI jumpSpace;
    public TextMeshProUGUI jumpRMouse;

    [Header("Aim")]
    public GameObject aimKey;
    public GameObject enemyRef;
    public TextMeshProUGUI aimKeyCtrl;
    public TextMeshProUGUI aimKeyLMouse;

    [Header("Ability")]
    public GameObject changeAbilityKeys;
    public GameObject abilityUIText;

    public TextMeshProUGUI ability1;
    public TextMeshProUGUI ability2;
    public TextMeshProUGUI ability3;

    [Header("Free look")]
    public GameObject freeLookKeys;
    public TextMeshProUGUI freeLook;

    public static Action<string> OnKeyPressed;
    public static Action OnJumpReached;
    public static Action OnEnemyDamaged;

    private void Awake() 
    {
        OnKeyPressed += KeyPressed;
        OnJumpReached += JumpPointReached;
        OnEnemyDamaged += EnemyDamaged;

        GameTurnEvents.OnTurnStart += ShowTimeLeft;
        GameTurnEvents.OnTurnEndManager += ShowNextTurnHelp;
    }

    private void OnDestroy() 
    {
        OnKeyPressed -= KeyPressed;
        OnJumpReached -= JumpPointReached;
        OnEnemyDamaged -= EnemyDamaged;

        GameTurnEvents.OnTurnStart -= ShowTimeLeft;
        GameTurnEvents.OnTurnEndManager -= ShowNextTurnHelp;
    }

    private void NextStep()
    {
        switch (currentStepIndex)
        {
            case 0:
                info.SetActive(true);
                playerTut.tutCanMove = true;
                movementKeys.SetActive(true);
                break;
            case 1:
                jumpKeys.SetActive(true);
                worldJumpKeys.SetActive(true);
                playerTut.tutCanJump = true;
                break;
            case 2:
                timerHelpStart.SetActive(false);
                timerHelpEnd.SetActive(false);
                enemyRef.SetActive(true);
                aimKey.SetActive(true);
                playerTut.tutCanUseChargeBar = true;
                break;
            case 3:
                changeAbilityKeys.SetActive(true);
                playerTut.tutCanChangeAbility = true;
                break;
            case 4:
                freeLookKeys.SetActive(true);
                playerTut.tutCanUseFreeLook = true;
                break;
            case 5:
                sapo.canDie = true;
                Destroy(this.gameObject);
                break;
        }

        currentStepIndex++;
    }

    public void KeyPressed(string key)
    {
        switch (currentStepIndex)
        {
            case 1:
                if (!movementKeys.activeSelf) return;

                if (key == "a" || key == "d")
                {
                    if (key == "a" && moveA.color != Color.green)
                    {
                        moveA.color = Color.green;
                        InGameUIEvents.OnPlayUISound(nextStepSound);
                    }

                    if (key == "d" && moveD.color != Color.green)
                    {
                        moveD.color = Color.green;
                        InGameUIEvents.OnPlayUISound(nextStepSound);
                    }
                }

                if (moveA.color == Color.green && moveD.color == Color.green)
                {
                    movementKeys.SetActive(false);
                    StartCoroutine(StartNextTurnTimer());
                }
                break;

            case 4:
                if (!changeAbilityKeys.activeSelf) return;

                if (key == "ab1" || key == "ab2" || key == "ab3")
                {
                    if (key == "ab1" && ability1.color != Color.green)
                    {
                        ability1.color = Color.green;
                        InGameUIEvents.OnPlayUISound(nextStepSound);
                    }
                    if (key == "ab2" && ability2.color != Color.green)
                    {
                        ability2.color = Color.green;
                        InGameUIEvents.OnPlayUISound(nextStepSound);
                    }
                    if (key == "ab3" && ability3.color != Color.green)
                    {
                        ability3.color = Color.green;
                        InGameUIEvents.OnPlayUISound(nextStepSound);
                    }     
                }

                if (ability1.color == Color.green && ability2.color == Color.green && ability3.color == Color.green)
                {
                    abilityUIText.SetActive(false);
                    changeAbilityKeys.SetActive(false);
                    StartCoroutine(StartNextTurnTimer());
                }
                break;

            case 5:
                if (!freeLookKeys.activeSelf) return;

                if (key == "freelook")
                {
                    freeLook.color = Color.green;
                    InGameUIEvents.OnPlayUISound(nextStepSound);
                    OnKeyPressed -= KeyPressed;
                    StartCoroutine(StartNextTurnTimer());
                }
                break;
        }
    }

    public void ShowTimeLeft()
    {
        GameTurnEvents.OnTurnStart -= ShowTimeLeft;
        timerHelpStart.SetActive(true);
    }

    public void ShowNextTurnHelp()
    {
        GameTurnEvents.OnTurnEndManager -= ShowNextTurnHelp;
        timerHelpEnd.SetActive(true);
        StartCoroutine(StartNextTurnTimer());
    }

    public void JumpPointReached()
    {
        OnJumpReached -= JumpPointReached;
        jumpSpace.color = Color.green;
        jumpRMouse.color = Color.green;
        InGameUIEvents.OnPlayUISound(nextStepSound);

        jumpKeys.SetActive(false);
        worldJumpKeys.SetActive(false);
        StartCoroutine(StartNextTurnTimer());
    }

    public void EnemyDamaged()
    {
        aimKey.SetActive(false);
        abilityUIText.SetActive(true);
        OnEnemyDamaged -= EnemyDamaged;
        StartCoroutine(StartNextTurnTimer());
    }

    IEnumerator StartNextTurnTimer()
    {
        yield return new WaitForSeconds(timeBeforeNextStep);
        InGameUIEvents.OnPlayUISound(endStepSound);
        NextStep();
    }
}
