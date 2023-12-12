using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Spring : MonoBehaviour
{
    void OnLaunch()
    {
        Collider[] _colliders = Physics.OverlapSphere(transform.position, 2.5f);

        foreach (var collider in _colliders)
        {
            Ball ball = collider.GetComponent<Ball>();
            
            if (ball)
                collider.GetComponent<Ball>().Launch();
        }
    }
}
