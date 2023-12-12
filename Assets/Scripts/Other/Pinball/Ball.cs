using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _startPos;
    
    [SerializeField]private TextMeshProUGUI textMeshProUGUI;
    [SerializeField]private TextMeshProUGUI textMeshProTimer;
    [SerializeField]private int score;
    
    private float _seconds;
    private int _minutes;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _startPos = transform.position;
        ResetTimerUI();
    }

    private void Update()
    {
        if (textMeshProUGUI)
            textMeshProUGUI.text = "score: " + score.ToString();

        if (score > 0)
            UpdateTimerUI(Time.deltaTime);
        else
            ResetTimerUI();
    }

    public void Launch()
    {
        // Add force upwards
        _rigidbody.AddForce(new Vector3(0, 60, 0), ForceMode.Impulse);
    }
    
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bumper"))
        {
            // Calculate the direction from the collision point to the center of the GameObject.
            Vector3 forceDirection = transform.position - col.contacts[0].point;
            forceDirection.Normalize();

            float strength = Random.Range(10, 40);
            
            // Apply the impulse using AddForce with ForceMode.Impulse.
            GetComponent<Rigidbody>().AddForce(forceDirection * strength, ForceMode.Impulse);

            score += (int)strength * 100;
        }

        if (col.gameObject.CompareTag("Floor"))
        {
            transform.position = _startPos;
            score = 0;
        }
    }
    
    public void UpdateTimerUI(float dt)
    {
        if (textMeshProTimer)
        {
            _seconds += dt;
            textMeshProTimer.text = "Time: " + _minutes + ":" + (int)_seconds;
            if (_seconds >= 60)
            {
                _minutes++;
                _seconds = 0;
            }
        }
    }

    private void ResetTimerUI()
    {
        _seconds = 0;
        _minutes = 0;
        textMeshProTimer.text = "Time: 0:0";
    }
}
