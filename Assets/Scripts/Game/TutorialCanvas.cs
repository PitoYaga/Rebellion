using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCanvas : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(GameObject.FindWithTag(Constants.cameraTag).transform.position);
    }
}
