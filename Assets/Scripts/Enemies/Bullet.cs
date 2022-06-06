using UnityEngine;

namespace Enemies
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] float bulletSpeed = 50;
        [SerializeField] private float bulletDamage = 5;
        [SerializeField] private Transform target;
        
        private Rigidbody _rigidbody;
    
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            transform.LookAt(target.position);
            _rigidbody.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
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