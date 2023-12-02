using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubController : MonoBehaviour
{
    // Controller class which simulates submarine movement:
    // Moving, Turning, Rising/Sinking, Stabilizing (will always default back to an upright position)
    
    private Rigidbody _rigidbody;
    private float _currentSpeed;
    public float maxSpeed;
    public float minSpeed;
    public float speedChangeAmt;
    public float turnSpeed;
    public float riseSpeed;
    public float stabilizationSmoothing;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        transform.position = new Vector3(GridMetrics.Scale / 2, GridMetrics.Scale / 2, GridMetrics.Scale / 2);
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
        Rise();
        Stabilize();
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _currentSpeed += speedChangeAmt;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _currentSpeed -= speedChangeAmt;
        }
        else if (Mathf.Abs(_currentSpeed) <= minSpeed)
        {
            _currentSpeed = 0;
        }
        _currentSpeed = Mathf.Clamp(_currentSpeed, -maxSpeed, maxSpeed);
        _rigidbody.AddForce(transform.forward * _currentSpeed);
    }
    
    private void Turn()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.AddTorque(transform.up * turnSpeed);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.AddTorque(transform.up * -turnSpeed);
        }
    }
    
    private void Rise()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space))
        {
            _rigidbody.AddForce(transform.up * riseSpeed);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            _rigidbody.AddForce(transform.up * -riseSpeed);
        }
    }
    
    private void Stabilize()
    {
        _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, Quaternion.Euler(
                new Vector3(0, _rigidbody.rotation.eulerAngles.y, 0)),stabilizationSmoothing )
        );
    }
}
