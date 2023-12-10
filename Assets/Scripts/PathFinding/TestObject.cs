using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Player")]
public class TestObject : ScriptableObject
{
    public float speed = 1.0f;
    public float rotationSpeed = 2.0f;
    public Vector3 pos = Vector3.zero;
    public Material mat;

    public void InitObject(Vector3 p)
    {
        pos = new Vector3(p.x, p.y, p.z);
        Debug.Log("Player :" + this.name + "placed at :"+ pos);
    }
}