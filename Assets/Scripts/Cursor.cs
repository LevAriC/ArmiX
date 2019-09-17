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

    public Vector2Int getCoords { get { return cursorPosition(); } }

    protected void Awake()
    {
        cursorInstance = this;
        restrictedList = null;
        restrictedListIndex = -1;
        posX = 0;
        posY = 0;
    }

    protected void Start()
    {
        //_mainMenu.OnAttackPressedEvent += () => StartCoroutine(PlayerChoosingTarget());
        maxX = GameManager.Instance.getBoard.getWidth - 1;
        maxY = GameManager.Instance.getBoard.getHeight - 1;
        moveCursor(posX, posY);
    }

    protected void Update()
    {
        cursorMovement();
    }

    public void moveCursor(int x, int y)
    {
        currentTile = GameManager.Instance.getBoard.getFromBoard(x, y);
        transform.position = currentTile.transform.position;
    }

    private void cursorMovement()
    {
        if (!_mainMenu.menuOpen && !_mainMenu.playerIsChoosing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && posY < maxY)
                posY += 1;
            if (Input.GetKeyDown(KeyCode.DownArrow) && posY > 0)
                posY -= 1;
            if (Input.GetKeyDown(KeyCode.LeftArrow) && posX > 0)
                posX -= 1;
            if (Input.GetKeyDown(KeyCode.RightArrow) && posX < maxX)
                posX += 1;

            if (restrictedList != null)
            {
                restrictedList = null;
                restrictedListIndex = -1;
            }
        }

        if (_mainMenu.playerIsChoosing)
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