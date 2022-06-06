using UnityEngine;

namespace Enemies
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] float bulletSpeed = 50;
        [SerializeField] private float bulletDamage = 5;
        [SerializeField] private Transform barrel;
        [SerializeField] private Transform target;
    
        private Rigidbody _rigidbody;
    
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            transform.LookAt(target.position);
            _rigidbody.AddForce(barrel.forward * bulletSpeed, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag(Constants.playerTag))
            {
                FindObjectOfType<Player>().PlayerGetHit(bulletDamage);
                Destroy(gameObject);
            }
        }

    
        //Rage mode
        //Alan içindeki düşmanları algıla.
        //Mermiyi seçili olan düşmana döndür.
        //Ateş edildiğinde aimsiz düşman doğrultusunda ateşlesin.
    }
}