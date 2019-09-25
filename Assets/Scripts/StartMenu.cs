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

        if (newScreen == MenuScreen.Loading)
        {
            gameObject.SetActive(false);
            return;
        }

        _screens[(int)newScreen].SetActive(true);
        _currentScreen = newScreen;
        _eventSystem.SetSelectedGameObject(_firstButton[(int)newScreen].gameObject);

    }

    public void ColorChosen(int color)
    {
        if (GameManager.Instance.PlayerOneColor == Character.CharacterColors.None)
        {
            GameManager.Instance.PlayerOneColor = (Character.CharacterColors)color;

            if (_currentScreen == MenuScreen.Singleplayer)
            {
                GameManager.Instance.PlayerTwoColor = (Character.CharacterColors)UnityEngine.Random.Range(0, 9);
            }
            else
            {
                GameManager.Instance.PlayerTwoColor = (Character.CharacterColors)color;
            }
        }
        else
        {
            GameManager.Instance.PlayerTwoColor = (Character.CharacterColors)color;
        }
        ChangeScreenLogic(MenuScreen.Loading);
    }
}
    //public string setTurnText { set { _turnText.GetComponent<Text>().text = value; } }