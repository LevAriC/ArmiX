using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //[Header("References")]
    //[SerializeField] Tile _tile;

    public void setOnTile(Character character)
    {
        character.transform.position = transform.position;
    }
}
