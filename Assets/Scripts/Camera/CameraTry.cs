using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CameraTry : MonoBehaviour
{
    public Vector3 crosshair;

    public Transform player;
    public Transform aimTarget;
   
    public float smoothingTime = 10.0f; // it should follow it faster by jumping (y-axis) (previous: 0.1 or so) ben0bi
    public Vector3 pivotOffset = new Vector3(0.2f, 0.7f,  0.0f); // offset of point from player transform (?) ben0bi
    public Vector3 camOffset   = new Vector3(0.0f, 0.7f, -3.4f); // offset of camera from pivotOffset (?) ben0bi
    public Vector3 closeOffset = new Vector3(0.35f, 1.7f, 0.0f); // close offset of camera from pivotOffset (?) ben0bi
   
    public float horizontalAimingSpeed = 800f; 
    public float verticalAimingSpeed = 800f; 
    public float maxVerticalAngle = 80f;
    public float minVerticalAngle = -80f;

    private float _angleH ;
    private float _angleV ;
    public Transform cam;
    public float maxCamDist = 1;
    public LayerMask mask;
    public Vector3 smoothPlayerPos;
    
    void Start ()
    {
        //cam = transform;
        //smoothPlayerPos = player.position;
        
        Cursor.visible = false;
    }

    public Vector3 Crosshair()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        
        if (Physics.Raycast(ray, out hit)) 
        {
            crosshair = hit.point;
        }
        else
        {
            crosshair = ray.GetPoint(75);
        }

        return crosshair;
    }

    /*void LateUpdate () 
    {
        _angleH += Mathf.Clamp(Input.GetAxis("Mouse X") , -1, 1) * horizontalAimingSpeed * Time.deltaTime;
        _angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * verticalAimingSpeed * Time.deltaTime;
        _angleV = Mathf.Clamp(_angleV, minVerticalAngle, maxVerticalAngle);

        // Set aim rotation
        Quaternion aimRotation = Quaternion.Euler(-_angleV, _angleH, 0);
        Quaternion camYRotation = Quaternion.Euler(0, _angleH, 0);
        cam.rotation = aimRotation;
       
        // Find far and close position for the camera
        smoothPlayerPos = Vector3.Lerp(smoothPlayerPos, player.position, smoothingTime * Time.deltaTime);
        smoothPlayerPos.x = player.position.x;
        smoothPlayerPos.z = player.position.z;
        Vector3 farCamPoint = smoothPlayerPos + aimRotation * camOffset;
        Vector3 closeCamPoint = player.position + camYRotation * closeOffset;
        float farDist = Vector3.Distance(farCamPoint, closeCamPoint);
       
        // Smoothly increase maxCamDist up to the distance of farDist
        maxCamDist = Mathf.Lerp(maxCamDist, farDist, 5 * Time.deltaTime);
       
        // Make sure camera doesn't intersect geometry
        // Move camera towards closeOffset if ray back towards camera position intersects something
        RaycastHit hit;
        Vector3 closeToFarDir = (farCamPoint - closeCamPoint) / farDist;
        float padding = 0.3f;
        if (Physics.Raycast(closeCamPoint, closeToFarDir, out hit, maxCamDist + padding, mask)) 
        {
            maxCamDist = hit.distance - padding;
        }
        cam.position = closeCamPoint + closeToFarDir * maxCamDist;
    }*/
}