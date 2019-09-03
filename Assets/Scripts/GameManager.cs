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
    #region Menu
    private bool _menuOpen;
    private Vector2Int _whereClicked;
    Character _characterClicked;
    #endregion

    #region Grid
    private Dictionary<Vector2Int, Character> _characterDictionary;
    public Grid getBoard { get { return _gameBoard; } }
    #endregion

    // Properties
    public static GameManager Instance { get; private set; }


    protected void Awake()
    {
        Instance = this;
        _menuOpen = false;
        _characterDictionary = new Dictionary<Vector2Int, Character>();
    }

    protected void Start()
    {
        _gameBoard.GridInit(transform);
        spawnCharacter();
    }

    protected void Update()
    {
        menuController();
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

    private void menuController()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_menuOpen)
        {
            _whereClicked = Cursor.cursorInstance.getCoords;
            _characterClicked = _characterDictionary[_whereClicked];
            if(_characterDictionary.ContainsKey(_whereClicked))
            {
                _menuOpen = true;
                _characterClicked.showPossibleMove(_menuOpen);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && _menuOpen)
        {
            if (_characterDictionary.ContainsKey(_whereClicked))
            {
                _menuOpen = false;
                _characterClicked.showPossibleMove(_menuOpen);
            }
        }
    }

    private void moveCharacter()
    {
        //_gameBoard.setOnBoard(cursorPos.x, cursorPos.y, toMove);
    }
}
