using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform[] spawnPoses;

    private int enemyType;
    private int spawnPos;
    
    void Start()
    {
       
    }
    
    void Update()
    {
        
    }

    void EnemySpawn()
    {
        for (int i = 0; i < spawnPoses.Length * 1.5f; i++)
        {
            enemyType = Random.Range(0, 1);
            spawnPos = Random.Range(0, spawnPoses.Length);
            
            Instantiate(enemies[enemyType], spawnPoses[spawnPos]);

            if (i == spawnPoses.Length * 1.5f)
            {
                Destroy(gameObject);
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            EnemySpawn();
        }
    }
}
