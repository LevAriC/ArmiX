using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Tile[] _tilesTypes;
    [SerializeField] Character[] _characterTypes;
    [SerializeField] Surface _gameBoard;
    [SerializeField] Button _restartButton;

    [Header("Variables")]
    [SerializeField] int _charactersPerPlayer;

    #region Surface
    public Dictionary<Vector2Int, Character> _characterDictionary { get; private set; }
    public Surface GetBoard { get { return _gameBoard; } }
    #endregion

    #region Turns Management
    public bool IsMyTurn { get; private set; }
    public Character.CharacterColors PlayerOneColor { get; set; }
    public Character.CharacterColors PlayerTwoColor { get; set; }
    public bool GameStarted { get; private set; }
    public bool GameOver { get; private set; }
    public string UserId { get; set; }

    public bool InvalidCommand { get; set; }
    private bool _choosingColor;
    private int _leftThisTurn;
    private int _playerOneLeft;
    private int _playerTwoLeft;
    //private Vector2Int _RIP;
    #endregion

    #region Click Detectors
    public Vector2Int _whereClicked { get; private set; }
    public Character _characterClicked { get; set; }
    public Character _characterEnemyClicked { get; set; }
    #endregion

    #region Multiplayer
    public Dictionary<string, object> _toSend;

    private void SendingJSONToServer()
    {
        string _send = MiniJSON.Json.Serialize(_toSend);
        WarpClient.GetInstance().sendMove(_send);
    }
    private void OnMoveCompleted(MoveEvent _Move)
    {
        if (_Move.getSender() != UserId)
        {
            Dictionary<string, object> _data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(_Move.getMoveData());
            if (_data != null && _data.ContainsKey("Index"))
            {
                int _index = int.Parse(_data["Index"].ToString());
                SendingJSONToServer();
            }
        }

        if (_Move.getNextTurn() == UserId)
            IsMyTurn = true;
        else IsMyTurn = false;
    }

    private void OnGameStopped(string _Sender, string _RoomId)
    {
        Debug.Log("Game Over");
    }

    private void OnEnable()
    {
        Listener.OnGameStarted += OnGameStarted;
        Listener.OnMoveCompleted += OnMoveCompleted;
        Listener.OnGameStopped += OnGameStopped;
    }

    private void OnDisable()
    {
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnMoveCompleted -= OnMoveCompleted;
        Listener.OnGameStopped -= OnGameStopped;
    }
    #endregion
    public static GameManager Instance { get; private set; }

    protected void Awake()
    {
        Instance = this;
        PlayerOneColor = Character.CharacterColors.None;
        PlayerTwoColor = Character.CharacterColors.None;

        _restartButton.gameObject.SetActive(false);
        UserId = null;
        GameInit();
        _characterDictionary = new Dictionary<Vector2Int, Character>();
        _toSend = new Dictionary<string, object>();

    }
    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        GameStarted = true;
        if (UserId == _NextTurn)
        {
            IsMyTurn = true;
        }
        else
        {
            IsMyTurn = false;
        }
    }
    protected void Start()
    {
        _gameBoard.SurfaceInit(transform);
        SpawnCharacter();
        GUI.menuInstance.MoveCharacterEvent += OnPlayerMoveCharacter;
        GUI.menuInstance.OverwatchEvent += OnCharacterOverwatchingTile;
        GUI.menuInstance.AttackPressedEvent += () => _characterClicked.myAnimator.SetTrigger("isAttacking");
        StartCoroutine(PlayersChoosingColor());
    }
    protected void Update()
    {
        if(GameStarted || UserId == null && !GameStarted)
        {
            if(!GameOver)
            {
                if(IsMyTurn)
                {
                    GUI.menuInstance.MenuController();
                    TurnManagement();
                }
            }
            else if (GameOver)
            {
                _restartButton.gameObject.SetActive(true);
            }
        }
    }

    private void SpawnCharacter()
    {
        for(int i = 0; i < _charactersPerPlayer * 2; i++)
        {
            var newCharacter = Instantiate(_characterTypes[i % _charactersPerPlayer]);
            if (i < _charactersPerPlayer)
            {
                newCharacter.IsPlayerOne = true;
                _gameBoard.SetCharacterOnBoard(i, 0, newCharacter);
                _characterDictionary.Add(new Vector2Int(i, 0), newCharacter);
            }
            else
            {
                newCharacter.IsPlayerOne = false;
                newCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);
                _gameBoard.SetCharacterOnBoard(_gameBoard.GetWidth - (i % _charactersPerPlayer) - 1, _gameBoard.GetHeight - 1, newCharacter);
                _characterDictionary.Add(new Vector2Int(_gameBoard.GetWidth - (i % _charactersPerPlayer) - 1, _gameBoard.GetHeight - 1), newCharacter);
            }
        }
    }

    private IEnumerator PlayersChoosingColor()
    {
        _choosingColor = true;
        while (_choosingColor)
        {
            if (PlayerOneColor != Character.CharacterColors.None && PlayerTwoColor != Character.CharacterColors.None)
                _choosingColor = false;
            yield return null;
        }

        foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary.ToList())
        {
            if (alive.Value.IsPlayerOne)
                alive.Value.SetColor(PlayerOneColor);
            else
                alive.Value.SetColor(PlayerTwoColor);
        }
    }

    private void GameInit()
    {
        IsMyTurn = true;
        //IsMyTurn = Random.value > 0.5f ? true : false;
        GameOver = false;
        _leftThisTurn = _charactersPerPlayer;
        _playerOneLeft = _charactersPerPlayer;
        _playerTwoLeft = _charactersPerPlayer;
        //_RIP = new Vector2Int(-1, -1);
    }

    public List<Vector2Int> GetTargetsInRange()
    {
        List<Vector2Int> tmpList = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary.ToList())
        {
            if (alive.Value.myColor != PlayerTwoColor && !IsMyTurn || alive.Value.myColor == PlayerTwoColor && IsMyTurn)
            {
                if(_whereClicked.x <= alive.Key.x && alive.Key.x <= _whereClicked.x + _characterClicked.getRange ||
                   _whereClicked.x >= alive.Key.x && alive.Key.x >= _whereClicked.x + _characterClicked.getRange ||
                   _whereClicked.y <= alive.Key.y && alive.Key.y <= _whereClicked.y + _characterClicked.getRange ||
                   _whereClicked.y >= alive.Key.y && alive.Key.y >= _whereClicked.y + _characterClicked.getRange)
                tmpList.Add(alive.Key);
            }
        }

        return tmpList;
    }

    private void TurnManagement()
    {
        if (GUI.stateChanged)
        {
            if (_leftThisTurn > 0)
            {
                foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary.ToList())
                {
                    alive.Value.UpdateStatus();
                    if (alive.Value.isDead)
                    {
                        if (!CheckGameIsOver(alive.Value.IsPlayerOne))
                        {
                            _characterDictionary.Remove(alive.Key);
                            Destroy(alive.Value.gameObject);
                        }
                        else
                        {
                            WarpClient.GetInstance().stopGame();
                            return;
                        }
                    }
                }

                GUI.stateChanged = false;
                _leftThisTurn--;
            }

            if (_leftThisTurn <= 0)
            {
                _leftThisTurn = !IsMyTurn ? _playerOneLeft : _playerTwoLeft;
                IsMyTurn = !IsMyTurn;
                foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary.ToList())
                {
                    if (alive.Value.IsPlayerOne == IsMyTurn || !(alive.Value.IsPlayerOne) == !IsMyTurn)
                        Cursor.cursorInstance.MoveCursor(alive.Key.x, alive.Key.y);

                    alive.Value.ResetState();
                }
            }
        }
    } 

    private bool CheckGameIsOver(bool player)
    {
        if (player)
            _playerOneLeft--;
        else
            _playerTwoLeft--;

        if (_playerTwoLeft <= 0 || _playerOneLeft <= 0)
            return GameOver = true;

        return false;
    }

    public void RestartGame()
    {
        GameInit();
        if(_characterDictionary != null)
        {
            foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary.ToList())
                Destroy(alive.Value.gameObject);
        }

        _characterDictionary = new Dictionary<Vector2Int, Character>();
        SpawnCharacter();
        _restartButton.gameObject.SetActive(false); 
    }

    public bool IsCharacterHere()
    {
        _whereClicked = Cursor.cursorInstance.GetCoords;
        bool isCharHere = _characterDictionary.ContainsKey(_whereClicked) ? true : false;
        return isCharHere;
    }

    private void OnPlayerMoveCharacter()
    {
        _gameBoard.SetCharacterOnBoard(_whereClicked.x, _whereClicked.y, _characterClicked);

        foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary.ToList())
        {
            if (alive.Value.getCharacterID == _characterClicked.getCharacterID)
            {
                _characterDictionary.Remove(alive.Key);
                _characterDictionary.Add(new Vector2Int(_whereClicked.x, _whereClicked.y), _characterClicked);
            }
        }
    }

    private void OnCharacterOverwatchingTile()
    {
        _gameBoard.SetTextureOnTiles(_whereClicked.x, _whereClicked.y, _tilesTypes[3]);

        //foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
        //{
        //    if (alive.Value.getCharacterID == _characterClicked.getCharacterID)
        //    {
        //        _characterDictionary.Remove(alive.Key);
        //        _characterDictionary.Add(new Vector2Int(_whereClicked.x, _whereClicked.y), _characterClicked);
        //    }
        //}
    }
}