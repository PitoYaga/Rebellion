using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToCollidor : MonoBehaviour
{
    [SerializeField] private Transform collidor;
    
    public bool moveToCollidor;
    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (moveToCollidor)
        {
            GameObject.Find("MeleeEnemy").GetComponent<MeleeEnemy>().enabled = false;
            _navMeshAgent.SetDestination(collidor.position);
        }
        else
        {
            GameObject.Find("MeleeEnemy").GetComponent<MeleeEnemy>().enabled = true;
            this.enabled = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.doorTag))
        {
            moveToCollidor = true;
        }
        else if (other.CompareTag(Constants.collidorTag))
        {
            moveToCollidor = false;
        }
    }
}
