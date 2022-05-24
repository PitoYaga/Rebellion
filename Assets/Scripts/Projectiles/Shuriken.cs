using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 50;
    [SerializeField] private float shurikenDamage = 5;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform target;
    
    private Rigidbody _rigidbody;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        Vector3 direction = FindObjectOfType<CameraTry>().Crosshair() - barrel.position;
        transform.forward = direction;
        _rigidbody.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Constants.meleeEnemyTag))
        {
            FindObjectOfType<MeleeEnemy>().MeleeEnemyGetHit(shurikenDamage);
            Destroy(gameObject);
        }
        if (other.collider.CompareTag(Constants.rangedEnemyTag))
        {
            FindObjectOfType<RangedEnemy>().RangedEnemyGetHit(shurikenDamage);
            Destroy(gameObject);
        }
    }
    
    //Rage mode
    //Alan içindeki düşmanları algıla.
    //Mermiyi seçili olan düşmana döndür.
    //Ateş edildiğinde aimsiz düşman doğrultusunda ateşlesin.
}
