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
    [SerializeField] private Transform[] path;
    private int _currentIdx;
    //[SerializeField] private float stepSize = 1;

    [Header("Objects")]
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] GameObject player;
    [SerializeField] private AudioClip[] _audioClips;
    
    private float _timeSinceLastDecision;
    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    private Player _playerCs;
    private Rigidbody _rigidbody;
    private bool _isAlive = true;
    private AudioSource _audioSource;
    private Animator _animator;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _playerCs = FindObjectOfType<Player>();
        _target = GameObject.FindGameObjectWithTag(Constants.playerTag).transform;
        currentState = EnemyStates.Attack;
        
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
            _playerCs.playerHealth += playerDamage / 2;
            if (_playerCs.playerHealth >= _playerCs.playerMaxHealth)
            {
                _playerCs.playerHealth = _playerCs.playerMaxHealth;
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
        _isAlive = false;
        _playerCs.rageBar += enemyRageXp;
        
        if(UnityEngine.Random.Range(1 , 100) <= shurikenTreshold)
        {
            Instantiate(loots[1], transform.position, Quaternion.identity);
        }
        if(UnityEngine.Random.Range(1 , 100) <= potionTreshold)
        {
            Instantiate(loots[0], transform.position, Quaternion.identity);
        }
        
        //_audioSource.PlayOneShot(_audioClips[3]);
        _animator.SetTrigger("death");
        
        Destroy(gameObject, 1);
    }
    
    
    
    
    
    
    
    
    
    
    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentChaseRadius);*/
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
