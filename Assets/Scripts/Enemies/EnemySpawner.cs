using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private Transform[] spawnPoses;

        private int _enemyType;
        private int _spawnPos;
        private GameObject _enemy;
        
        void EnemySpawn()
        {
            for (int i = 0; i < spawnPoses.Length * 1.5f; i++)
            {
                _spawnPos = Random.Range(0, spawnPoses.Length);
                _enemyType = Random.Range(0, 2);

                _enemy = Instantiate(enemies[_enemyType], spawnPoses[_spawnPos]);
                _enemy.transform.position = spawnPoses[_spawnPos].position;
            
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
}
