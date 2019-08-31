using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //[Header("References")]

    [SerializeField] Character _character;
    public bool isOccupied = false;

    public void setOnTile(Character character)
    {
        if(isOccupied)
        {

        }
        _character = character;
        character.transform.position = transform.position;
        isOccupied = true;
    }
}
