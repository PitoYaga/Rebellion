using System;
using UnityEngine;
using UnityEngine.UI;

namespace Projectiles
{
    public class HealthPosion : MonoBehaviour
    {
        [SerializeField] private float healthPosion = 20;
        [SerializeField] private Transform rotateAncher;
        private Slider playerSlider;


        private void Start()
        {
            playerSlider = FindObjectOfType<Player>().playerHeathSlider;
        }

        private void Update()
        {
            rotateAncher.Rotate(new Vector3(0, 120,0) * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == Constants.playerTag)
            {
                if (FindObjectOfType<Player>().playerHealth >= 80)
                {
                    FindObjectOfType<Player>().playerHealth = 100;
                }
                else
                {
                    FindObjectOfType<Player>().playerHealth += healthPosion;
                }
                playerSlider.value = FindObjectOfType<Player>().playerHealth;
                Destroy(gameObject);
            }
        }
    }
}
