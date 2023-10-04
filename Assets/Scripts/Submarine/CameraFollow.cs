using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float moveSmoothing;
    public float rotationSmoothing;
    
    void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, moveSmoothing);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotationSmoothing);
    }
}
