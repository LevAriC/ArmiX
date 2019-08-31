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

    private List<List<Tile>> _tilesOnBoard;


    protected void Awake()
    {
        _tilesOnBoard = new List<List<Tile>>();
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
                newTile.transform.parent = parent;
                newTile.transform.Translate(new Vector3(parent.position.x + i, 0, parent.position.z + j));

                _tilesOnBoard[i].Add(newTile);
                newTile.transform.SetParent(_tileContainer);
            }
        }
    }

    public void placeOnBoard(int xPos, int yPos, Character character)
    {
        _tilesOnBoard[xPos][yPos].setOnTile(character);
    }
}
