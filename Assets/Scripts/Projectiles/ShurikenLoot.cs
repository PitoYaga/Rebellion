using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenLoot : MonoBehaviour
{
    [SerializeField] private StatsSaves _statsSaves;
    
    private void Update()
    {
        transform.Rotate(new Vector3(120, 0, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            _statsSaves.ShurikenVar++;
            Destroy(gameObject);
        }
    }
}
