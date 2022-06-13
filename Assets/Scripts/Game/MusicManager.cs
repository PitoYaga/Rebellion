using UnityEngine;
using UnityEngine.SceneManagement;

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

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            _sceneIndex = SceneManager.GetActiveScene().buildIndex;

            Debug.Log(_currentClip);
            Debug.Log(_audioSource.isPlaying);
        
            SetCurrentClip();
            PlayMusic();
        }

        void SetCurrentClip()
        {
            if (_sceneIndex == 0 || _sceneIndex == 1 || _sceneIndex == 3)
            {
                StopMusic();
                _currentClip = menuMusic;
                PlayMusic();
            }
            else if (_sceneIndex == 2 || _sceneIndex == 4 || _sceneIndex == 5)
            {
                _currentClip = gameMusics[0];
            }
            else
            {
                _currentClip = psychologistMusic;
            }
        }
    
        void PlayMusic()
        {
            if (_audioSource.isPlaying)
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
