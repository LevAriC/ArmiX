using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private int posX, posY;
    private int maxX, maxY;
    private Tile currentTile;
    private List<Vector2Int> restrictedList;
    private int restrictedListIndex;
    [SerializeField] Menu _mainMenu;
    public static Cursor cursorInstance { get; private set; }
    private Character.CharacterColors _prevColor;


    public Vector2Int getCoords { get { return cursorPosition(); } }

    protected void Awake()
    {
        cursorInstance = this;
        restrictedList = null;
        restrictedListIndex = -1;
    }

    protected void Start()
    {
        _prevColor = GameManager.Instance.CurrentPlayer;

        maxX = GameManager.Instance.getBoard.getWidth - 1;
        maxY = GameManager.Instance.getBoard.getHeight - 1;

        if(_prevColor == Character.CharacterColors.Blue)
        {
            posX = 0;
            posY = 0;
        }
        else
        {
            posX = maxX;
            posY = maxY;
        }

        moveCursor(posX, posY);
    }

    protected void Update()
    {
        if (_prevColor != GameManager.Instance.CurrentPlayer)
            _prevColor = GameManager.Instance.CurrentPlayer;

        cursorMovement();
    }

    public void moveCursor(int x, int y)
    {
        currentTile = GameManager.Instance.getBoard.getFromBoard(x, y);
        transform.position = currentTile.transform.position;
    }

    private void DefaultMovement(int restrict = -1, int originX = -1, int originY = -1)
    {
        var curMaxX = maxX;
        var curMaxY = maxY;
        var curMinX = 0;
        var curMinY = 0;

        if (restrict != -1)
        {
            if (maxX >= curMaxX) curMaxX = originX + restrict;
            if (maxY >= curMaxY) curMaxY = originY + restrict;
            if (originX - restrict > 0) curMinX = originX - restrict;
            if (originY - restrict > 0) curMinY = originY - restrict;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && posY < curMaxY)
            posY += _prevColor == Character.CharacterColors.Blue ? 1 : -1;
        if (Input.GetKeyDown(KeyCode.DownArrow) && posY > curMinY)
            posY += _prevColor == Character.CharacterColors.Blue ? -1 : 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow) && posX > curMinX)
            posX += _prevColor == Character.CharacterColors.Blue ? -1 : 1;
        if (Input.GetKeyDown(KeyCode.RightArrow) && posX < curMaxX)
            posX += _prevColor == Character.CharacterColors.Blue ? 1 : -1;
    }

    private void cursorMovement()
    {
        if (!_mainMenu.menuOpen && !_mainMenu.playerIsChoosing)
        {
            DefaultMovement();

            if (restrictedList != null)
            {
                restrictedList = null;
                restrictedListIndex = -1;
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
                if (restrictedList == null)
                {
                    restrictedList = new List<Vector2Int>(GameManager.Instance.GetAllEnemiesOrAllies());
                    restrictedListIndex = 0;
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (restrictedListIndex == 0)
                        restrictedListIndex = restrictedList.Count - 1;
                    else
                        restrictedListIndex--;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (restrictedListIndex == restrictedList.Count - 1)
                        restrictedListIndex = 0;
                    else
                        restrictedListIndex++;
                }

                posX = restrictedList[restrictedListIndex].x;
                posY = restrictedList[restrictedListIndex].y;
            }
        }

        moveCursor(posX, posY);
    }

    //private IEnumerator PlayerChoosingTarget()
    //{
    //    Debug.Log("Haleluja?");
    //    if (restrictedList == null)
    //    {
    //        restrictedList = new List<Vector2Int>(GameManager.Instance.GetAllEnemiesOrAllies());
    //        restrictedListIndex = 0;
    //    }
    //    //while (_mainMenu.playerIsChoosing)
    //    //{
    //    //    if (Input.GetKeyDown(KeyCode.LeftArrow))
    //    //    {
    //    //        restrictedListIndex--;
    //    //        break;
    //    //    }
    //    //    if (Input.GetKeyDown(KeyCode.RightArrow))
    //    //    {
    //    //        restrictedListIndex++;
    //    //        break;
    //    //    }

    //    //    restrictedListIndex %= restrictedList.Count - 1;
    //    //    posX = restrictedList[restrictedListIndex].x;
    //    //    posY = restrictedList[restrictedListIndex].y;

    //    //    moveCursor(posX, posY);
    //    //}
    //        yield return null;

    //    //_mainMenu.playerIsChoosing = false;
    //}

    private Vector2Int cursorPosition()
    {
        return new Vector2Int(posX, posY);
    }
}