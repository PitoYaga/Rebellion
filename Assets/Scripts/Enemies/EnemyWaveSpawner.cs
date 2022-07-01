using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyWave;

    private void OnTriggerEnter(Collider other)
    {
        enemyWave.SetActive(true);
    }
}
