using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Waypoints : MonoBehaviour
{
    public GameObject[] points;
    private int _currentPoint;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float rotationSpeed = 2.0f;
    public TestObject sObjectPlayer;
    public int lap;

    [FormerlySerializedAs("_manager")] public TrafficLightManager manager;
    public BoxCollider boxCollider;

    void Start()
    {
        sObjectPlayer.InitObject(transform.position);
        GetComponent<Renderer>().material = sObjectPlayer.mat;
        speed = sObjectPlayer.speed;
        rotationSpeed = sObjectPlayer.rotationSpeed;
        Debug.Log(sObjectPlayer.name);
    }
    
    void Update()
    {
        // check the distance to the way points in the array // if we get close to one, the go to the next 
        if (Vector3.Distance(this.transform.position, points[_currentPoint].transform.position) < 2 )
        {
            _currentPoint++;
        }

        //finish the lap, start the next lap 
        if (_currentPoint >= points.Length)
        {
            _currentPoint = 0;
            lap++;
            Debug.Log(this.name+ " Lap is : " + lap);
        }
        Quaternion lookAtPoints = Quaternion.LookRotation(points[_currentPoint].transform.position - this.transform.position).normalized; // smooth out the rotation 
        this.transform.rotation = Quaternion.Lerp(transform.rotation, lookAtPoints, rotationSpeed * Time.deltaTime); // interpolate bwteen two points 
        // this.transform.LookAt(points[currentPoint].transform.position );
        this.transform.Translate(0,0,speed * Time.deltaTime); // move

        if (manager && boxCollider)
        {
            if (boxCollider.bounds.Contains(transform.position) && manager._lightStatus == 0)
            {
                speed = 0;
            }
            else
            {
                speed = sObjectPlayer.speed;
            }
        }
    }
}
