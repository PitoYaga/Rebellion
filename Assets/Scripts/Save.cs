using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    private float numCoins = 0;
    
    void Start()
    {
        numCoins = PlayerPrefs.GetFloat("Coins", 0);
        Debug.Log("coins =" + numCoins);
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            numCoins++;
            PlayerPrefs.SetFloat("Coins", numCoins);
            PlayerPrefs.Save();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            numCoins = 0; 
        }
    }
}
