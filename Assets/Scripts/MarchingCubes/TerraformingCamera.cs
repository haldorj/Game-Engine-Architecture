using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraformingCamera : MonoBehaviour
{
    private Vector3 _hitPoint;
    private Camera _camera;

    public float brushSize;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    
    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
            Terraform(true);
        else if (Input.GetMouseButton(1))
            Terraform(false);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Terraform(bool add)
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit, 1000))
        {
            Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();

            _hitPoint = hit.point;
            // print(_hitPoint.ToString());
            
            hitChunk.EditWeights(_hitPoint, brushSize, add);
        }
    }
    
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_hitPoint, brushSize);
    }
}
