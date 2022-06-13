using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class EnemySlider : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(GameObject.FindWithTag(Constants.cameraTag).transform.position);
    }
}
