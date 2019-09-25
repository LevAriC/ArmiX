using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] GUI _mainMenu;
    //private Character.CharacterColors _currentColor;
    private List<Vector2Int> _restrictedList;
    private Tile _currentTile;
    private int _restrictedListIndex;

    private int _posX, _posY;
    private int _maxX, _maxY;

    public static Cursor cursorInstance { get; private set; }
    public Vector2Int GetCoords { get { return new Vector2Int(_posX, _posY); } }

    protected void Awake()
    {
        cursorInstance = this;
        _restrictedList = null;
        _restrictedListIndex = -1;
    }

    protected void Start()
    {
        _maxX = GameManager.Instance.GetBoard.GetWidth - 1;
        _maxY = GameManager.Instance.GetBoard.GetHeight - 1;

        if(GameManager.Instance.IsPlayerOneTurn) 
        {
            _posX = 0;
            _posY = 0;
        }
        else
        {
            _posX = _maxX;
            _posY = _maxY;
        }

        MoveCursor(_posX, _posY);
    }

    protected void Update()
    {
        if(GameManager.Instance.GameStarted)
            CursorMovement();
    }

    public void MoveCursor(int x, int y)
    {
        _posX = x;
        _posY = y;
        _currentTile = GameManager.Instance.GetBoard.GetFromBoard(x, y);
        transform.position = _currentTile.transform.position;
    }

    private void DefaultMovement(int restrict = -1, int originX = -1, int originY = -1)
    {
        var curMaxX = _maxX;
        var curMaxY = _maxY;
        var curMinX = 0;
        var curMinY = 0;

        if (restrict != -1)
        {
            if (_maxX >= curMaxX) curMaxX = originX + restrict;
            if (_maxY >= curMaxY) curMaxY = originY + restrict;
            if (originX - restrict > 0) curMinX = originX - restrict;
            if (originY - restrict > 0) curMinY = originY - restrict;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
            if (_posY < curMaxY || _posY > curMinY)
                _posY += GameManager.Instance.IsPlayerOneTurn ? 1 : -1;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            if(_posY > curMinY || _posY < curMaxY)
                _posY += GameManager.Instance.IsPlayerOneTurn ? -1 : 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            if(_posX > curMinX || _posX < curMaxX)
                _posX += GameManager.Instance.IsPlayerOneTurn ? -1 : 1;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            if(_posX < curMaxX || _posX > curMinX)
                _posX += GameManager.Instance.IsPlayerOneTurn ? 1 : -1;

        if (_posX < curMinX) _posX = curMinX;
        if (_posX > curMaxX) _posX = curMaxX;
        if (_posY < curMinY) _posY = curMinY;
        if (_posY > curMaxY) _posY = curMaxY;
    }

    private void CursorMovement()
    {
        if (!_mainMenu.menuOpen && !_mainMenu.playerIsChoosing)
        {
            DefaultMovement();

            if (_restrictedList != null)
            {
                _restrictedList = null;
                _restrictedListIndex = -1;
            }
        }

        if (_mainMenu.playerIsChoosing)
        {
            if (_mainMenu.moveRoutine || _mainMenu.overwatchRoutine)
            {
                var charToMove = GameManager.Instance._characterClicked;
                var charPos = GameManager.Instance._whereClicked;
                if (_mainMenu.moveRoutine)
                    DefaultMovement(charToMove.getMovement, charPos.x, charPos.y);
                else
                    DefaultMovement(charToMove.getRange, charPos.x, charPos.y);
            }

            if (_mainMenu.attackRoutine)
            {
                if (_restrictedList == null)
                {
                    _restrictedList = new List<Vector2Int>(GameManager.Instance.GetAllEnemiesOrAllies());
                    _restrictedListIndex = 0;
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (_restrictedListIndex == 0)
                        _restrictedListIndex = _restrictedList.Count - 1;
                    else
                        _restrictedListIndex--;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (_restrictedListIndex == _restrictedList.Count - 1)
                        _restrictedListIndex = 0;
                    else
                        _restrictedListIndex++;
                }

                _posX = _restrictedList[_restrictedListIndex].x;
                _posY = _restrictedList[_restrictedListIndex].y;
            }
        }

        MoveCursor(_posX, _posY);
    }
}