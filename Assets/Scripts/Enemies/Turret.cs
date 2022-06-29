using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float turretMaxHealth = 10;
    [SerializeField] float turretHealth = 10;
    [SerializeField] private float enemyRageXp = 20;
    [SerializeField] private ParticleSystem deathVFX;
    
    [Header("Attack")]
    [SerializeField] private float fireRate = 3;
    [SerializeField] private Transform turretBarrel;
    [SerializeField] private GameObject bullet;
    [SerializeField] float attackRadius = 2;
    [SerializeField] private ParticleSystem shootVFX;

    [Header("Loot")] 
    [SerializeField] private GameObject[] loots;
    [SerializeField] private int potionTreshold = 30;
    [SerializeField] private int shurikenTreshold = 30;

    [Header("Objects")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private StatsSaves _statsSaves;
    
    //[SerializeField] private AudioClip[] _audioClips;
    
    private float _timeSinceLastFire;
    private Animator _animator;
    GameObject _player;
    private Player _playerCs;
    private bool _isAlive = true;
    private AudioSource _audioSource;
    private bool gettingHit;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _playerCs = FindObjectOfType<Player>();
        shootVFX.Stop();
        deathVFX.Stop();
    }

    void Start()
    {
        _player = GameObject.FindWithTag(Constants.bulletTargetTag);

        enemyHealthSlider.maxValue = turretMaxHealth;
        enemyHealthSlider.value = turretHealth;
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
            _animator.enabled = false;
            transform.LookAt(_player.transform.position);
            
            Ray ray = new Ray(turretBarrel.position, turretBarrel.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.CompareTag(Constants.playerTag))
                {
                    _timeSinceLastFire += Time.deltaTime;
                    if (_timeSinceLastFire > fireRate)
                    {
                        _animator.enabled = false;
                        _audioSource.PlayOneShot(_audioClips[0]);
                        //_animator.SetTrigger("isAttacking");
                        
                        Instantiate(shootVFX, turretBarrel.position, Quaternion.Euler(turretBarrel.rotation.eulerAngles));
                        Instantiate(bullet, turretBarrel.position, Quaternion.identity);
                        _timeSinceLastFire = 0;
                    }
                }
            }
        }
        else
        {
            _animator.enabled = true;
        }
    }

    /*void FireBullet()
    {
        Instantiate(bullet, turretBarrel.position, Quaternion.identity);
    }*/

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
        
        _audioSource.PlayOneShot(_audioClips[1]);
        
        turretHealth -= playerDamage;
        enemyHealthSlider.value = turretHealth;
        
        if (turretHealth <= 0 && _isAlive)
        {
            TurretDeath();
        }
    }
    
    void TurretDeath()
    {
        StartCoroutine("DeathVFX");
        Vector3 lootPosOffset = new Vector3(0, 0, 8);
        _isAlive = false;
        _playerCs.rageBar += enemyRageXp;
        _animator.enabled = true;
        _animator.SetTrigger("death");
        
        if(UnityEngine.Random.Range(1 , 100) <= shurikenTreshold)
        {
            Instantiate(loots[1], transform.position + lootPosOffset, Quaternion.identity);
        }
        else if(UnityEngine.Random.Range(1 , 100) <= potionTreshold)
        {
            Instantiate(loots[0], transform.position + lootPosOffset, Quaternion.identity);
        }
        
        //_audioSource.PlayOneShot(_audioClips[2]);
    }
    
    IEnumerator DeathVFX()
    {
        deathVFX.Play();
        yield return new WaitForSeconds(0.5f);
        deathVFX.Stop();
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
        Vector3 direction = turretBarrel.forward * attackRadius;
        Gizmos.DrawRay(turretBarrel.position, direction);
    }
}
