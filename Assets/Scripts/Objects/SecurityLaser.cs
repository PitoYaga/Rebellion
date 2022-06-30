using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityLaser : MonoBehaviour
{
    [SerializeField] private float barrierDamage = 1;
    [SerializeField] private AudioClip[] _audioClips;
    public int destroyedCore = 0;

    private AudioSource _audioSource;
    private AudioClip _currentClip;
    Player _playerCs;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerCs = FindObjectOfType<Player>();
        _currentClip = _audioClips[0];
    }

    private void Update()
    {
        DestroyLaser();
        PlaySound();
    }
    
    void PlaySound()
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

    void DestroyLaser()
    {
        if (destroyedCore == 2)
        {
            //_currentClip = _audioClips[2];
            Destroy(gameObject, 2);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Constants.playerTag))
        {
            _playerCs.PlayerGetHit(barrierDamage);
            _currentClip = _audioClips[1];
        }
    }
}
