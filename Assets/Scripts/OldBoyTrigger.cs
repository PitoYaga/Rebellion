using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OldBoyTrigger : MonoBehaviour
{
    public bool oldboy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            //GameObject.Find("Main Camera").GetComponent<CameraTry>().enabled = false;
            //GameObject.Find("Main Camera").GetComponent<OldBoyCamera>().enabled = true;
            oldboy = false;
            Destroy(gameObject);
            //SceneManager.LoadScene("Next Scene");
        }
    }
}
