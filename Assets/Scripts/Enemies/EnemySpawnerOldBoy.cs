using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerOldBoy : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    
    [SerializeField] int minTimeBetweenSpawn;
    [SerializeField] int maxTimeBetweenSpawn;
    private int _timeBetweenSpawn;
    
    [SerializeField] private int minEnemyCount;
    [SerializeField] private int maxEnemyCount;
    private int _enemyCount;
    
    float _x;
    private float _timer;
    private float _waveCount;
    Vector3 _enemySpawn;


    void Start()
    {
        
    }
    
    void Update()
    {
        _timeBetweenSpawn = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn);
        _enemyCount = Random.Range(minEnemyCount, maxEnemyCount);
        
        _timer += Time.deltaTime;
        if (_timer >= _timeBetweenSpawn)
        {
            if (_waveCount <= 2)
            {
                OldBoyEnemySpawner();
                _timer = 0;
                _waveCount++;
            }
        }
    }

    void OldBoyEnemySpawner()
    {
        for (int i = 0; i < _enemyCount; i++)
        { 
            _x = Random.Range(-6, 6);
            _enemySpawn = new Vector3(transform.position.x + _x, transform.position.y, transform.position.z);
            enemy = Instantiate(enemy, _enemySpawn, Quaternion.identity);
        }
    }
}