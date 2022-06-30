using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private int coreHealth = 3;
    [SerializeField] private ParticleSystem explodeVFX;
    [SerializeField] private AudioClip[] _audioClips;
    public int destroyedCore = 0;
    
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private SecurityLaser _securityLaser;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        explodeVFX.Stop();
        _audioSource.PlayOneShot(_audioClips[0]);
        _securityLaser = FindObjectOfType<SecurityLaser>();
    }
    
    void Update()
    {
      
    }

    IEnumerator Boom()
    {
        explodeVFX.Play();
        yield return new WaitForSeconds(0.5f);
        explodeVFX.Stop();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Constants.shurikenTag))
        {
            coreHealth--;
            if (coreHealth == 0)
            {
                _securityLaser.destroyedCore++;
                StartCoroutine("Boom");
                Destroy(gameObject, 1);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerSwordTag))
        {
            coreHealth--;
            if (coreHealth == 0)
            {
                _securityLaser.destroyedCore++;
                StartCoroutine("Boom");
                Destroy(gameObject, 1);
            }
        }
    }
}
