using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public void attackEnemy(Character attacker, Character defender)
    {
        defender.remainingHealth -= attacker.getStrength; 
    }
}