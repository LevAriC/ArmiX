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

    private Transform _transform;
    private List<List<Tile>> _tilesOnBoard;


    protected void Awake()
    {
        _transform = this.transform;
        _tilesOnBoard = new List<List<Tile>>();
        for (int i = 0; i < _height; i++)
        {
            _tilesOnBoard.Add(new List<Tile>());
        }
    }

    protected void Start()
    {
        BoardInit();
    }

    private void BoardInit()
    {
        for(int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                var newTile = Instantiate(_testTile);
                newTile.transform.Translate(new Vector3(i, j, 0));
                newTile.transform.Rotate(new Vector3(90, 0, 0));

                _tilesOnBoard[i].Add(newTile);
                newTile.transform.SetParent(_tileContainer);
            }
        }
    }

    //    [Header("References")]



    //    [SerializeField] Tile[] _grid;

    //    //public float gridSize;
    //    //public GameObject target;
    //    //public GameObject structure;
    //    //Vector3 truePos;

    //    //void LateUpdate()
    //    //{
    //    //    truePos.x = Mathf.Floor(target.transform.position.x / gridSize) * gridSize;
    //    //    truePos.y = Mathf.Floor(target.transform.position.y / gridSize) * gridSize;
    //    //    truePos.z = Mathf.Floor(target.transform.position.z / gridSize) * gridSize;

    //    //    structure.transform.position = truePos;
    //    //}
}
