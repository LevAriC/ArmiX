using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float _x;
    [SerializeField] float _y;
    [SerializeField] float _z;

    void Update()
    {
        transform.position = target.position + new Vector3(_x, _y, _z);
        transform.LookAt(target.position + new Vector3(0, 0.5f, 1f));
    }
}