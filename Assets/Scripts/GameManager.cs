using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
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
    public Character.CharacterColors WhosTurn { get; private set; }
    public Character.CharacterColors PlayerOneColor { get; set; }
    public Character.CharacterColors PlayerTwoColor { get; set; }
    public bool GameStarted { get; private set; }
    public bool GameOver { get; private set; }
    public string UserId { get; set; }
    public bool IsSingleplayer { get; set; }

    public Dictionary<string, Character.CharacterColors> _playersDictionary;
    public bool InvalidCommand { get; set; }
    private bool _choosingColor;
    private int _leftThisTurn;
    private int _playerOneLeft;
    private int _playerTwoLeft;
    #endregion

    #region Click Detectors
    public Vector2Int _whereClicked { get; private set; }
    public Character _characterClicked { get; set; }
    public Character _characterEnemyClicked { get; set; }
    #endregion

    public bool IsMyTurn()
    {
        foreach (KeyValuePair<string, Character.CharacterColors> player in _playersDictionary)
        {
            if (player.Value == WhosTurn && player.Key == UserId && _characterClicked == null)
                return true;
            if (player.Value == WhosTurn && player.Key == UserId && _characterClicked.MyColor == WhosTurn)
                return true;
        }
        return false;
    }
    #region Multiplayer

    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        GameStarted = true;
        WhosTurn = PlayerOneColor;
    }

    private void SendingJSONToServer()
    {
        Dictionary<string, object> _toSend = new Dictionary<string, object>();
        foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
        {
            _toSend.Add(alive.Key.ToString(), alive.Value);
        }

        string _send = MiniJSON.Json.Serialize(_toSend);
        WarpClient.GetInstance().sendMove(_send);
    }

    private void OnMoveCompleted(MoveEvent _Move)
    {
        if (_Move.getSender() != UserId)
        {
            Dictionary<string, object> _data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(_Move.getMoveData());
            if (_data != null)
            {
                var _characterDictionaryTmp = new Dictionary<Vector2Int, Character>();
                foreach(KeyValuePair<string, object> alive in _data)
                {
                    _characterDictionaryTmp.Add((Vector2Int)Enum.Parse(typeof(Vector2Int), alive.Key), (Character)alive.Value);
                }

                _characterDictionary = _characterDictionaryTmp;
                //SendingJSONToServer();
            }
            else
            {
                Debug.Log("Data not received");
            }
        }

        WhosTurn = _playersDictionary[_Move.getNextTurn()];
        GUI.stateChanged = true;
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

        _characterDictionary = new Dictionary<Vector2Int, Character>();
        _playersDictionary = new Dictionary<string, Character.CharacterColors>();

        PlayerOneColor = Character.CharacterColors.None;
        PlayerTwoColor = Character.CharacterColors.None;

        _restartButton.gameObject.SetActive(false);
        GameInit();
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
        if(GameStarted)
        {
            if (Input.GetKeyDown(KeyCode.E))
                WarpClient.GetInstance().stopGame();

            if (!GameOver)
            {
                if(IsMyTurn() || IsSingleplayer)
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

    private void SingleplayerUserID()
    {
        if (WhosTurn == PlayerOneColor)
            UserId = "PlayerOne";
        else
            UserId = "PlayerTwo";
    }

    private IEnumerator PlayersChoosingColor()
    {
        _choosingColor = true;
        while (_choosingColor)
        {
            if (PlayerOneColor != Character.CharacterColors.None)
                if(IsSingleplayer)
                    _playersDictionary.Add("PlayerOne", PlayerOneColor);
                //else
                //    _playersDictionary.Add(UserId, PlayerOneColor);

            if (PlayerTwoColor != Character.CharacterColors.None)
                if (IsSingleplayer)
                    _playersDictionary.Add("PlayerTwo", PlayerTwoColor);
                //else
                //    _playersDictionary.Add(UserId, Character.CharacterColors.Black);

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

        if(IsSingleplayer)
        {
            WhosTurn = UnityEngine.Random.value > 0.5f ? PlayerOneColor : PlayerTwoColor;
            SingleplayerUserID();
        }

        GameStarted = true;
    }

    private void GameInit()
    {
        WhosTurn = Character.CharacterColors.None;
        GameOver = false;
        _leftThisTurn = _charactersPerPlayer;
        _playerOneLeft = _charactersPerPlayer;
        _playerTwoLeft = _charactersPerPlayer;
    }

    public List<Vector2Int> GetTargetsInRange()
    {
        List<Vector2Int> tmpList = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary.ToList())
        {
            if (alive.Value.MyColor != WhosTurn)
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

                _leftThisTurn = WhosTurn == PlayerTwoColor ? _playerOneLeft : _playerTwoLeft;
                if (IsSingleplayer)
                {
                    WhosTurn = WhosTurn == PlayerTwoColor ? PlayerOneColor : PlayerTwoColor;
                    SingleplayerUserID();
                }
                else
                {
                    if (IsMyTurn())
                        SendingJSONToServer();
                }

                foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary.ToList())
                {
                    if (alive.Value.MyColor == WhosTurn || alive.Value.MyColor != WhosTurn)
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