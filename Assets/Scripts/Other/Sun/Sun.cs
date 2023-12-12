using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Sun : MonoBehaviour
{
    //Assign a GameObject in the Inspector to rotate around
    public GameObject target;

    public new ParticleSystem particleSystem;

    [SerializeField]private float currentAngle;
    private Vector3 _startPos;
    private Quaternion _startRot;
    private void Awake()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    void Update()
    {
        float angle = Time.deltaTime * -10;
        
        // Spin the object around the target at 20 degrees/second.
        transform.RotateAround(target.transform.position, Vector3.forward, angle);
        currentAngle += angle;

        if (currentAngle <= -180f)
        {
            transform.position = _startPos;
            transform.rotation = _startRot;
            currentAngle = 0;
            particleSystem.Clear();
            particleSystem.Play();
        }
    }
}
