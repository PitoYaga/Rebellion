using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCard : MonoBehaviour
{
    private PlayerInteraction _player;

    void Start()
    {
        _player = FindObjectOfType<PlayerInteraction>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 120,0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            _player.securityCard++;
            Destroy(gameObject);
        }
    }
}
