using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Tile[] _tilesTypes;
    [SerializeField] Character[] _characterTypes;
    [SerializeField] Grid _gameBoard;

    [Header("Variables")]
    [SerializeField] int _charactersPerPlayer;

    // Variables
    private bool _isRunning;
    private List<Tile> _spawnedGrid;
    private List<Character> livingCharacters;

    // Properties
    public static GameManager Instance { get; private set; }
    public Grid getBoard { get { return _gameBoard; } }

    protected void Awake()
    {
        Instance = this;
        _isRunning = true;
        _gameBoard.BoardInit(transform);
        livingCharacters = new List<Character>();
    }

    protected void Start()
    {
        spawnCharacter();
    }

    private void spawnCharacter()
    {
        for(int i = 0; i < _charactersPerPlayer * 2; i++)
        {
            var newCharacter = Instantiate(_characterTypes[0]);
            livingCharacters.Add(newCharacter);
            if (i < _charactersPerPlayer)
            {
                _gameBoard.setOnBoard(i, 0, newCharacter);
            }
            else
            {
                _gameBoard.setOnBoard(_gameBoard.getWidth - (i % _charactersPerPlayer) - 1, _gameBoard.getHeight - 1, newCharacter);
            }
        }
    }
}
