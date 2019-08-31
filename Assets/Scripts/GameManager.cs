using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Tile[] _tilesTypes;
    [SerializeField] Character[] _characterTypes;
    [SerializeField] Grid _gameBoard;

    // Variables
    private bool _isRunning;
    private List<Tile> _spawnedGrid;

    // Properties
    public static GameManager Instance { get; private set; }
    public Grid getBoard { get { return _gameBoard; } }

    protected void Awake()
    {
        Instance = this;
        _isRunning = true;
        _spawnedGrid = new List<Tile>();
        _gameBoard.BoardInit(transform);
    }

    protected void Start()
    {
        spawnCharacter();
    }

    protected void FixedUpdate()
    {

    }

    private void spawnCharacter()
    {
        var newCharacter = Instantiate(_characterTypes[0]);
        _gameBoard.setOnBoard(0, 0, newCharacter);
    }
}
