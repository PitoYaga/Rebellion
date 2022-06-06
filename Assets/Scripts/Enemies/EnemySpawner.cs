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
    private GameObject enemy;
    
    void Start()
    {
       
    }
    
    void Update()
    {
        enemyType = Random.Range(0, 2);
        Debug.Log(enemyType);
    }

    void EnemySpawn()
    {
        
        for (int i = 0; i < spawnPoses.Length * 1.5f; i++)
        {
            spawnPos = Random.Range(0, spawnPoses.Length);

            enemy = Instantiate(enemies[Random.Range(0, enemies.Length - 1)], spawnPoses[spawnPos]);
            enemy.transform.position = spawnPoses[spawnPos].position;

            
            if (i == (spawnPoses.Length * 1.5f) - 1)
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
