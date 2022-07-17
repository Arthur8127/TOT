using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public Transform target;  
    public float moveSpeed = 10f;
    private void FixedUpdate()
    {
        if(target)
        {
            transform.position = Vector3.Slerp(transform.position, target.position + Vector3.back, Time.fixedDeltaTime * moveSpeed);
        }
    }
}
