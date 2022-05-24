using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OldBoyCamera : MonoBehaviour
{
    private OldBoyTrigger _oldBoyTrigger;
    private Camera _camera;

    [SerializeField] private Transform target;
    [SerializeField] private float yClamp;
    [SerializeField] private Vector3 offset;

    void Start()
    {
        
    }

    
    void Update()
    {
        OldBoyOn();
    }

    void OldBoyOn()
    {
        if (FindObjectOfType<Player>().oldboy)
        {
            transform.position = new Vector3(target.position.x, yClamp, target.position.z);
            transform.position += offset;
            transform.LookAt(target);
        }
        else
        {
            GameObject.Find("Main Camera").GetComponent<CameraTry>().enabled = true;
        }
    }
}
