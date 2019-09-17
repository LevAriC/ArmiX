using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Combat _combatLogic;
    [SerializeField] Text _turnText;
    [SerializeField] Canvas _combatMenu;

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
    #endregion

    //public event Action OnMenuOpenedEvent;
    public event Action OnMovePressedEvent;
    public event Action OnAttackPressedEvent;
    public event Action OnGuardPressedEvent;

    private void OnMoveClicked() { }
    private void OnAttackClicked()
    {
        OnAttackPressedEvent?.Invoke();
    }
    private void OnGuardClicked() { }

    protected void Awake()
    {
        _moveButton.onClick.AddListener(OnMoveClicked);
        _attackButton.onClick.AddListener(OnAttackClicked);
        _guardButton.onClick.AddListener(OnGuardClicked);

        ToggleMenu(false);
        playerIsChoosing = false;
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
                    //_combatMenu.gameObject.SetActive(true);
                    //AttackCharacter();
                }
                else
                {
                    _characterClicked = null;
                }
            }
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
        {
            
        }
        else
        {

            if (_characterClicked)
            {
                _characterClicked.showPossibleMove(open);
                _characterClicked = null;
            }
            if (_characterEnemyClicked)
            {
                _characterEnemyClicked.showPossibleMove(open);
                _characterEnemyClicked = null;
            }
        }

        menuOpen = _combatMenu.gameObject.activeInHierarchy;
    }

    public void MoveCharacter()
    {

    }
    public void AttackCharacter()
    {
        _characterClicked.showPossibleMove(true);
        playerIsChoosing = true;

        //_whereClicked = Cursor.cursorInstance.PlayerIsChoosingTarget();

        var character = GameManager.Instance._characterDictionary[_whereClicked];
        //if (_characterClicked && _characterClicked.getCharacterID == character.getCharacterID)
        //{
        //    ToggleMenu(false);
        //    return;
        //}

        _characterEnemyClicked = character;
        _combatLogic.attackEnemy(_characterClicked, _characterEnemyClicked);
        stateChanged = true;
        //_combatMenu.gameObject.SetActive(false);
        ToggleMenu(false);
    }
}