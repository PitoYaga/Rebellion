using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Game
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] gameMusics;
        [SerializeField] private AudioClip actionMusic;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip psychologistMusic;
    
        private AudioSource _audioSource;
        private AudioClip _currentClip;
        private int _sceneIndex;
        private int _randomMusic;
        private MeleeEnemy[] _meleeEnemyCS;

        private void Awake()
        {
            int musicManagerCount = FindObjectsOfType<MusicManager>().Length;
            if (musicManagerCount > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            _sceneIndex = SceneManager.GetActiveScene().buildIndex;
            _meleeEnemyCS = FindObjectsOfType<MeleeEnemy>();

            SetCurrentClip();
            PlayMusic();
        }

        void SetCurrentClip()
        {
            if (_sceneIndex == 0 || _sceneIndex == 1 || _sceneIndex == 2 || _sceneIndex == 6)
            {
                _currentClip = menuMusic;
            }
            else if (_sceneIndex == 4)
            {
                for (int i = 0; i < _meleeEnemyCS.Length; i++)
                {
                    if (_meleeEnemyCS[i].isChasing)
                    {
                        _currentClip = actionMusic;
                    }
                    else
                    {
                         _currentClip = gameMusics[_randomMusic];
                    }
                }
            }
            else
            {
                _currentClip = psychologistMusic;
            }
        }
    
        void PlayMusic()
        {
            if (_audioSource.isPlaying && _audioSource.clip == _currentClip)
            {
                return;
            }
            else
            {
                _randomMusic = Random.Range(0, gameMusics.Length);
                _audioSource.clip = _currentClip;
                _audioSource.Play();
            }
        }
 
        void StopMusic()
        {
            _audioSource.Stop();
        }
    }
}
