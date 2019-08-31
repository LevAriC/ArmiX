using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] Transform _tileContainer;

    [Header("References")]
    [SerializeField] Tile _testTile;

    [Header("Configuration")]
    [SerializeField] int _height;
    [SerializeField] int _width;

    [Header("Debug")]
    [SerializeField] List<Vector2> _occupiedTiles;

    public int getHeight { get { return _height; } }
    public int getWidth { get { return _width; } }

    private List<List<Tile>> _tilesOnBoard;

    protected void Awake()
    {
        _tilesOnBoard = new List<List<Tile>>();
        _occupiedTiles = new List<Vector2>();
        for (int i = 0; i < _height; i++)
        {
            _tilesOnBoard.Add(new List<Tile>());
        }
    }

    public void BoardInit(Transform parent)
    {
        for(int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                var newTile = Instantiate(_testTile);
                newTile.transform.Translate(new Vector3(parent.position.x + i, 0, parent.position.z + j));
                _tilesOnBoard[i].Add(newTile);

                newTile.transform.SetParent(_tileContainer);
            }
        }
    }

    public void setOnBoard(int xPos, int yPos, Character character)
    {
        _tilesOnBoard[xPos][yPos].setOnTile(character);
        _occupiedTiles.Add(new Vector2(xPos, yPos));
    }

    public void moveCharacter(int xPos, int yPos, Character character)
    {
        if (!(_occupiedTiles.Contains(new Vector2(xPos, yPos))))
        {
            setOnBoard(xPos, yPos, character);
        }
    }

    public Tile getFromBoard(int xPos, int yPos)
    {
        return _tilesOnBoard[xPos][yPos];
    }
}
