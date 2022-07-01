using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenLoot : MonoBehaviour
{
    [SerializeField] private StatsSaves _statsSaves;
    [SerializeField] Transform ancherPoint;
    
    private void Update()
    {
        ancherPoint.Rotate(new Vector3(0, 120, 0) * Time.deltaTime);
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
