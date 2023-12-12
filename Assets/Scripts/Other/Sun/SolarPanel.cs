using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    [SerializeField] private int numCollisions;

    Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnParticleCollision(GameObject other)
    {
        numCollisions++;
    }
}
