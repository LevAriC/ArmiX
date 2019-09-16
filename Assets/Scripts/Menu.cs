using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Combat _combatLogic;
    [SerializeField] Text _turnText;
    [SerializeField] Canvas _combatMenu;

    public string setTurnText { set { _turnText.GetComponent<Text>().text = value; } }

    #region Click Detectors
    private Vector2Int _whereClicked;
    private Character _characterClicked;
    private Character _characterEnemyClicked;
    #endregion

    #region Boolians
    //private bool _menuOpen;
    public static bool stateChanged { get; set; }
    #endregion

    protected void Awake()
    {
        //_menuOpen = false;
        //_combatMenu.gameObject.SetActive(false);
        stateChanged = false; // Should be event
    }

    protected void FixedUpdate()
    {
        TurnText();
    }

    public void menuController()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsCharacterHere())
            {
                if (!_combatMenu.gameObject.activeInHierarchy)
                {
                    _characterClicked = GameManager.Instance._characterDictionary[_whereClicked];
                    if (IsMyTurn())
                    {
                        _combatMenu.gameObject.SetActive(true);
                        _characterClicked.showPossibleMove(_combatMenu.gameObject.activeInHierarchy);
                    }
                    else
                    {
                        _characterClicked = null;
                    }
                }
                else
                {
                    var character = GameManager.Instance._characterDictionary[_whereClicked];
                    if (_characterClicked && _characterClicked.getCharacterID == character.getCharacterID)
                    {
                        CloseMenu();
                        return;
                    }
                    _characterEnemyClicked = character;
                    _combatLogic.attackEnemy(_characterClicked, _characterEnemyClicked);
                    stateChanged = true;
                    _combatMenu.gameObject.SetActive(false);
                    CloseMenu();
                }
            }

            else
            {
                CloseMenu();
                return;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Escape) && _menuOpen)
        //{
        //    CloseMenu();
        //}
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

    private void CloseMenu()
    {
        //_menuOpen = false;
        //if (_characterClicked)
        //{
        //    _characterClicked.showPossibleMove(_menuOpen);
        //    _characterClicked = null;
        //}
        //if (_characterEnemyClicked)
        //{
        //    _characterEnemyClicked.showPossibleMove(_menuOpen);
        //    _characterEnemyClicked = null;
        //}
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
}