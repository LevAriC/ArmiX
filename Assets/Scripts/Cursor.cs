using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private int posX, posY;
    private int maxX, maxY;
    private Tile currentTile;

    public static Cursor cursorInstance { get; private set; }

    public Vector2Int getCoords { get { return cursorPosition(); } }

    protected void Awake()
    {
        cursorInstance = this;
        posX = 0;
        posY = 0;
    }

    protected void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.UpArrow) && posY < maxY)
        {
            posY += 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && posY > 0)
        {
            posY -= 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && posX > 0)
        {
            posX -= 1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && posX < maxX)
        {
            posX += 1;
        }

        moveCursor(posX, posY);
    }

    private Vector2Int cursorPosition()
    {
        return new Vector2Int(posX, posY);
    }
}