using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float _x;
    [SerializeField] float _y;
    [SerializeField] float _z;
    [SerializeField] float _lerp;
    private Vector3 prevPos;

    protected void Awake()
    {
        prevPos = transform.position;
    }
    protected void Update()
    {
        transform.position = target.position + new Vector3(_x, _y, _z);
        //transform.position = Vector3.Lerp(prevPos, target.position + new Vector3(_x, _y, _z), _lerp);
        transform.LookAt(LookHere());
        prevPos = transform.position;
    }

    private Vector3 LookHere()
    {
        return target.position + new Vector3(0, 1f, 1f);
    }
}