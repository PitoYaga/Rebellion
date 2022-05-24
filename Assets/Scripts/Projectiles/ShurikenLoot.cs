using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenLoot : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(120, 0, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.playerTag))
        {
            FindObjectOfType<Player>().shurikenMagazine++;
            Destroy(gameObject);
        }
    }
}
