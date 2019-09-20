using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public enum CharacterTypes { MachineGun, Sniper, Agent };
    public enum CharacterColors { Blue, Red };

    [Header("References")]
    [SerializeField] Surface _attackSurface;
    [SerializeField] Surface _movementSurface;
    [SerializeField] Slider _healthSlider;

    [Header("Variables")]
    [SerializeField] CharacterTypes _characterType;
    [SerializeField] int _strength;
    [SerializeField] int _health;
    [SerializeField] int _movement;
    [SerializeField] int _range;

    #region State
    public bool movedThisTurn { get; set; }
    public bool attackedThisTurn { get; set; }
    public bool overwatchedThisTurn { get; set; }
    public int remainingHealth { get; set; }
    #endregion

    #region ID
    public CharacterColors myColor { get; set; }
    private static int _characterID = 0;
    public int getCharacterID { get; private set; }
    public bool isDead { get; private set; }
    #endregion

    #region Properties
    public int getStrength { get { return _strength; } }
    public int getMovement { get { return _movement; } }
    public int getRange { get { return _range; } }
    #endregion

    protected void Awake()
    {
        movedThisTurn = false;
        ++_characterID;
        getCharacterID = _characterID;
        remainingHealth = _health;
        updateHUD();
    }

    protected void Start()
    {
        _attackSurface.SurfaceInit(transform);
        _attackSurface.gameObject.SetActive(false);
        _movementSurface.SurfaceInit(transform);
        _movementSurface.gameObject.SetActive(false);
    }

    public void showPossibleMove(bool show)
    {
        _attackSurface.gameObject.SetActive(show);
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