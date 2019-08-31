using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Tile[] tilesTypes;
    //[SerializeField] Tile[] tilesTypes;
    [SerializeField] Grid _gameBoard;

    // Variables
    private bool _isRunning;
    private List<Tile> _spawnedGrid;

    // Properties
    public static GameManager Instance { get; private set; }

    protected void Awake()
    {
        Instance = this;
        _isRunning = true;
        _spawnedGrid = new List<Tile>();
    }
}
