using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyStates { Walk, Chase, Attack };

public class MeleeEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float meleeEnemyMaxHealth = 10;
    [SerializeField] float meleeEnemyHealth = 10;
    [SerializeField] private float enemyRageXp = 20;
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float chaseSpeed = 6;
    [SerializeField] private float attackSpeed = 1;
    [SerializeField] private float knockBackPower;

    [Header("States")]
    [SerializeField] private EnemyStates currentState;
    [SerializeField] private float decisionInterval = 1;
    private Collider[] _chaseCollider;
    private Collider[] _attackCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack")]
    [SerializeField] private float enemyDamage = 5;
    [SerializeField] private Transform enemyAttackArea;
    [SerializeField] float currentChaseRadius = 5;
    [SerializeField] float chaseRadius = 5;
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
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;
    
    private float _timeSinceLastDecision;
    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    private Player _playerCs;
    private Rigidbody _rigidbody;
    private bool isAlive = true;
    private Animator _animator;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerCs = FindObjectOfType<Player>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _target = GameObject.FindGameObjectWithTag(Constants.playerTag).transform;
        
        currentState = EnemyStates.Walk;
        enemyHealthSlider.maxValue = meleeEnemyMaxHealth;
        enemyHealthSlider.value = meleeEnemyHealth;
    }
    
    void Update()
    {
        if (isAlive)
        {
            _timeSinceLastDecision += Time.deltaTime;
            if (_timeSinceLastDecision > decisionInterval)
            {
                DoingSth();
                _timeSinceLastDecision = 0;
            }
        
            if (ReachedTarget())
            {
                IterateIndex();
            }
        
            if (FindObjectOfType<Player>().rageModeOn)
            {
                currentChaseRadius = 100;
            }
            else
            {
                currentChaseRadius = chaseRadius;
            }
        }
    }

    void DoingSth()
    {
        switch (currentState)
        {
            case EnemyStates.Walk:
                MoveTowardsNextWp();
                break;
            
            case EnemyStates.Chase:
                Chase();
                break;
            
            case EnemyStates.Attack:
                MeleeEnemyAttack();
                break;
            
            default:
                break;
        }
    }

    void MoveTowardsNextWp()
    {
        if (Physics.CheckSphere(transform.position, currentChaseRadius, playerLayer))
        {
            currentState = EnemyStates.Chase;
        }
        
        if (path != null)
        {
            //walking animation
            //_audioSource.PlayOneShot(_audioClips[0]);
            
            _animator.SetTrigger("isWalking");
            _navMeshAgent.speed = walkSpeed;
            _navMeshAgent.SetDestination(path[_currentIdx].position);
            return;
        }
    }
    
    bool ReachedTarget()
    {
        float distance = Vector3.Distance(transform.position, path[_currentIdx].position);
        if (distance < 0.5f)
        {
            return true;
        }
        return false;
    }
    
    void IterateIndex()
    {
        _currentIdx++;
        _currentIdx %= path.Length;
    }
    
    void Chase()
    {
        _animator.SetTrigger("alerted");
        _navMeshAgent.speed = chaseSpeed;
        _navMeshAgent.SetDestination(_target.position);

        //_audioSource.PlayOneShot(_audioClips[0]);
        
        //animation will get more fast
        //maybe character's color can chance to red or sth like that

        if (!Physics.CheckSphere(transform.position, currentChaseRadius, playerLayer))
        {
            currentState = EnemyStates.Walk;
        }
        
        _attackCollider = Physics.OverlapSphere(enemyAttackArea.position, attackRadius, playerLayer);
        foreach (var hitCollider in _attackCollider)
        {
            currentState = EnemyStates.Attack;
        }
    }

    void MeleeEnemyAttack()
    {
        if (Physics.CheckSphere(transform.position, attackRadius, playerLayer))
        {
            _animator.SetTrigger("isAttacking");
            //_audioSource.PlayOneShot(_audioClips[1]);
            _navMeshAgent.velocity = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            _navMeshAgent.speed = attackSpeed;
            _playerCs.PlayerGetHit(enemyDamage);
        }
        _attackCollider = null;
        
        if (!Physics.CheckSphere(transform.position, attackRadius, playerLayer))
        {
            _animator.SetTrigger("isWalking");
            currentState = EnemyStates.Chase;
        }

    }

    public void MeleeEnemyGetHit(float playerDamage)
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
        
        _navMeshAgent.velocity = -transform.forward * knockBackPower;
        _animator.SetTrigger("knockback");

        meleeEnemyHealth -= playerDamage;
        enemyHealthSlider.value = meleeEnemyHealth;
        
        if (meleeEnemyHealth <= 0 && isAlive)
        {
            MeleeEnemyDeath();
        }
    }
    
    void MeleeEnemyDeath()
    {
        isAlive = false;
        _playerCs.rageBar += enemyRageXp;
        _animator.SetTrigger("death");
        
        if(UnityEngine.Random.Range(1 , 100) <= potionTreshold)
        {
            Instantiate(loots[0], transform.position, Quaternion.identity);
        } 
        if(UnityEngine.Random.Range(1 , 100) <= shurikenTreshold)
        {
            Instantiate(loots[1], transform.position, Quaternion.identity);
        }
        
        //_audioSource.PlayOneShot(_audioClips[3]);

        Destroy(gameObject, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.doorTag))
        {
             GetComponent<MoveToCollidor>().enabled = true;
             this.enabled = false;
        }
        if (other.CompareTag(Constants.collidorTag))
        {
             GetComponent<MoveToCollidor>().enabled = false;
             this.enabled = true;
        }
    }

    
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentChaseRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyAttackArea.position, attackRadius);
    }
}
