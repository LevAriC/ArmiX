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
    private int remainingHealth;
    private bool movedThisTurn;
    #endregion

    #region ID
    private static int _numOfCharacters = 0;
    public int getCharacterID { get; private set; }
    #endregion

    //[SerializeField] GameObject characterType;

    protected void Awake()
    {
        movedThisTurn = false;
        ++_numOfCharacters;
        getCharacterID = _numOfCharacters;
        remainingHealth = _health;
        _healthSlider.value = _health;
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

    private void attackCharacter(Character enemyCharacter)
    {

    }
}
