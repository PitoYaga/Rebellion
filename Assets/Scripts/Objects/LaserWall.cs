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
    private Player _playerCS;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _currentClip = audioClips[0];
        _playerCS = FindObjectOfType<Player>();
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
        if (other.CompareTag(Constants.playerTag) && _playerCS._currentSpeed < _playerCS.dashSpeed)
        {
            _playerCS.PlayerGetHit(laserWallDamage);
            _currentClip = audioClips[1];
        }
        
        if (other.CompareTag(Constants.meleeEnemyTag))
        {
            Debug.Log("enemy in wall");
            MeleeEnemy meleeEnemy = GetComponent<MeleeEnemy>();
            meleeEnemy.walkSpeed /= 2;
            meleeEnemy.chaseSpeed /= 2;
            meleeEnemy.meleeAttackSpeed /= 2;
            meleeEnemy.chaseRadius -= 15;
            _currentClip = audioClips[1];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
      
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
