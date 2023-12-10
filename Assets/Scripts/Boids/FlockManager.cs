using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class FlockManager : MonoBehaviour
{
    public static FlockManager instance;
    public GameObject boidPrefab;
    public GameObject goalPrefab;
    private GameObject _goal;
    public int numBoids = 20;
    public GameObject[] boids;
    public Vector3 limits = new(5, 5, 5);
    public Vector3 goalPos = Vector3.zero;

    [Header("Boid Settings")]
    [Range(0.0f, 5.0f)] public float minSpeed = 0.5f;
    [Range(0.0f, 5.0f)] public float maxSpeed = 2.0f;
    [Range(1.0f, 10.0f)] public float neighbourDistance = 3.0f;
    [Range(1.0f, 5.0f)] public float rotationSpeed = 1.0f;
    
    private void Awake()
    {
        boids = new GameObject[numBoids];
        for (int i = 0; i < numBoids; i++)
        {
            Vector3 pos = new Vector3(
                UnityEngine.Random.Range(-limits.x, limits.x),
                UnityEngine.Random.Range(-limits.y, limits.y),
                UnityEngine.Random.Range(-limits.z, limits.z)
            );
            boids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
            //boids[i].GetComponent<Boid>().flockManager = this;
        }
        instance = this;
        goalPos = this.transform.position;
        _goal = Instantiate(goalPrefab, goalPos, Quaternion.identity);
        _goal.transform.position = goalPos;
    }
    
    private void Update()
    {
        if (UnityEngine.Random.Range(0, 10000) < 10)
        {
            goalPos = GetRandomGoalPos();
            _goal.transform.position = goalPos;
        }
    }
    
    Vector3 GetRandomGoalPos()
    {
        Vector3 pos = new Vector3(
            UnityEngine.Random.Range(-limits.x, limits.x),
            UnityEngine.Random.Range(-limits.y, limits.y),
            UnityEngine.Random.Range(-limits.z, limits.z)
        );
        return pos;
    }
}