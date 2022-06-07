using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            int _sceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(_sceneIndex + 1);
        }
    }
}
