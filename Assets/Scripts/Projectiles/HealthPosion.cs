using System;
using UnityEngine;
using UnityEngine.UI;

namespace Projectiles
{
    public class HealthPosion : MonoBehaviour
    {
        [SerializeField] private float healthPosion = 20;
        private Slider playerSlider;

        [SerializeField] private StatsSaves _statsSaves;


        private void Start()
        {
            playerSlider = FindObjectOfType<Player>().playerHeathSlider;
        }

        private void Update()
        {
            transform.Rotate(new Vector3(120, 0,0) * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == Constants.playerTag)
            {
                if (_statsSaves.HealthVar >= 80)
                {
                    _statsSaves.HealthVar = 100;
                }
                else
                {
                    _statsSaves.HealthVar += healthPosion;
                }

                playerSlider.value = _statsSaves.HealthVar;
                Destroy(gameObject);
            }
        }
    }
}
