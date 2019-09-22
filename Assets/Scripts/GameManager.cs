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
    public bool GameOver { get; private set; }
    public bool InvalidCommand { get; set; }
    private int _leftThisTurn;
    private int _blueLeft;
    private int _redLeft;
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
        _restartButton.gameObject.SetActive(false);
        TurnInit();
        _characterDictionary = new Dictionary<Vector2Int, Character>();
    }

    protected void Start()
    {
        _gameBoard.SurfaceInit(transform);
        SpawnCharacter();
        Menu.menuInstance.OnMoveCharacterEvent += OnPlayerMoveCharacter;
        Menu.menuInstance.OnOverwatchEvent += OnCharacterOverwatchingTile;
        Menu.menuInstance.OnAttackPressedEvent += () => _characterClicked.myAnimator.SetTrigger("isAttacking");
    }

    protected void Update()
    {
        if(!GameOver)
        {
            Menu.menuInstance.MenuController();
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
                newCharacter.myColor = Character.CharacterColors.Blue;
                _gameBoard.SetCharacterOnBoard(i, 0, newCharacter);
                _characterDictionary.Add(new Vector2Int(i, 0), newCharacter);
            }
            else
            {
                newCharacter.myColor = Character.CharacterColors.Red;
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
            if (alive.Value.myColor != Character.CharacterColors.Red && CurrentPlayer == Character.CharacterColors.Red)
            {
                tmpList.Add(alive.Key);
            }
            if (alive.Value.myColor == Character.CharacterColors.Red && CurrentPlayer != Character.CharacterColors.Red)
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

                Menu.stateChanged = false;
                _leftThisTurn--;
            }
            if (_leftThisTurn <= 0)
            {
                _leftThisTurn = CurrentPlayer == Character.CharacterColors.Blue ? _blueLeft : _redLeft;
                CurrentPlayer = CurrentPlayer == Character.CharacterColors.Red ? Character.CharacterColors.Blue : Character.CharacterColors.Red;
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

    private void TurnInit()
    {
        CurrentPlayer = Random.value > 0.5f ? Character.CharacterColors.Blue : Character.CharacterColors.Red;
        GameOver = false;
        _leftThisTurn = _charactersPerPlayer;
        _blueLeft = _charactersPerPlayer;
        _redLeft = _charactersPerPlayer;
        _RIP = new Vector2Int(-1, -1);
    }

    private bool GameIsOver(Character.CharacterColors color)
    {
        if (color == Character.CharacterColors.Red)
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