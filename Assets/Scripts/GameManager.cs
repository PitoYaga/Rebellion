using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int startingSceneIndex;

    public int playerHealth;
    public int playerRageBar = 0;
    public int shurikenCount = 15;
    

    private void Awake()
    {
        int numScenePersist = FindObjectsOfType<GameManager>().Length;
        if (numScenePersist > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        
    }
}
