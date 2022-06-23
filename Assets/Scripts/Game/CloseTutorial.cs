using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseTutorial : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            Destroy(canvas);
            Destroy(gameObject);
        }
    }
}
