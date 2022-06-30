using System;
using System.Collections;
using Game;
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
    public float currentSpeed;
    public float walkSpeed = 3;
    public float chaseSpeed = 6;
    [SerializeField] private float attackWalkSpeed = 0;
    [SerializeField] private float knockBackPower;
    [SerializeField] private ParticleSystem deathVFX;

    [Header("States")]
    [SerializeField] private EnemyStates currentState;
    [SerializeField] private float decisionInterval = 1;
    private Collider[] _attackCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack")]
    [SerializeField] private int enemyDamage = 5;
    public float meleeAttackSpeed = 1.5f;
    [SerializeField] private Transform enemyAttackArea;
    public float currentChaseRadius = 5;
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
    [SerializeField] private Image attackAlert;
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private StatsSaves statsSaves;
    public bool isChasing;
    public bool isAttacking;
    
    private AudioSource _audioSource;
    private AudioClip _currentClip;
    private float _timeSinceLastDecision;
    float _attackTimer;
    private NavMeshAgent _navMeshAgent;
    private Transform _target;
    private Player _playerCs;
    private bool _isAlive = true;
    private Animator _animator;
    private GameObject alertImage;
    private Animator _alertImageAnimator;
    private MeleeEnemy[] _meleeEnemyCs;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerCs = FindObjectOfType<Player>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        _alertImageAnimator = attackAlert.GetComponent<Animator>();
        
        _target = GameObject.FindGameObjectWithTag(Constants.playerTag).transform;
        attackAlert.enabled = false;
        deathVFX.Stop();
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
        _meleeEnemyCs = FindObjectsOfType<MeleeEnemy>();

        if (_isAlive)
        {
            _navMeshAgent.speed = currentSpeed;
            
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

            if (_audioSource.isPlaying && _audioSource.clip == _currentClip)
            {
                return;
            }
            else
            {
                _audioSource.clip = _currentClip;
                _audioSource.Play();
            }

            #region PlayerRageMode
            
            if (_playerCs.rageModeOn)
            {
                currentChaseRadius = currentChaseRadius * 2.5f;
            }
            else
            {
                currentChaseRadius = chaseRadius;
            }
            
            #endregion

            #region EnemyAlertMode

             if (isChasing)
             {
                 for (int i = 0; i < _meleeEnemyCs.Length; i++)
                 {
                     _meleeEnemyCs[i].currentChaseRadius = _meleeEnemyCs[i].currentChaseRadius * 2;
                 }
             }
             else
             {
                 for (int i = 0; i < _meleeEnemyCs.Length; i++)
                 {
                     _meleeEnemyCs[i].currentChaseRadius = _meleeEnemyCs[i].chaseRadius;
                 }
             }

            #endregion
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
        else if(Physics.CheckSphere(transform.position, attackRadius, 8))
        {
            _navMeshAgent.isStopped = true;
        }
        else
        {
            _navMeshAgent.isStopped = false;
        }
        
        if (path != null)
        {
            _currentClip = _audioClips[0];
            
            _animator.SetTrigger("isWalking");
            currentSpeed = walkSpeed;
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
        currentSpeed = chaseSpeed;
        _navMeshAgent.SetDestination(_target.position);
        attackAlert.enabled = true;
        isChasing = true;

        _currentClip = _audioClips[1];

        if (!Physics.CheckSphere(transform.position, currentChaseRadius, playerLayer))
        {
            isChasing = false;
            currentState = EnemyStates.Walk;
        }
        
        if(Physics.CheckSphere(transform.position, attackRadius, 8))
        {
            _navMeshAgent.isStopped = true;
        }
        else
        {
            _navMeshAgent.isStopped = false;
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
            transform.LookAt(_target.position);
            _navMeshAgent.SetDestination(transform.position);
            currentSpeed = attackWalkSpeed;

            //salise gibi artıyor
            _attackTimer += Time.deltaTime * 8;
            //Debug.Log(_attackTimer); 
            
            if (_attackTimer >= 0 && _attackTimer <= meleeAttackSpeed - meleeAttackSpeed / 2)
            {
                _alertImageAnimator.SetTrigger("attack");
            }

            
            if (meleeAttackSpeed <= _attackTimer)
            {
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack"))
                {
                    _animator.SetTrigger("isAttacking");
                    _navMeshAgent.isStopped = true;
                }
                else
                {
                    _navMeshAgent.isStopped = true;
                }

                _playerCs.PlayerGetHit(enemyDamage);
                _attackTimer = 0;
            }
            else
            {
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("WaitToAttack") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack"))
                {
                    _animator.SetTrigger("waitToAttack");
                    _navMeshAgent.isStopped = true;
                    _currentClip = _audioClips[4];
                }
                else
                {
                    _navMeshAgent.isStopped = true;
                }
            }
        }
        else if(Physics.CheckSphere(transform.position, attackRadius, 8))
        {
            _navMeshAgent.isStopped = true;
        }
        else
        {
            _navMeshAgent.isStopped = false;
            _animator.Play("EnemyRun");
            currentState = EnemyStates.Chase;
        }
        
    }

    public void MeleeEnemyGetHit(float playerDamage)
    {
        if (_playerCs.rageModeOn)
        {
            statsSaves.HealthVar += (playerDamage / 2);
            if ( statsSaves.HealthVar >= _playerCs.playerMaxHealth)
            {
                statsSaves.HealthVar = _playerCs.playerMaxHealth;
            }

            _playerCs.playerHeathSlider.value = statsSaves.HealthVar;
        }
        
        _currentClip = _audioClips[2];
        
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
        StartCoroutine("DeathVFX");
        _navMeshAgent.speed = walkSpeed;
        Vector3 lootPosOffset= new Vector3(0,  10, 0);
        _isAlive = false;
        isChasing = false;
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
        
        _audioSource.PlayOneShot(_audioClips[3]);

        Destroy(gameObject, 1);
    }
    
    IEnumerator DeathVFX()
    {
        deathVFX.Play();
        yield return new WaitForSeconds(0.5f);
        deathVFX.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("wall");
        if (other.CompareTag(Constants.laserWallTag))
        {
            walkSpeed /= 2;
            chaseSpeed /= 2;
            meleeAttackSpeed *= 1.5f;
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
