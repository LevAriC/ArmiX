using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
    [SerializeField] Transform _tileContainer;

    [Header("References")]
    [SerializeField] Tile _testTile;

    [Header("Configuration")]
    [SerializeField] int _height;
    [SerializeField] int _width;

    public int GetHeight { get { return _height; } }
    public int GetWidth { get { return _width; } }

    private List<List<Tile>> _tilesOnBoard;

    protected void Awake()
    {
        _tilesOnBoard = new List<List<Tile>>();
        for (int i = 0; i < _height; i++)
        {
            _tilesOnBoard.Add(new List<Tile>());
        }
    }

    public void SurfaceInit(Transform parent)
    {
        for(int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                var newTile = Instantiate(_testTile);
                newTile.transform.Translate(new Vector3(parent.position.x + i, parent.position.y, parent.position.z + j));
                _tilesOnBoard[i].Add(newTile);

                newTile.transform.SetParent(_tileContainer);
                newTile.ToggleTexture(false);
            }
        }
    }

    public void SetObjectOnTile(int xPos, int yPos, Transform objectToPlace)
    {
        objectToPlace.position = _tilesOnBoard[xPos][yPos].transform.position;
    }

    public void SetTextureOnTile(int xPos, int yPos, Tile newTexture = null)
    {
        if (newTexture)
            _tilesOnBoard[xPos][yPos].ChangeTileTexture(newTexture);
        else
            _tilesOnBoard[xPos][yPos].RevertTileToDefault();
    }

    public Tile GetFromBoard(int xPos, int yPos)
    {
        return _tilesOnBoard[xPos][yPos];
    }
}