using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Tile[] _tilesTypes;
    [SerializeField] Character[] _characterTypes;
    [SerializeField] Surface _gameBoard;
    [SerializeField] Menu _attackMenu;
    [SerializeField] Button _restartButton;

    [Header("Variables")]
    [SerializeField] int _charactersPerPlayer;

    #region Surface
    public Dictionary<Vector2Int, Character> _characterDictionary { get; private set; }
    public Surface getBoard { get { return _gameBoard; } }
    #endregion

    #region Turns Management
    public bool IsRedTurn { get; private set; }
    public bool GameOver { get; private set; }
    private int leftThisTurn;
    private int _blueLeft;
    private int _redLeft;
    private Vector2Int RIP;
    #endregion

    #region Click Detectors
    public Vector2Int _whereClicked { get; private set; }
    public Character _characterClicked { get; set; }
    public Character _characterEnemyClicked { get; set; }
    #endregion

    public static GameManager Instance { get; private set; }

    protected void Awake()
    {
        Instance = this;
        _restartButton.gameObject.SetActive(false);
        TurnInit();
        _characterDictionary = new Dictionary<Vector2Int, Character>();
    }

    protected void Start()
    {
        _gameBoard.SurfaceInit(transform);
        spawnCharacter();
        _attackMenu.OnMovePressedEvent += () => PlayerMoveCharacter();
    }

    protected void Update()
    {
        if(!GameOver)
        {
            _attackMenu.MenuController();
            TurnManagement();
        }
        else if (GameOver)
        {
            _restartButton.gameObject.SetActive(true);
        }
    }

    private void spawnCharacter()
    {
        for(int i = 0; i < _charactersPerPlayer * 2; i++)
        {
            var newCharacter = Instantiate(_characterTypes[0]);
            if (i < _charactersPerPlayer)
            {
                newCharacter.isRed = false;
                _gameBoard.setOnBoard(i, 0, newCharacter);
                _characterDictionary.Add(new Vector2Int(i, 0), newCharacter);
            }
            else
            {
                newCharacter.isRed = true;
                newCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);
                _gameBoard.setOnBoard(_gameBoard.getWidth - (i % _charactersPerPlayer) - 1, _gameBoard.getHeight - 1, newCharacter);
                _characterDictionary.Add(new Vector2Int(_gameBoard.getWidth - (i % _charactersPerPlayer) - 1, _gameBoard.getHeight - 1), newCharacter);
            }
        }
    }

    public List<Vector2Int> GetAllEnemiesOrAllies()
    {
        List<Vector2Int> tmpList = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
        {
            if (!alive.Value.isRed && IsRedTurn)
            {
                tmpList.Add(alive.Key);
            }
            else if (alive.Value.isRed && !IsRedTurn)
            {
                tmpList.Add(alive.Key);
            }
        }

        return tmpList;
    }

    private void TurnManagement()
    {
        if (Menu.stateChanged)
        {
            if (leftThisTurn > 0)
            {
                foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
                {
                    alive.Value.UpdateStatus();
                    if (alive.Value.isDead)
                    {
                        if(!GameIsOver(alive.Value.isRed))
                        {
                            RIP = alive.Key;
                            Destroy(alive.Value.gameObject);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        alive.Value.updateHUD();
                    }
                }

                if (RIP != new Vector2Int(-1, -1))
                {
                    _characterDictionary.Remove(RIP);
                    RIP = new Vector2Int(-1, -1);
                }

                Menu.stateChanged = false;
                leftThisTurn--;
            }
            if (leftThisTurn <= 0)
            {
                leftThisTurn = IsRedTurn ? _redLeft : _blueLeft;
                IsRedTurn = !IsRedTurn;
            }
        }
    }

    private void TurnInit()
    {
        IsRedTurn = false;
        GameOver = false;
        leftThisTurn = _charactersPerPlayer;
        _blueLeft = _charactersPerPlayer;
        _redLeft = _charactersPerPlayer;
        RIP = new Vector2Int(-1, -1);
    }

    private bool GameIsOver(bool red)
    {
        if (red)
            _redLeft--;
        else
            _blueLeft--;

        if (_redLeft <= 0 || _blueLeft <= 0)
            return GameOver = true;

        return false;
    }

    public void RestartGame()
    {
        TurnInit();
        if(_characterDictionary != null)
        {
            foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
                Destroy(alive.Value.gameObject);
        }

        _characterDictionary = new Dictionary<Vector2Int, Character>();
        spawnCharacter();
        _restartButton.gameObject.SetActive(false); 
    }

    public bool IsCharacterHere()
    {
        _whereClicked = Cursor.cursorInstance.getCoords;
        bool isCharHere = _characterDictionary.ContainsKey(_whereClicked) ? true : false;
        return isCharHere;
    }

    private void PlayerMoveCharacter()
    {
        //foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
        //{
        //    if(alive.Key == _whereClicked)
        //}
        //    _characterClicked = _characterDictionary[_whereClicked];
        _gameBoard.setOnBoard(_whereClicked.x, _whereClicked.y, _characterClicked);

    }
}