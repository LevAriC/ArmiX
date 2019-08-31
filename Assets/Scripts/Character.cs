using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Grid _attackGrid;
    //[SerializeField] GameObject characterType;

    protected void Start()
    {
        _attackGrid.BoardInit(transform);
    }
}
