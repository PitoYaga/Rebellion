using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class LaserWall : MonoBehaviour
{
    [SerializeField] private float laserWallDamage = 1;
    [SerializeField] private AudioClip[] audioClips;

    private AudioSource _audioSource;
    private AudioClip _currentClip;
    private Player _playerCs;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _currentClip = audioClips[0];
        _playerCs = FindObjectOfType<Player>();
    }

    private void Update()
    {
        PlayMusic();
    }

    void PlayMusic()
    {
        if (_audioSource.isPlaying && _audioSource.clip == _currentClip)
        {
            
        }
        else
        {
            _audioSource.clip = _currentClip;
            _audioSource.Play();
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Constants.playerTag) && _playerCs.currentSpeed < _playerCs.dashSpeed)
        {
            _playerCs.PlayerGetHit(laserWallDamage);
            _currentClip = audioClips[1];
        }
    }
    

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            _currentClip = audioClips[0];
        }
        else if (other.CompareTag(Constants.meleeEnemyTag))
        {
            _currentClip = audioClips[0];
        }
    }
}
