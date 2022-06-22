using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float rangedEnemyMaxHealth = 10;
    [SerializeField] float rangedEnemyHealth = 10;
    [SerializeField] private float enemyRageXp = 20;

    [Header("Attack")]
    [SerializeField] private float fireRate = 3;
    [SerializeField] private Transform rangedEnemyBarrel;
    [SerializeField] private GameObject bullet;
    [SerializeField] float attackRadius = 2;

    [Header("Loot")] 
    [SerializeField] private GameObject[] loots;
    [SerializeField] private int potionTreshold = 30;
    [SerializeField] private int shurikenTreshold = 30;

    [Header("Objects")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private StatsSaves _statsSaves;
    
    //[SerializeField] private AudioClip[] _audioClips;
    
    private float _timeSinceLastFire;
    private Animator _animator;
    GameObject player;
    private Player _playerCs;
    private bool _isAlive = true;
    private AudioSource _audioSource;
    private bool gettingHit;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _playerCs = FindObjectOfType<Player>();
    }

    void Start()
    {
        player = GameObject.FindWithTag(Constants.playerTag);

        enemyHealthSlider.maxValue = rangedEnemyMaxHealth;
        enemyHealthSlider.value = rangedEnemyHealth;
    }
    
    void Update()
    {
        if (_isAlive)
        {
            TurretAttack();
        }
    }
    
    void TurretAttack()
    {
        if (Physics.CheckSphere(transform.position, attackRadius, playerLayer))
        {
            
            transform.LookAt(player.transform.position);

            Ray ray = new Ray(rangedEnemyBarrel.position, rangedEnemyBarrel.forward * -1);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, attackRadius))
            {
                if (hit.collider.CompareTag(Constants.playerTag))
                {
                    Debug.Log("turret");
                    _timeSinceLastFire += Time.deltaTime;
                    if (_timeSinceLastFire > fireRate)
                    {
                        //_audioSource.PlayOneShot(_audioClips[1]);
                        transform.rotation = Quaternion.Inverse(transform.rotation);
                
                        _animator.SetTrigger("isAttacking");
                        _timeSinceLastFire = 0;
                    }
                }
            }
        }
    }

    public void FireBullet()
    {
        Instantiate(bullet, rangedEnemyBarrel.position, Quaternion.identity);
    }

    public void TurretGetHit(float playerDamage)
    {
        if (_playerCs.rageModeOn)
        {
            _statsSaves.HealthVar += playerDamage / 2;
            if ( _statsSaves.HealthVar >= _playerCs.playerMaxHealth)
            {
                _statsSaves.HealthVar = _playerCs.playerMaxHealth;
            }
        }
        
        //_audioSource.PlayOneShot(_audioClips[2]);
        
        rangedEnemyHealth -= playerDamage;
        enemyHealthSlider.value = rangedEnemyHealth;
        
        if (rangedEnemyHealth <= 0 && _isAlive)
        {
            TurretDeath();
        }
    }
    
    void TurretDeath()
    {
        Vector3 lootPosOffset = new Vector3(0, 0, 7);
        _isAlive = false;
        _playerCs.rageBar += enemyRageXp;
        _animator.SetTrigger("death");
        
        if(UnityEngine.Random.Range(1 , 100) <= shurikenTreshold)
        {
            Instantiate(loots[1], transform.position + lootPosOffset, Quaternion.identity);
        }
        else if(UnityEngine.Random.Range(1 , 100) <= potionTreshold)
        {
            Instantiate(loots[0], transform.position + lootPosOffset, Quaternion.identity);
        }
        
        //_audioSource.PlayOneShot(_audioClips[3]);

        //Destroy(gameObject, 2);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == (Constants.shurikenTag))
        {
            TurretGetHit(FindObjectOfType<Shuriken>().shurikenDamage);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        
        Gizmos.color = Color.yellow;
        Vector3 direction = transform.forward * attackRadius;
        Gizmos.DrawRay(rangedEnemyBarrel.position, direction);
    }
}
