using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private int coreHealth = 3;
    [SerializeField] private ParticleSystem explodeVFX;
    public int destroyedCore = 0;
    
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        explodeVFX.Stop();
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
        if (other.collider.CompareTag(Constants.playerSwordTag))
        {
            coreHealth--;
            if (coreHealth <= 0)
            {
                destroyedCore++;
                StartCoroutine("Boom");
                Destroy(gameObject, 1);
            }
        }

        if (other.collider.CompareTag(Constants.shurikenTag))
        {
            coreHealth--;
            if (coreHealth <= 0)
            {
                destroyedCore++;
                StartCoroutine("Boom");
                Destroy(gameObject, 1);
            }
        }
    }
}
