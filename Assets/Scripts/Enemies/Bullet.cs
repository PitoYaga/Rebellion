using UnityEngine;

namespace Enemies
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] float bulletSpeed = 50;
        [SerializeField] private float bulletDamage = 5;
        
        private Rigidbody _rigidbody;
        private GameObject _target;
    
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _target = GameObject.FindWithTag(Constants.bulletTargetTag);
            
            transform.LookAt(_target.transform.position);
            _rigidbody.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
            
            Destroy(gameObject, 3);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag(Constants.playerTag))
            {
                FindObjectOfType<Player>().PlayerGetHit(bulletDamage);
                Destroy(gameObject);
            }
        }
    }
}