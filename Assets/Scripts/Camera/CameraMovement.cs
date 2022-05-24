using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float xSensivity = 1;
    [SerializeField] private float ySensivity = 1;
    float xRotation;
    float yRotation;
    Quaternion rotation;

    [SerializeField] Transform player;
    [SerializeField] Transform look;
    [SerializeField] Vector3 offset;
    [SerializeField] [Range(1,10)] float smoothValue;
    [SerializeField] private Vector3 minValue;
    [SerializeField] private Vector3 maxValue;
    [SerializeField] private Vector3 newOffset;
    [SerializeField] private Transform aim;

    private void Update()
    {
        //Follow();
        RotateCamera();
    }
    
    private void Follow()
    {
        var followPosition = player.position + offset;
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(followPosition.x, minValue.x, maxValue.x),
            Mathf.Clamp(followPosition.y, minValue.y, maxValue.y),
            Mathf.Clamp(followPosition.z, minValue.z, maxValue.z));
        
        followPosition = Vector3.Lerp(transform.position, boundPosition, smoothValue * Time.deltaTime);
        transform.position = followPosition;
    }

    public void RotateCamera()
    {
        yRotation += xSensivity * Input.GetAxis("Mouse X");
        xRotation -= ySensivity * Input.GetAxis("Mouse Y");
        
        rotation = Quaternion.Euler(xRotation, yRotation, 0);
        xRotation = Mathf.Clamp(xRotation, -30, 45);

        transform.position = player.position + rotation * offset;
        transform.position = transform.position + offset;
        transform.LookAt(player);
    }
}