using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flipper : MonoBehaviour
{
    [SerializeField] private float hitStrength = 80000f;
    [SerializeField] private float dampening = 250f;
    
    [SerializeField] private HingeJoint _hingeJointLeft;
    [SerializeField] private HingeJoint _hingeJointRight;

    private JointSpring _jointSpringReleased = new();
    private JointSpring _jointSpringPressed = new();
    
    private bool _leftFlipperPressed;
    private bool _rightFlipperPressed;

    private void Start()
    {
        _jointSpringPressed.spring = _jointSpringReleased.spring = hitStrength;
        _jointSpringPressed.damper = _jointSpringReleased.damper = dampening;
        _jointSpringPressed.targetPosition = _hingeJointLeft.limits.max;
        _jointSpringReleased.targetPosition = _hingeJointLeft.limits.min;
    }

    private void Update()
    {
        // Left
        _hingeJointLeft.spring = _leftFlipperPressed ? _jointSpringPressed : _jointSpringReleased;
        // Right
        _hingeJointRight.spring = _rightFlipperPressed ? _jointSpringPressed : _jointSpringReleased;
    }

    void OnLeftFlipper(InputValue value)
    {
        Debug.Log("L");
        _leftFlipperPressed = value.isPressed;
    }

    void OnRightFlipper(InputValue value)
    {
        Debug.Log("R");
        _rightFlipperPressed = value.isPressed;
    }
}
