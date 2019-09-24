using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float _lerp;
    private Vector3 _prevPos;
    private Character.CharacterColors _prevColor;

    private float _x;
    private float _y;
    private float _z;

    private float _xx;
    private float _yy;
    private float _zz;

    protected void Awake()
    {
        _x = 0f;
        _y = 1.5f;
        _z = -2.5f;

        _xx = 0f;
        _yy = 1f;
        _zz = 1f;

        _prevPos = transform.position;
    }

    protected void Start()
    {
        _z = GameManager.Instance.IsPlayerOneTurn ? _z : -_z;
        _zz = GameManager.Instance.IsPlayerOneTurn ? _zz : -_zz;
    }
    protected void Update()
    {
        if (!GameManager.Instance.IsPlayerOneTurn)
        {
            _z = -_z;
            _zz = -_zz;
        }

        transform.position = target.position + new Vector3(_x, _y, _z);
        //transform.position = Vector3.Lerp(prevPos, target.position + new Vector3(_x, _y, _z), _lerp);
        transform.LookAt(LookHere());
        _prevPos = transform.position;
    }

    private Vector3 LookHere()
    {
        return target.position + new Vector3(_xx, _yy, _zz);
    }
}