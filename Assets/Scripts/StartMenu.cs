using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] EventSystem _eventSystem;
    [SerializeField] GameObject[] _screens;
    [SerializeField] GameObject[] _firstButton;

    private enum MenuScreen { MainMenu, Singleplayer, Multiplayer, Options, Loading };
    private MenuScreen _currentScreen;
    private MenuScreen _prevScreen;
    private bool _gameStart;

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

        if (newScreen == MenuScreen.Options)
            _prevScreen = _currentScreen;

        if (_currentScreen == MenuScreen.Options)
            newScreen = _prevScreen;

        _screens[(int)newScreen].SetActive(true);
        _currentScreen = newScreen;
        _eventSystem.SetSelectedGameObject(_firstButton[(int)newScreen].gameObject);

        if (newScreen == MenuScreen.Loading)
            _gameStart = true;
    }
    //public string setTurnText { set { _turnText.GetComponent<Text>().text = value; } }
}