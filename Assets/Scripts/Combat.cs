using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public void AttackEnemy(Character attacker, Character defender, int distance)
    {
        defender.remainingHealth -= (int)(attacker.getStrength - distance); 
    }
}