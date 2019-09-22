using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public void AttackEnemy(Character attacker, Character defender)
    {
        defender.remainingHealth -= attacker.getStrength; 
    }
}