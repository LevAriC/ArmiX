using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public enum CharacterTypes { MachineGun, Sniper, Agent };

    [SerializeField] Combat _combatLogic;
    [SerializeField] Text _turnText;
    [SerializeField] Text _popUpText;
    [SerializeField] Canvas _combatMenu;
    [SerializeField] EventSystem _eventSystem;

    [SerializeField] Button _moveButton;
    [SerializeField] Button _attackButton;
    [SerializeField] Button _overwatchButton;

    public string setTurnText { set { _turnText.GetComponent<Text>().text = value; } }
    public Combat getCombatLogic { get { return _combatLogic; } }

    #region Boolians
    public bool menuOpen { get; private set; }
    public static bool stateChanged { get; set; }
    public bool playerIsChoosing { get; set; }
    public bool targetChoosed { get; set; }
    #endregion

    #region Actions
    public event Action MenuOpenedEvent;
    public event Action MoveCharacterEvent;
    public event Action AttackPressedEvent;
    public event Action TargetAcquiredEvent;
    public event Action OverwatchEvent;

    public bool moveRoutine { get; private set; }
    public bool attackRoutine { get; set; }
    public bool overwatchRoutine { get; private set; }
    #endregion


    public static GUI menuInstance { get; private set; }

    public void RunPopup(string text)
    {
        StartCoroutine(FadePopUp(text));
    }

    private IEnumerator FadePopUp(string text)
    {
        float duration = 0.5f;
        _popUpText.canvasRenderer.SetAlpha(0.00f);
        _popUpText.GetComponent<Text>().text = text;
        _popUpText.CrossFadeAlpha(1.0f, duration, false);
        yield return new WaitForSeconds(duration * 2);
        _popUpText.CrossFadeAlpha(0.0f, duration, false);
    }

    private void OnMoveClicked()
    {
        if(!GameManager.Instance._characterClicked.overwatchedThisTurn)
        {
            if (!GameManager.Instance._characterClicked.movedThisTurn)
            {
                moveRoutine = true;
                ToggleMenu(false);
                GameManager.Instance.GetBoard.ToggleArea(GameManager.Instance._whereClicked.x, GameManager.Instance._whereClicked.y, GameManager.Instance._characterClicked.getMovement, true);
                StartCoroutine(WaitUntilChosen());
            }
            else
                RunPopup("Already Moved This Turn");
        }
        else
            RunPopup("Cannot Move While Overwatching");
    }
    private void OnAttackClicked()
    {
        if (!GameManager.Instance._characterClicked.overwatchedThisTurn)
        {
            if (!GameManager.Instance._characterClicked.attackedThisTurn)
            {
                attackRoutine = true;
                ToggleMenu(false);
                StartCoroutine(WaitUntilChosen());
                AttackPressedEvent?.Invoke();
            }
            else
                RunPopup("Already Attacked This Turn");
        }
        else
            RunPopup("Cannot Attack While Overwatching");
    }
    private void OnOverwatchClicked()
    {
        if (!GameManager.Instance._characterClicked.attackedThisTurn)
        {
            if (!GameManager.Instance._characterClicked.overwatchedThisTurn)
            {
                overwatchRoutine = true;
                ToggleMenu(false);
                StartCoroutine(WaitUntilChosen());
            }
            else
                RunPopup("Already Overwatching This Turn");
        }
        else
            RunPopup("Cannot Overwatch After Attacking");
    }

    private void RoutinesReset()
    {
        moveRoutine = false;
        attackRoutine = false;
        overwatchRoutine = false;
    }

    protected void Awake()
    {
        menuInstance = this;
        _moveButton.onClick.AddListener(OnMoveClicked);
        _attackButton.onClick.AddListener(OnAttackClicked);
        _overwatchButton.onClick.AddListener(OnOverwatchClicked);

        ToggleMenu(false);
        targetChoosed = false;
        stateChanged = false;
        RoutinesReset();
    }

    protected void FixedUpdate()
    {
        if(GameManager.Instance.GameStarted || GameManager.Instance.UserId == null && !GameManager.Instance.GameStarted)
            TurnText();
    }

    public void GameController()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !menuOpen && !playerIsChoosing)
        {
            if (GameManager.Instance.IsCharacterHere())
            {
                foreach (var alive in GameManager.Instance._characterDictionary)
                {
                    if (alive.Value == GameManager.Instance._whereClicked)
                        GameManager.Instance._characterClicked = alive.Key;
                }

                if (GameManager.Instance.IsMyTurn())
                {
                    ToggleMenu(true);
                }
                else
                {
                    GameManager.Instance._characterClicked = null;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !menuOpen && playerIsChoosing)
            playerIsChoosing = false;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AbortChoice();
            if (GameManager.Instance._characterClicked != null)
                Cursor.cursorInstance.MoveCursor(GameManager.Instance._whereClicked.x, GameManager.Instance._whereClicked.y);
        }

        if (Input.GetKeyDown(KeyCode.T))
            GameManager.Instance.EndTurn();
    }

    private void TurnText()
    {
        if(!GameManager.Instance.GameOver)
            _turnText.GetComponent<Text>().text = GameManager.Instance.WhosTurn.ToString() + " Turn";
        else
            _turnText.GetComponent<Text>().text = GameManager.Instance.WhosTurn.ToString() + " Won!!";
    }

    private void ToggleMenu(bool open)
    {
        _combatMenu.gameObject.SetActive(open);
        if (open)
            _eventSystem.SetSelectedGameObject(_moveButton.gameObject);
        menuOpen = open;
    }

    private IEnumerator WaitUntilChosen()
    {
        playerIsChoosing = true;
        while (playerIsChoosing)
        {
            yield return null;
        }

        if (moveRoutine)
        {
            if (!GameManager.Instance.IsCharacterHere())
            {
                MoveCharacterEvent?.Invoke();
                GameManager.Instance._characterClicked.movedThisTurn = true;
            }
            else
            {
                GameManager.Instance.InvalidCommand = true;
                GameManager.Instance._characterClicked.movedThisTurn = false;
            }
        }

        if (attackRoutine)
        {
            Character character = null;
            var attackerPos = GameManager.Instance._whereClicked;
            if (GameManager.Instance.IsCharacterHere())
            {
                foreach (var alive in GameManager.Instance._characterDictionary)
                {
                    if (alive.Value == GameManager.Instance._whereClicked)
                        character = alive.Key;
                }

                var defenderPos = GameManager.Instance._whereClicked;
                int distance = (int)Math.Floor(Math.Sqrt(Math.Abs(attackerPos.x - defenderPos.x)) + Math.Sqrt(Math.Abs(attackerPos.y - defenderPos.y)));
                if(character)
                    GameManager.Instance._characterEnemyClicked = character;
                var didHit = _combatLogic.AttackEnemy(GameManager.Instance._characterClicked, GameManager.Instance._characterEnemyClicked, distance);
                if(didHit)
                {
                    GameManager.Instance._characterClicked.myAnimator.SetTrigger("isTargetAcquired");
                    GameManager.Instance._characterEnemyClicked.myAnimator.SetTrigger("isHit");
                    GameManager.Instance._characterClicked.attackedThisTurn = true;
                }
                else
                    RunPopup("Target Missed!!");

                Cursor.cursorInstance.MoveCursor(attackerPos.x, attackerPos.y);
            }
            else
            {
                GameManager.Instance.InvalidCommand = true;
                Cursor.cursorInstance.MoveCursor(attackerPos.x, attackerPos.y);
            }

            GameManager.Instance._characterClicked = null;
            GameManager.Instance._characterEnemyClicked = null;

            stateChanged = true;
        }

        if (overwatchRoutine)
        {
            if (!GameManager.Instance.IsCharacterHere())
            {
                OverwatchEvent?.Invoke();
                GameManager.Instance._characterClicked.overwatchedThisTurn = true;
            }
            else
            {
                GameManager.Instance.InvalidCommand = true;
                GameManager.Instance._characterClicked.overwatchedThisTurn = false;
            }
        }

        AbortChoice();
    }

    private void AbortChoice()
    {
        playerIsChoosing = false;
        ToggleMenu(false);
        RoutinesReset();
    }
}