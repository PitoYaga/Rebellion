using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 50;
    public float shurikenDamage = 5;
    [SerializeField] private ParticleSystem laserBlast;

    private Rigidbody _rigidbody;
    private CameraTry _cameraTry;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _cameraTry = FindObjectOfType<CameraTry>();
        laserBlast.Stop();
        
        transform.LookAt(_cameraTry.crosshair);
        _rigidbody.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        
        Destroy(gameObject, 3);
    }

    private void OnCollisionEnter(Collision other)
    {
        transform.position = transform.position;
        laserBlast.Play();
        Destroy(gameObject);
    }
}
