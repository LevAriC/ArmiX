using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public enum CharacterTypes { MachineGun, Sniper, Agent };
    public enum CharacterColors { Blue, Red };

    [Header("References")]
    [SerializeField] Slider _healthSlider;
    [SerializeField] GameObject _characterPrafab;

    #region Properties
    [Header("Attributes")]
    [SerializeField] CharacterTypes _characterType;
    [SerializeField] int _strength;
    [SerializeField] int _health;
    [SerializeField] int _movement;
    [SerializeField] int _range;

    public int getStrength { get { return _strength; } }
    public int getMovement { get { return _movement; } }
    public int getRange { get { return _range; } }
    #endregion

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

    #region Animation
    public Animator myAnimator { get; private set; }
    #endregion


    protected void Awake()
    {
        myAnimator = _characterPrafab.GetComponent<Animator>();
        movedThisTurn = false;
        ++_characterID;
        getCharacterID = _characterID;
        remainingHealth = _health;
        UpdateHUD();
    }

    public void UpdateHUD()
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