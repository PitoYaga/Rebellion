using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Game
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] gameMusics;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip psychologistMusic;
    
        private AudioSource _audioSource;
        private AudioClip _currentClip;
        private int _sceneIndex;
        private int randomMusic;

        private void Awake()
        {
            if (_audioSource != null)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            _sceneIndex = SceneManager.GetActiveScene().buildIndex;
            randomMusic = Random.Range(0, gameMusics.Length);

            SetCurrentClip();
            PlayMusic();
        }

        void SetCurrentClip()
        {
            if (_sceneIndex == 0 || _sceneIndex == 1 || _sceneIndex == 2)
            {
                _currentClip = menuMusic;
            }
            else if (_sceneIndex == 3)
            {
                _currentClip = gameMusics[randomMusic];
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
