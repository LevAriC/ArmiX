using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public enum CharacterTypes { MachineGun, Sniper, Agent };

    [Header("References")]
    [SerializeField] Grid _attackGrid;
    [SerializeField] Grid _movementGrid;
    [SerializeField] Slider _healthSlider;

    [Header("Variables")]
    [SerializeField] CharacterTypes _characterType;
    [SerializeField] int _strength;
    [SerializeField] int _health;

    #region State
    private bool _movedThisTurn;
    public int remainingHealth { get { return remainingHealth; } set { remainingHealth = value; } }
    #endregion

    #region ID
    private static int _numOfCharacters = 0;
    public int getCharacterID { get; private set; }
    #endregion

    #region Properties
    public int getStrength { get { return _strength; } }
    #endregion

    protected void Awake()
    {
        _movedThisTurn = false;
        ++_numOfCharacters;
        getCharacterID = _numOfCharacters;
        remainingHealth = _health;
        updateHUD();
    }

    protected void Start()
    {
        _attackGrid.GridInit(transform);
        _attackGrid.gameObject.SetActive(false);
        _movementGrid.GridInit(transform);
        _movementGrid.gameObject.SetActive(false);
    }

    protected void FixedUpdate()
    {
        if(Menu.stateChanged)
        {
            updateHUD();
            Menu.stateChanged = false;
        }
    }

    public void showPossibleMove(bool show)
    {
        _movementGrid.gameObject.SetActive(show);
    }

    public void updateHUD()
    {
        _healthSlider.value = remainingHealth;
    }
}
