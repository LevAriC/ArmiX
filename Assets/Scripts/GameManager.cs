using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Tile[] _tilesTypes;
    [SerializeField] Character[] _characterTypes;
    [SerializeField] Grid _gameBoard;
    [SerializeField] Menu _attackMenu;

    [Header("Variables")]
    [SerializeField] int _charactersPerPlayer;

    #region Grid
    public Dictionary<Vector2Int, Character> _characterDictionary { get; private set; }
    public Grid getBoard { get { return _gameBoard; } }
    #endregion

    // Properties
    public static GameManager Instance { get; private set; }

    protected void Awake()
    {
        Instance = this;
        _characterDictionary = new Dictionary<Vector2Int, Character>();
        _attackMenu = new Menu();
    }

    protected void Start()
    {
        _gameBoard.GridInit(transform);
        spawnCharacter();
    }

    protected void Update()
    {
        _attackMenu.menuController();
    }

    private void spawnCharacter()
    {
        for(int i = 0; i < _charactersPerPlayer * 2; i++)
        {
            var newCharacter = Instantiate(_characterTypes[0]);
            if (i < _charactersPerPlayer)
            {
                _gameBoard.setOnBoard(i, 0, newCharacter);
                _characterDictionary.Add(new Vector2Int(i, 0), newCharacter);
            }
            else
            {
                newCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);
                _gameBoard.setOnBoard(_gameBoard.getWidth - (i % _charactersPerPlayer) - 1, _gameBoard.getHeight - 1, newCharacter);
                _characterDictionary.Add(new Vector2Int(_gameBoard.getWidth - (i % _charactersPerPlayer) - 1, _gameBoard.getHeight - 1), newCharacter);
            }
        }
    }

    //private void moveCharacter()
    //{
    //    _gameBoard.setOnBoard(_whereClicked.x, _whereClicked.y, _characterClicked);
    //}
}
