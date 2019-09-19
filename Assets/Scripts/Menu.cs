using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Combat _combatLogic;
    [SerializeField] Text _turnText;
    [SerializeField] Canvas _combatMenu;
    [SerializeField] EventSystem _eventSystem;

    [SerializeField] Button _moveButton;
    [SerializeField] Button _attackButton;
    [SerializeField] Button _guardButton;

    public string setTurnText { set { _turnText.GetComponent<Text>().text = value; } }

    #region Boolians
    public bool menuOpen { get; private set; }
    public static bool stateChanged { get; set; }
    public bool playerIsChoosing { get; set; }
    public bool targetChoosed { get; set; }
    #endregion

    public event Action OnMenuOpenedEvent;
    public event Action OnMoveCharacterEvent;
    //public event Action OnAttackPressedEvent;
    public event Action OnGuardPressedEvent;

    public bool moveRoutine { get; private set; }
    public bool attackRoutine { get; private set; }
    public bool guardRoutine { get; private set; }

    private void OnMoveClicked()
    {
        if (!GameManager.Instance._characterClicked.movedThisTurn)
        {
            moveRoutine = true;
            ToggleMenu(false);
            StartCoroutine(WaitUntilChosen());
        }
    }
    private void OnAttackClicked()
    {
        if (!GameManager.Instance._characterClicked.attackedThisTurn)
        {
            attackRoutine = true;
            GameManager.Instance._characterClicked.showPossibleMove(true);
            ToggleMenu(false);
            StartCoroutine(WaitUntilChosen());
            //OnAttackPressedEvent?.Invoke();
        }
    }
    private void OnGuardClicked()
    {
        guardRoutine = true;
    }

    private void RoutinesReset()
    {
        moveRoutine = false;
        attackRoutine = false;
        guardRoutine = false;
    }

    protected void Awake()
    {
        _moveButton.onClick.AddListener(OnMoveClicked);
        _attackButton.onClick.AddListener(OnAttackClicked);
        _guardButton.onClick.AddListener(OnGuardClicked);

        ToggleMenu(false);
        targetChoosed = false;
        stateChanged = false; // Should be event
        RoutinesReset();
    }

    protected void FixedUpdate()
    {
        TurnText();
    }

    public void MenuController()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !menuOpen && !playerIsChoosing)
        {
            if (GameManager.Instance.IsCharacterHere())
            {
                GameManager.Instance._characterClicked = GameManager.Instance._characterDictionary[GameManager.Instance._whereClicked];
                if (IsMyTurn())
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
        {
            playerIsChoosing = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && menuOpen)
        {
            ToggleMenu(false);
        }
    }

    private bool IsMyTurn()
    {
        if (GameManager.Instance._characterClicked && GameManager.Instance._characterClicked.isRed == GameManager.Instance.IsRedTurn)
            return true;
        else
            return false;
    }

    private void TurnText()
    {
        if(!GameManager.Instance.GameOver)
        {
            if (GameManager.Instance.IsRedTurn)
                _turnText.GetComponent<Text>().text = "Red Turn";
            else
                _turnText.GetComponent<Text>().text = "Blue Turn";
        }
        else
        {
            if (GameManager.Instance.IsRedTurn)
                _turnText.GetComponent<Text>().text = "Red Wins!";
            else
                _turnText.GetComponent<Text>().text = "Blue Wins!";
        }
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
                OnMoveCharacterEvent?.Invoke();
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
            if (GameManager.Instance.IsCharacterHere())
            {
                var character = GameManager.Instance._characterDictionary[GameManager.Instance._whereClicked];
                GameManager.Instance._characterEnemyClicked = character;
                _combatLogic.attackEnemy(GameManager.Instance._characterClicked, GameManager.Instance._characterEnemyClicked);
                GameManager.Instance._characterClicked.attackedThisTurn = true;
            }
            else
            {
                GameManager.Instance.InvalidCommand = true;
            }

            GameManager.Instance._characterClicked.showPossibleMove(false);
            GameManager.Instance._characterClicked = null;
            GameManager.Instance._characterEnemyClicked.showPossibleMove(false);
            GameManager.Instance._characterEnemyClicked = null;

            stateChanged = true;
        }

        playerIsChoosing = false;
        ToggleMenu(false);
        RoutinesReset();
    }
}