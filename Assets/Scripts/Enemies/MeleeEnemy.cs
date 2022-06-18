using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum EnemyStates { Walk, Chase, Attack };

public class MeleeEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float meleeEnemyMaxHealth = 10;
    [SerializeField] float meleeEnemyHealth = 10;
    [SerializeField] private float enemyRageXp = 20;
    public float walkSpeed = 3;
    public float chaseSpeed = 6;
    [SerializeField] private float attackWalkSpeed = 0;
    [SerializeField] private float knockBackPower;

    [Header("States")]
    [SerializeField] private EnemyStates currentState;
    [SerializeField] private float decisionInterval = 1;
    private Collider[] _attackCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack")]
    [SerializeField] private int enemyDamage = 5;
    public float meleeAttackSpeed = 1.5f;
    [SerializeField] private Transform enemyAttackArea;
    [SerializeField] float currentChaseRadius = 5;
    public float chaseRadius = 5;
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
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private StatsSaves statsSaves;
    
    private float _timeSinceLastDecision;
    float _attackTimer;
    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    private Player _playerCs;
    private bool _isAlive = true;
    private Animator _animator;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerCs = FindObjectOfType<Player>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _target = GameObject.FindGameObjectWithTag(Constants.playerTag).transform;
    }

    void Start()
    {
        currentState = EnemyStates.Walk;
        enemyHealthSlider.maxValue = meleeEnemyMaxHealth;
        enemyHealthSlider.value = meleeEnemyHealth;
        _currentIdx = Random.Range(0, path.Length - 1);
    }
    
    void Update()
    {
        enemyHealthSlider.value = meleeEnemyHealth;
        if (_isAlive)
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
        if (distance < 5)
        {
            return true;
        }
        return false;
    }
    
    void IterateIndex()
    {
        _currentIdx = Random.Range(0, path.Length - 1);
        _currentIdx %= path.Length;
    }
    
    void Chase()
    {
        _animator.SetTrigger("alerted");
        _navMeshAgent.speed = chaseSpeed;
        _navMeshAgent.SetDestination(_target.position);

        //_audioSource.PlayOneShot(_audioClips[0]);
        //enemy alerted sign

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
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.speed = attackWalkSpeed;
            
            //salise gibi artıyor
            _attackTimer += Time.deltaTime;
            if (meleeAttackSpeed <= _attackTimer)
            {
                _animator.SetTrigger("isAttacking");
                //_audioSource.PlayOneShot(_audioClips[1]);
                
                _playerCs.PlayerGetHit(enemyDamage);
                _attackTimer = 0;
            }
            else
            {
                _animator.SetTrigger("waitToAttack");
                //it is always working
                //need to design
            }
        }
        else
        {
            _animator.SetTrigger("alerted");
            currentState = EnemyStates.Chase;
        }
    }

    public void MeleeEnemyGetHit(float playerDamage)
    {
        if (_playerCs.rageModeOn)
        {
            statsSaves.HealthVar += playerDamage / 2;
            if ( statsSaves.HealthVar >= _playerCs.playerMaxHealth)
            {
                statsSaves.HealthVar = _playerCs.playerMaxHealth;
            }
        }
        
        //_audioSource.PlayOneShot(_audioClips[2]);
        
        _navMeshAgent.velocity = -transform.forward * knockBackPower;
        _animator.SetTrigger("knockback");

        meleeEnemyHealth -= playerDamage;
        enemyHealthSlider.value = meleeEnemyHealth;
        
        if (meleeEnemyHealth <= 0 && _isAlive)
        {
            MeleeEnemyDeath();
        }
    }
    
    void MeleeEnemyDeath()
    {
        Vector3 lootPosOffset= new Vector3(0,  10, 0);
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("wall");
        if (other.CompareTag(Constants.laserWallTag))
        {
            walkSpeed /= 2;
            chaseSpeed /= 2;
            meleeAttackSpeed /= 2;
            chaseRadius -= 15;
        }
        
        
        /*if (other.CompareTag(Constants.doorTag))
        {
             GetComponent<MoveToCollidor>().enabled = true;
             this.enabled = false;
        }
        if (other.CompareTag(Constants.collidorTag))
        {
             GetComponent<MoveToCollidor>().enabled = false;
             this.enabled = true;
        }*/
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag((Constants.shurikenTag)))
        {
            MeleeEnemyGetHit(FindObjectOfType<Shuriken>().shurikenDamage);
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
