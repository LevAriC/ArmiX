﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MiniJSON;
using UnityEngine.UI;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class StartMenu : MonoBehaviour
{
    [SerializeField] EventSystem _eventSystem;
    [SerializeField] GameObject[] _screens;
    [SerializeField] GameObject[] _firstButton;
    [SerializeField] GameObject _multiplayerPanel;
    [SerializeField] GameObject _statusText;
    [SerializeField] GameObject _slider;
    [SerializeField] GameObject _sliderMusic;

    private enum MenuScreen { MainMenu, Singleplayer, Multiplayer, Options, Loading };
    private MenuScreen _currentScreen;

    #region Multiplayer
    private string apiKey = "f776fb1419d8e7b2315c7a0755351306ebc4eedd2c9d7b0994ebb3812427d049";
    private string secretKey = "77e4c68e882c992617656369508b5c95cefe9ad18eb22fb40e9edae7a3b0ebf5";
    private string curRoomId;
    private int roomIdx = 0;
    private int _password;
    public Listener listen;

    private void OnEnable()
    {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGameStarted += OnGameStarted;
    }
    private void OnDisable()
    {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGameStarted -= OnGameStarted;
    }

    private void MenuInit()
    {
        if (listen == null)
            listen = new Listener();

        WarpClient.initialize(apiKey, secretKey);
        WarpClient.GetInstance().AddConnectionRequestListener(listen);
        WarpClient.GetInstance().AddChatRequestListener(listen);
        WarpClient.GetInstance().AddUpdateRequestListener(listen);
        WarpClient.GetInstance().AddLobbyRequestListener(listen);
        WarpClient.GetInstance().AddNotificationListener(listen);
        WarpClient.GetInstance().AddRoomRequestListener(listen);
        WarpClient.GetInstance().AddZoneRequestListener(listen);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listen);

        matchRoomData = new Dictionary<string, object>();
    }
    #endregion

    private Dictionary<string, GameObject> unityObjects;
    private Dictionary<string, object> matchRoomData;
    private List<string> roomIds;

    protected void Start()
    {
        var buttons = _multiplayerPanel.GetComponentsInChildren<Button>();
        foreach (var toDisable in buttons)
            toDisable.interactable = false;

        MenuInit();
    }

    #region Events
    private void OnConnect(bool _IsSuccess)
    {
        Debug.Log(_IsSuccess);
        if (_IsSuccess)
        {
            UpdateStatus("Connected!");
            var buttons = _multiplayerPanel.GetComponentsInChildren<Button>();
            foreach (var toDisable in buttons)
            {
                if (toDisable.name != GameManager.Instance.PlayerOneColor.ToString())
                    toDisable.interactable = true;
            }
        }
        else UpdateStatus("Connection Error");
    }
    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        Debug.Log(_IsSuccess + " " + "" + eventObj.getRoomsData().Length);
        if (_IsSuccess)
        {
            UpdateStatus("Parsing Rooms");
            roomIds = new List<string>();
            foreach (var roomData in eventObj.getRoomsData())
            {
                Debug.Log("RoomId " + roomData.getId());
                Debug.Log("Room Owner " + roomData.getRoomOwner());
                roomIds.Add(roomData.getId());
            }

            roomIdx = 0;
            DoRoomSearchLogic();
        }
        else UpdateStatus("Error Fetching Rooms in Range");
    }
    private void DoRoomSearchLogic()
    {
        if (roomIdx < roomIds.Count)
        {
            UpdateStatus("Get Room Details (" + roomIds[roomIdx] + ")");
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIdx]);
        }
        else
        {
            UpdateStatus("Create Room...");
            WarpClient.GetInstance().CreateTurnRoom("ArmiX", GameManager.Instance.UserId, 2, matchRoomData, 120);
        }
    }
    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        Debug.Log("OnCreateRoom " + _IsSuccess + " " + _RoomId);
        if (_IsSuccess)
        {
            UpdateStatus("Room Created, waiting for opponent...");
            curRoomId = _RoomId;
            WarpClient.GetInstance().JoinRoom(curRoomId);
            WarpClient.GetInstance().SubscribeRoom(curRoomId);
        }
        else UpdateStatus("Failed to create Room");
    }
    private void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        Dictionary<string, object> _prams = eventObj.getProperties();
        if (_prams != null && _prams.ContainsKey("Password"))
        {
            string _pass = _prams["Password"].ToString();
            Debug.Log("_pass - " + _pass);
            Debug.Log("_password - " + _password);
            Debug.Log("_passs - " + matchRoomData["Password"].ToString());

            if (_pass == matchRoomData["Password"].ToString())
            {
                curRoomId = eventObj.getData().getId();
                UpdateStatus("Joining Room " + curRoomId);
                if (_prams.ContainsKey("PlayerOneColor"))
                {
                    GameManager.Instance.PlayerTwoColor = GameManager.Instance.PlayerOneColor;
                    GameManager.Instance.PlayerOneColor = (Character.CharacterColors)Enum.Parse(typeof(Character.CharacterColors), _prams["PlayerOneColor"].ToString());
                    GameManager.Instance._playersDictionary.Add(eventObj.getData().getRoomOwner(), GameManager.Instance.PlayerOneColor);
                    var _maxX = GameManager.Instance.GetBoard.GetWidth - 1;
                    var _maxY = GameManager.Instance.GetBoard.GetHeight - 1;
                    Cursor.cursorInstance.MoveCursor(_maxX, _maxY);
                }

                Debug.Log("Player one color - " + GameManager.Instance.PlayerOneColor);
                WarpClient.GetInstance().JoinRoom(curRoomId);
                WarpClient.GetInstance().SubscribeRoom(curRoomId);
            }
            else
            {
                UpdateStatus("Password Incorrect");
                Debug.Log("Password Incorrect");
                roomIdx++;
                DoRoomSearchLogic();
            }
        }
    }
    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
            UpdateStatus("Succefully Joined Room " + _RoomId);
        else UpdateStatus("Failed to Joined Room " + _RoomId);
    }
    private void OnUserJoinRoom(RoomData eventObj, string _UserId)
    {
        if (GameManager.Instance.UserId != _UserId)
        {
            GameManager.Instance._playersDictionary.Add(_UserId, Character.CharacterColors.Black);
            GameManager.Instance.PlayerTwoColor = Character.CharacterColors.Black;
            UpdateStatus(_UserId + " Have joined the room");
            WarpClient.GetInstance().startGame();
        }
    }
    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region ArmiX
    protected void Awake()
    {
        Init();
    }
    private void Init()
    {
        foreach (GameObject scr in _screens)
            scr.SetActive(false);

        ChangeScreenLogic(MenuScreen.MainMenu);
    }
    public void ChangeToScreen(string chosenScreen)
    {
        MenuScreen newScreen = (MenuScreen)Enum.Parse(typeof(MenuScreen), chosenScreen);
        ChangeScreenLogic(newScreen);
    }
    private void ChangeScreenLogic(MenuScreen newScreen)
    {
        _screens[(int)_currentScreen].SetActive(false);

        if (newScreen == MenuScreen.Multiplayer && GameManager.Instance.UserId == null)
        {
            GameManager.Instance.UserId = System.DateTime.Now.Ticks.ToString();
            Debug.Log(GameManager.Instance.UserId);
            WarpClient.GetInstance().Connect(GameManager.Instance.UserId);
            UpdateStatus("Connecting...");
        }

        if (newScreen == MenuScreen.Loading && _currentScreen == MenuScreen.Multiplayer)
        {
            UpdateStatus("Searching for room...");
            WarpClient.GetInstance().GetRoomsInRange(1, 2);
        }

        if (newScreen == MenuScreen.Loading && _currentScreen == MenuScreen.Singleplayer)
        {
            gameObject.SetActive(false);
            return;
        }
        _screens[(int)newScreen].SetActive(true);
        _currentScreen = newScreen;
        if(_firstButton[(int)newScreen] != null)
            _eventSystem.SetSelectedGameObject(_firstButton[(int)newScreen].gameObject);

    }
    public void ColorChosen(int color)
    {
        if (GameManager.Instance.PlayerOneColor == Character.CharacterColors.None)
        {
            if (_currentScreen == MenuScreen.Singleplayer)
            {
                GameManager.Instance.IsSingleplayer = true;
                GameManager.Instance.PlayerOneColor = (Character.CharacterColors)color;
                GameManager.Instance.PlayerTwoColor = (Character.CharacterColors)UnityEngine.Random.Range(0, 9);
                while(GameManager.Instance.PlayerTwoColor == GameManager.Instance.PlayerOneColor)
                    GameManager.Instance.PlayerTwoColor = (Character.CharacterColors)UnityEngine.Random.Range(0, 9);
            }
            else
            {
                GameManager.Instance.PlayerOneColor = (Character.CharacterColors)color;
                matchRoomData.Add("PlayerOneColor", GameManager.Instance.PlayerOneColor.ToString());
                GameManager.Instance._playersDictionary.Add(GameManager.Instance.UserId, GameManager.Instance.PlayerOneColor);
            }
        }
        //else
        //{
        //    GameManager.Instance.PlayerTwoColor = Character.CharacterColors.Black;
        //}

        ChangeScreenLogic(MenuScreen.Loading);
    }
    private void UpdateStatus(string _NewStatus)
    {
        _statusText.GetComponent<Text>().text = _NewStatus;
    }

    public void UpdateSlider()
    {
        _password = (int)_slider.GetComponent<Slider>().value;
        _slider.GetComponentInChildren<Text>().text = _password.ToString();
    }
    public void MusicVolumeSlider()
    {
        int volume = (int)_sliderMusic.GetComponent<Slider>().value;
        GameManager.Instance.SetMusicVolume(volume);
        _sliderMusic.GetComponentInChildren<Text>().text = volume.ToString();
    }
    public void StartMulti()
    {
        matchRoomData.Add("Password", _password.ToString());
        _slider.transform.parent.gameObject.SetActive(false);
        _multiplayerPanel.gameObject.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("sdsdsd");
        Application.Quit();
    }

    #endregion
}