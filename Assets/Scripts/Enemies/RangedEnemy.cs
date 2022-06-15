using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum RangedEnemyStates { Walk, Chase, Attack };

public class RangedEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float rangedEnemyMaxHealth = 10;
    [SerializeField] float rangedEnemyHealth = 10;
    [SerializeField] private float enemyRageXp = 20;
    //[SerializeField] private float walkSpeed = 3;
    //[SerializeField] private float chaseSpeed = 6;
    [SerializeField] private float knockBackPower = 5;

    [Header("States")]
    [SerializeField] private EnemyStates currentState;
    [SerializeField] private float decisionInterval = 1;
    private Collider[] _chaseCollider;
    private Collider[] _attackCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack")]
    //[SerializeField] private float enemyDamage = 5;
    //[SerializeField] private Transform attackRange;
    private float _timeSinceLastFire;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private Transform rangedEnemyBarrel;
    [SerializeField] private GameObject bullet;
    //[SerializeField] float currentChaseRadius = 20;
    //[SerializeField] private float chaseRadius = 20;
    [SerializeField] float attackRadius = 2;

    [Header("Loot")] 
    [SerializeField] private GameObject[] loots;
    [SerializeField] private int potionTreshold = 30;
    [SerializeField] private int shurikenTreshold = 30;
    
    [Header("Path Movement")]
    //[SerializeField] private Transform[] path;
    private int _currentIdx;
    //[SerializeField] private float stepSize = 1;

    [Header("Objects")]
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private StatsSaves _statsSaves;
    
    //[SerializeField] private AudioClip[] _audioClips;
    
    private float _timeSinceLastDecision;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    GameObject player;
    private Player _playerCs;
    private bool _isAlive = true;
    private AudioSource _audioSource;
    private bool gettingHit;


    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _playerCs = FindObjectOfType<Player>();
        currentState = EnemyStates.Attack;
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
            _timeSinceLastDecision += Time.deltaTime;
            if (_timeSinceLastDecision > decisionInterval)
            {
                /*if (FindObjectOfType<Player>().rageModeOn)
                {
                    currentChaseRadius = 100;
                }
                else
                {
                    currentChaseRadius = chaseRadius;
                }*/
            
                DoingSth();
                _timeSinceLastDecision = 0;
            }
        
            /*if (ReachedTarget())
            {
                IterateIndex();
            }*/
        }
    }

    void DoingSth()
    {
        switch (currentState)
        {
            //case EnemyStates.Walk:
                //MoveTowardsNextWp();
                //break;
            
            //case EnemyStates.Chase:
                //Chase();
                //break;
            
            case EnemyStates.Attack:
                RangedEnemyAttack();
                break;
            
            default:
                break;
        }
    }

    /*void MoveTowardsNextWp()
    {
        if (Physics.CheckSphere(transform.position, currentChaseRadius, playerLayer))
        {
            currentState = EnemyStates.Chase;
        }
        
        if (path != null)
        {
            //walking animation
            
            _audioSource.PlayOneShot(_audioClips[0]);
            _navMeshAgent.speed = walkSpeed;
            _navMeshAgent.SetDestination(path[_currentIdx].position);
            return;
        }
    }*/
    
    /*bool ReachedTarget()
    {
        float distance = Vector3.Distance(transform.position, path[_currentIdx].position);
        if (distance < 0.5f)
        {
            return true;
        }
        return false;
    }*/
    
    /*void IterateIndex()
    {
        _currentIdx++;
        _currentIdx %= path.Length;
    }*/
    
    /*void Chase()
    {
        _audioSource.PlayOneShot(_audioClips[0]);
        _navMeshAgent.speed = chaseSpeed;
        _navMeshAgent.SetDestination(_target.position);
        
        //animation will get more fast
        //maybe character's color can chance to red or sth like that

        if (!Physics.CheckSphere(transform.position, currentChaseRadius, playerLayer))
        {
            currentState = EnemyStates.Walk;
        }
        
        _attackCollider = Physics.OverlapCapsule(transform.position, attackRange.position, attackRadius, playerLayer);
        foreach (var hitCollider in _attackCollider)
        {
            currentState = EnemyStates.Attack;
            _navMeshAgent.speed = 0;
        }
    }*/

    void RangedEnemyAttack()
    {
        if (Physics.CheckSphere(transform.position, attackRadius, playerLayer))
        {
            transform.LookAt(player.transform.position);
            
            Ray ray = new Ray(rangedEnemyBarrel.position, rangedEnemyBarrel.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.CompareTag(Constants.playerTag))
                {
                    _timeSinceLastFire += Time.deltaTime;
                    if (_timeSinceLastFire > fireRate)
                    {
                        //_audioSource.PlayOneShot(_audioClips[1]);
                        transform.rotation = Quaternion.Inverse(transform.rotation);
                
                        _animator.SetTrigger("isAttacking");
                        Instantiate(bullet, rangedEnemyBarrel.position, Quaternion.identity);
                        _timeSinceLastFire = 0;
                    }
                }
            }
        }

        //_attackCollider = null;
        
        /*if (!Physics.CheckSphere(transform.position, attackRadius, playerLayer))
        {
            //_navMeshAgent.speed = chaseSpeed;
            //currentState = EnemyStates.Chase;
        }*/
    }

    public void RangedEnemyGetHit(float playerDamage)
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
        
        _animator.SetTrigger("getHit");
        _navMeshAgent.velocity = -transform.forward * knockBackPower;
        rangedEnemyHealth -= playerDamage;
        enemyHealthSlider.value = rangedEnemyHealth;
        
        if (rangedEnemyHealth <= 0 && _isAlive)
        {
            RangedEnemyDeath();
        }
    }
    
    void RangedEnemyDeath()
    {
        Vector3 lootPosOffset = new Vector3(0, 10, 0);
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

        Destroy(gameObject, 1);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == (Constants.shurikenTag))
        {
            RangedEnemyGetHit(FindObjectOfType<Shuriken>().shurikenDamage);
        }
    }


    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentChaseRadius);*/
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        
        Gizmos.color = Color.yellow;
        Vector3 direction = rangedEnemyBarrel.TransformDirection(Vector3.forward) * attackRadius;
        Gizmos.DrawRay(rangedEnemyBarrel.position, direction);
    }
}
