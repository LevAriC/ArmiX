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
    [SerializeField] Button _restartButton;

    [Header("Variables")]
    [SerializeField] int _charactersPerPlayer;

    #region Surface
    public Dictionary<Vector2Int, Character> _characterDictionary { get; private set; }
    public Surface GetBoard { get { return _gameBoard; } }
    #endregion

    #region Turns Management
    public Character.CharacterColors CurrentPlayer { get; private set; }
    public Character.CharacterColors PlayerOneColor { get; set; }
    public Character.CharacterColors PlayerTwoColor { get; set; }
    public bool GameOver { get; private set; }
    public bool InvalidCommand { get; set; }
    private int _leftThisTurn;
    private int _playerOneLeft;
    private int _playerTwoLeft;
    private Vector2Int _RIP;
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
        PlayerOneColor = Character.CharacterColors.Black;
        PlayerTwoColor = Character.CharacterColors.Yellow;
        _restartButton.gameObject.SetActive(false);
        GameInit();
        _characterDictionary = new Dictionary<Vector2Int, Character>();
    }

    protected void Start()
    {
        _gameBoard.SurfaceInit(transform);
        SpawnCharacter();
        GUI.menuInstance.OnMoveCharacterEvent += OnPlayerMoveCharacter;
        GUI.menuInstance.OnOverwatchEvent += OnCharacterOverwatchingTile;
        GUI.menuInstance.OnAttackPressedEvent += () => _characterClicked.myAnimator.SetTrigger("isAttacking");
    }

    protected void Update()
    {
        if(!GameOver)
        {
            GUI.menuInstance.MenuController();
            TurnManagement();
        }
        else if (GameOver)
        {
            _restartButton.gameObject.SetActive(true);
        }
    }

    private void SpawnCharacter()
    {
        for(int i = 0; i < _charactersPerPlayer * 2; i++)
        {
            var newCharacter = Instantiate(_characterTypes[i % _charactersPerPlayer]);
            if (i < _charactersPerPlayer)
            {
                newCharacter.SetColor(PlayerOneColor);
                _gameBoard.SetCharacterOnBoard(i, 0, newCharacter);
                _characterDictionary.Add(new Vector2Int(i, 0), newCharacter);
            }
            else
            {
                newCharacter.SetColor(PlayerTwoColor);
                newCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);
                _gameBoard.SetCharacterOnBoard(_gameBoard.GetWidth - (i % _charactersPerPlayer) - 1, _gameBoard.GetHeight - 1, newCharacter);
                _characterDictionary.Add(new Vector2Int(_gameBoard.GetWidth - (i % _charactersPerPlayer) - 1, _gameBoard.GetHeight - 1), newCharacter);
            }
        }
    }

    public List<Vector2Int> GetAllEnemiesOrAllies()
    {
        List<Vector2Int> tmpList = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
        {
            if (alive.Value.myColor != PlayerTwoColor && CurrentPlayer == PlayerTwoColor)
            {
                tmpList.Add(alive.Key);
            }
            if (alive.Value.myColor == PlayerTwoColor && CurrentPlayer != PlayerTwoColor)
            {
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
                foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
                {
                    alive.Value.UpdateStatus();
                    if (alive.Value.isDead)
                    {
                        if (!GameIsOver(alive.Value.myColor))
                        {
                            _RIP = alive.Key;
                            Destroy(alive.Value.gameObject);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        alive.Value.UpdateHUD();
                    }
                }

                if (_RIP != new Vector2Int(-1, -1))
                {
                    _characterDictionary.Remove(_RIP);
                    _RIP = new Vector2Int(-1, -1);
                }

                GUI.stateChanged = false;
                _leftThisTurn--;
            }
            if (_leftThisTurn <= 0)
            {
                _leftThisTurn = CurrentPlayer == PlayerOneColor ? _playerOneLeft : _playerTwoLeft;
                CurrentPlayer = CurrentPlayer == PlayerTwoColor ? PlayerOneColor : PlayerTwoColor;
                foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
                {
                    if (alive.Value.myColor == CurrentPlayer)
                    {
                        Cursor.cursorInstance.MoveCursor(alive.Key.x, alive.Key.x);
                        break;
                    }
                }
            }
        }
    } 

    private void GameInit()
    {
        CurrentPlayer = Random.value > 0.5f ? PlayerOneColor : PlayerTwoColor;
        GameOver = false;
        _leftThisTurn = _charactersPerPlayer;
        _playerOneLeft = _charactersPerPlayer;
        _playerTwoLeft = _charactersPerPlayer;
        _RIP = new Vector2Int(-1, -1);
    }

    private bool GameIsOver(Character.CharacterColors color)
    {
        if (color == PlayerTwoColor)
            _playerTwoLeft--;
        else
            _playerOneLeft--;

        if (_playerTwoLeft <= 0 || _playerOneLeft <= 0)
            return GameOver = true;

        return false;
    }

    public void RestartGame()
    {
        GameInit();
        if(_characterDictionary != null)
        {
            foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
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

        foreach (KeyValuePair<Vector2Int, Character> alive in _characterDictionary)
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