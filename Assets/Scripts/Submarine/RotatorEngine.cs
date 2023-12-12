using System;
using UnityEngine;

public class RotatorEngine : MonoBehaviour
{
    public Transform target; // The object to orbit around
    public float multiplier = 10;
    
    public new ParticleSystem particleSystem;
    
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!target) return;

        // Rotate around the target object
        transform.RotateAround(target.position, SubController.instance.transform.forward, 
            multiplier * SubController.instance.CurrentSpeed * Time.deltaTime);
        
        // Spawn particles
        if (particleSystem)
        {
            var emission = particleSystem.emission;
            emission.rateOverTime = Mathf.Abs(SubController.instance.CurrentSpeed * 2);
        }
        
        // Play sound
        if (_audioSource)
        {
            _audioSource.pitch = Mathf.Abs(SubController.instance.CurrentSpeed) / 50;
        }
    }
}