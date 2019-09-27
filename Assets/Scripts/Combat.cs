using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public bool AttackEnemy(Character attacker, Character defender, int distance)
    {
        bool isHit = Random.value < attacker.getAccuracy ? true : false;
        if (isHit)
        {
            Debug.Log("Distance - " + distance); 
            Debug.Log("Before Hit - " + defender.remainingHealth);
            if(attacker.getStrength - distance >= 0)
                defender.remainingHealth -= (int)(attacker.getStrength - distance);
            Debug.Log("After Hit - " + defender.remainingHealth);
            return true;
        }

        return false;
    }
}