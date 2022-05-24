using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlider : MonoBehaviour
{
    [SerializeField] private Transform lookAtPlayer;
    
    void Update()
    {
        transform.LookAt(lookAtPlayer.position);
    }
}
