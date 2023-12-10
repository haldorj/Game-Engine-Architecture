using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private float _speed;
    
    private void Start()
    {
        _speed = UnityEngine.Random.Range(FlockManager.instance.minSpeed, FlockManager.instance.maxSpeed);
    }
    
    private void Update()
    {
        Bounds b = new Bounds(FlockManager.instance.transform.position, FlockManager.instance.limits * 2);

        if (!b.Contains(transform.position))
        {
            Vector3 direction = FlockManager.instance.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction),
                FlockManager.instance.rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Reset speed
            if (UnityEngine.Random.Range(0, 1000) < 50)
                _speed = UnityEngine.Random.Range(FlockManager.instance.minSpeed, FlockManager.instance.maxSpeed);
            
            ApplyRules();
        }
        
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
    }
    
    void ApplyRules()
    {
        GameObject[] boids = FlockManager.instance.boids; // get all boids
        Vector3 center = Vector3.zero; // center of the group
        Vector3 avoid = Vector3.zero; // avoid collision
        float groupSpeed = 0.1f; // speed of the group

        Vector3 goalPos = FlockManager.instance.goalPos; // goal position

        int groupSize = 0;
        foreach (GameObject boid in boids)
        {
            // if self: skip
            if (boid == gameObject) continue;
            
            // calculate distance between boid and self
            var dist = Vector3.Distance(boid.transform.position, transform.position);
            // if within neighbour distance
            if (dist <= FlockManager.instance.neighbourDistance) 
            {
                // add position of boid to center and increment group size
                center += boid.transform.position;
                groupSize++;
                // if too close to boid
                if (dist < 1.0f) 
                {
                    // add position of boid to avoid vector
                    avoid += (transform.position - boid.transform.position);
                }
                // get speed of boid and add to group speed
                Boid anotherBoid = boid.GetComponent<Boid>();
                groupSpeed += anotherBoid._speed;
            }
        }

        // if group size is greater than 0
        if (groupSize > 0)
        {
            // calculate center and speed of group and add goal position to center and speed of group
            center = center / groupSize + (goalPos - transform.position);
            _speed = groupSpeed / groupSize;
            
            _speed = Mathf.Clamp(_speed, FlockManager.instance.minSpeed, FlockManager.instance.maxSpeed);

            // calculate direction of group and rotate towards it
            Vector3 direction = (center + avoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction),
                    FlockManager.instance.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
