using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] Combat _combatLogic;

    #region Click Detectors
    private Vector2Int _whereClicked;
    private Character _characterClicked;
    private Character _characterEnemyClicked;
    #endregion

    #region Boolians
    private bool _selectingTarget;
    private bool _menuOpen;
    public static bool stateChanged { get; set; }
    #endregion

    protected void Awake()
    {
        _menuOpen = false;
        _selectingTarget = false;
        stateChanged = false; // Should be event
    }

    public void menuController()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_menuOpen && !_selectingTarget)
        {
            _whereClicked = Cursor.cursorInstance.getCoords;
            _characterClicked = GameManager.Instance._characterDictionary[_whereClicked];
            if (GameManager.Instance._characterDictionary.ContainsKey(_whereClicked))
            {
                _menuOpen = true;
                _characterClicked.showPossibleMove(_menuOpen);
                _selectingTarget = true;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && _menuOpen && _selectingTarget)
        {
            _whereClicked = Cursor.cursorInstance.getCoords;
            _characterEnemyClicked = GameManager.Instance._characterDictionary[_whereClicked];
            if (GameManager.Instance._characterDictionary.ContainsKey(_whereClicked))
            {
                _combatLogic.attackEnemy(_characterClicked, _characterEnemyClicked);
                stateChanged = true;
                _menuOpen = false;
                _selectingTarget = false;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _menuOpen)
        {
            if (GameManager.Instance._characterDictionary.ContainsKey(_whereClicked))
            {
                _menuOpen = false;
                _characterClicked.showPossibleMove(_menuOpen);
                _selectingTarget = false;
                _characterClicked = null;
            }
        }
    }
}
