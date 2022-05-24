using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCanvas : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] private GameObject close;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            canvas.SetActive(true);
            //close.SetActive(false);
            Time.timeScale = 0;
            FindObjectOfType<Player>().isAlive = false;
            Destroy(gameObject);
            
        }
    }
}
