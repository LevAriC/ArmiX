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

    #region Click Detectors
    private Vector2Int _whereClicked;
    private Character _characterClicked;
    private Character _characterEnemyClicked;
    #endregion

    #region Boolians
    public bool menuOpen { get; private set; }
    public static bool stateChanged { get; set; }
    public bool playerIsChoosing { get; set; }
    public bool targetChoosed { get; set; }
    #endregion

    //public event Action OnMenuOpenedEvent;
    public event Action OnMovePressedEvent;
    //public event Action OnAttackPressedEvent;
    public event Action OnGuardPressedEvent;

    private void OnMoveClicked() { }
    private void OnAttackClicked()
    {
        _characterClicked.showPossibleMove(true);
        ToggleMenu(false);
        //OnAttackPressedEvent?.Invoke();
        StartCoroutine(WaitUntilChosen());
    }
    private void OnGuardClicked() { }

    protected void Awake()
    {
        _moveButton.onClick.AddListener(OnMoveClicked);
        _attackButton.onClick.AddListener(OnAttackClicked);
        _guardButton.onClick.AddListener(OnGuardClicked);

        ToggleMenu(false);
        playerIsChoosing = false;
        targetChoosed = false;
        stateChanged = false; // Should be event
    }

    protected void FixedUpdate()
    {
        TurnText();
    }

    public void MenuController()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !menuOpen && !playerIsChoosing)
        {
            if (IsCharacterHere())
            {
                _characterClicked = GameManager.Instance._characterDictionary[_whereClicked];
                if (IsMyTurn())
                {
                    ToggleMenu(true);
                }
                else
                {
                    _characterClicked = null;
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

    private bool IsCharacterHere()
    {
        _whereClicked = Cursor.cursorInstance.getCoords;
        bool isCharHere = GameManager.Instance._characterDictionary.ContainsKey(_whereClicked)? true : false;
        return isCharHere;
    }

    private bool IsMyTurn()
    {
        if (_characterClicked && _characterClicked.isRed == GameManager.Instance.IsRedTurn)
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
        Debug.Log("Menu is " + menuOpen);
    }

    public void MoveCharacter()
    {

    }

    private IEnumerator WaitUntilChosen()
    {
        playerIsChoosing = true;
        while (playerIsChoosing)
        {
            yield return null;
        }

        if (IsCharacterHere())
        {
            var character = GameManager.Instance._characterDictionary[_whereClicked];
            _characterEnemyClicked = character;
            _combatLogic.attackEnemy(_characterClicked, _characterEnemyClicked);
        }
        stateChanged = true;
        _characterClicked.showPossibleMove(false);
        _characterClicked = null;
        _characterEnemyClicked.showPossibleMove(false);
        _characterEnemyClicked = null;
        playerIsChoosing = false;
        ToggleMenu(false);
    }
}