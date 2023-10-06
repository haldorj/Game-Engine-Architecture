using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{
    public GameObject redLight;
    public GameObject greenLight;
    public int _lightStatus;

    // Start is called before the first frame update
    void Start()
    {
        redLight.GetComponent<Renderer>().material.color = Color.white;
        greenLight.GetComponent<Renderer>().material.color = Color.white;
        _lightStatus = 0;
        InvokeRepeating("LightManagement", 2.0f, 3.0f);
    }

    public int LightManagement()
    {
        if (_lightStatus == 0)
        {
            greenLight.GetComponent<Renderer>().material.color = Color.green;
            redLight.GetComponent<Renderer>().material.color = Color.white;
            _lightStatus = 1;
            return _lightStatus;

        }
        else if (_lightStatus == 1)
        {
            greenLight.GetComponent<Renderer>().material.color = Color.white;
            redLight.GetComponent<Renderer>().material.color = Color.red;
            _lightStatus = 0;
            return _lightStatus;
        }
        return -1;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
