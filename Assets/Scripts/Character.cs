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
    public bool movedThisTurn { get; set; }
    public int remainingHealth { get; set; }
    #endregion

    #region ID
    private static int _characterID = 0;
    public int getCharacterID { get; private set; }
    public bool isRed { get; set; }
    public bool isDead { get; private set; }
    #endregion

    #region Properties
    public int getStrength { get { return _strength; } }
    #endregion

    protected void Awake()
    {
        movedThisTurn = false;
        isRed = false;
        ++_characterID;
        getCharacterID = _characterID;
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

    public void showPossibleMove(bool show)
    {
        _movementGrid.gameObject.SetActive(show);
    }

    public void updateHUD()
    {
        _healthSlider.value = remainingHealth;
    }

    public void UpdateStatus()
    {
        if (remainingHealth <= 0)
        {
            isDead = true;
        }
    }
}