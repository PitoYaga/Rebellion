using System;
using UnityEngine;

namespace Objects
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private float doorSpeed = 1;
      

        private Vector3 _openPosition;
        private Vector3 _closePosition;
        bool _isOpen;

        void Start()
        {
            _closePosition = transform.position;
            _openPosition = transform.position;
            _openPosition.x -= 5;
        }

    
        void Update()
        {
            Open();
        }

        void Open()
        {
            if (_isOpen)
            {
                transform.position = Vector3.MoveTowards(transform.position, _openPosition, doorSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _closePosition, doorSpeed * Time.deltaTime);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(Constants.meleeEnemyTag) || other.CompareTag(Constants.playerTag))
            {
                _isOpen = true;
                Debug.Log("opening");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag(Constants.meleeEnemyTag) || other.CompareTag(Constants.playerTag)) 
            {
                _isOpen = false;
                Debug.Log("closing");
            }
        }
    }
}
