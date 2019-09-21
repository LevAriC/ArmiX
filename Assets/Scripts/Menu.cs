﻿using System;
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
    [SerializeField] Button _overwatchButton;

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
    public event Action OnOverwatchEvent;

    public bool moveRoutine { get; private set; }
    public bool attackRoutine { get; private set; }
    public bool overwatchRoutine { get; private set; }

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
    private void OnOverwatchClicked()
    {
        if (!GameManager.Instance._characterClicked.overwatchedThisTurn)
        {
            overwatchRoutine = true;
            ToggleMenu(false);
            StartCoroutine(WaitUntilChosen());
        }
    }

    private void RoutinesReset()
    {
        moveRoutine = false;
        attackRoutine = false;
        overwatchRoutine = false;
    }

    protected void Awake()
    {
        _moveButton.onClick.AddListener(OnMoveClicked);
        _attackButton.onClick.AddListener(OnAttackClicked);
        _overwatchButton.onClick.AddListener(OnOverwatchClicked);

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
        if (GameManager.Instance._characterClicked && GameManager.Instance._characterClicked.myColor == GameManager.Instance.CurrentPlayer)
            return true;
        else
            return false;
    }

    private void TurnText()
    {
        if(!GameManager.Instance.GameOver)
        {
            if (GameManager.Instance.CurrentPlayer == Character.CharacterColors.Blue)
                _turnText.GetComponent<Text>().text = "Blue Turn";
            else
                _turnText.GetComponent<Text>().text = "Red Turn";
        }
        else
        {
            if (GameManager.Instance.CurrentPlayer == Character.CharacterColors.Blue)
                _turnText.GetComponent<Text>().text = "Blue Wins!";
            else
                _turnText.GetComponent<Text>().text = "Red Wins!";
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
            var attackerPos = GameManager.Instance._whereClicked;
            if (GameManager.Instance.IsCharacterHere())
            {
                var character = GameManager.Instance._characterDictionary[GameManager.Instance._whereClicked];
                GameManager.Instance._characterEnemyClicked = character;
                _combatLogic.attackEnemy(GameManager.Instance._characterClicked, GameManager.Instance._characterEnemyClicked);
                GameManager.Instance._characterClicked.attackedThisTurn = true;
                Cursor.cursorInstance.moveCursor(attackerPos.x, attackerPos.y);
            }
            else
            {
                GameManager.Instance.InvalidCommand = true;
                Cursor.cursorInstance.moveCursor(attackerPos.x, attackerPos.y);
            }

            GameManager.Instance._characterClicked.showPossibleMove(false);
            GameManager.Instance._characterClicked = null;
            GameManager.Instance._characterEnemyClicked.showPossibleMove(false);
            GameManager.Instance._characterEnemyClicked = null;

            stateChanged = true;
        }

        if (overwatchRoutine)
        {
            if (!GameManager.Instance.IsCharacterHere())
            {
                OnOverwatchEvent?.Invoke();
                GameManager.Instance._characterClicked.overwatchedThisTurn = true;
            }
            else
            {
                GameManager.Instance.InvalidCommand = true;
                GameManager.Instance._characterClicked.overwatchedThisTurn = false;
            }
        }

        playerIsChoosing = false;
        ToggleMenu(false);
        RoutinesReset();
    }
}