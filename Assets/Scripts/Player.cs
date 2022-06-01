using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float playerHealth = 20;
    public float playerMaxHealth = 20;
    [SerializeField] private float speed = 4;
    [SerializeField] private float runSpeed = 7;
    [SerializeField] private float dashSpeed = 200;
    [SerializeField] private float dashCooldown = 2;
    [SerializeField] private ParticleSystem dashVFX;
    [SerializeField] private float gravity = 9.8f;

    [Header("Attack")]
    [SerializeField] private float katanaDamage;
    [SerializeField] private float katanaRange = 2;
    [SerializeField] private Transform katanaArea;
    //[SerializeField] float katanaCooldown = 0.5f;
    //private float _timeSinceLastMelee;
    [SerializeField] float fireRate = 1;
    public float shurikenMagazine = 10;
    [SerializeField] private Text shurikenMagazineText;
    private float _timeSinceLastFire;
    
    //[SerializeField] private float pistolDamage;
    //[SerializeField] private float sniperDamage;
    
    [Header("Rage Mode")]
    public float rageBar;
    [SerializeField] float maxRageBar = 100;
    [SerializeField] private float rageModeCooldown = 5;
    private float _currentRageModeCooldown = 5;
    public bool rageModeOn;
    
    [Header("Components")] 
    [SerializeField] private new Camera camera;
    public Slider playerHeathSlider;
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Slider rageBarSlider;
    [SerializeField] private GameObject shuriken;
    [SerializeField] private Transform barrel;
    public bool oldboy;
    [SerializeField] private GameObject[] enemies;
    private MeleeEnemy _meleeEnemyCs;
    [SerializeField] private AudioSource[] _audioSources;

    private Vector3 _move;
    private Animator _animator;
    private float currentSpeed = 4;
    private bool _isMoving;
    private bool _isDashing;
    private Collider[] _colliders;
    private CharacterController _characterController;
    public bool isAlive = true;
    private CameraTry _camera;
    
    void Start()
    {
        dashVFX.Stop();
        playerHeathSlider.maxValue = playerMaxHealth;
        playerHeathSlider.value = playerHealth;
        rageBarSlider.value = rageBar;
        rageBarSlider.maxValue = maxRageBar;
        _currentRageModeCooldown = rageModeCooldown;
        currentSpeed = speed;
        
        _characterController = GetComponent<CharacterController>();
        _camera = FindObjectOfType<CameraTry>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        playerHealthText.text = playerHealth.ToString();
        shurikenMagazineText.text = shurikenMagazine.ToString();
        
        if (isAlive)
        {
            CharacterControllerMove();
            Dash();
            PlayerAttack();
            PlayerFire();
            RageMode();
        }
        else
        {
            //SceneManager.LoadScene("DeathScreen");
        }
    }
    
    void RotatePlayer()
    {
        transform.rotation = Quaternion.LookRotation(_move);
    }

    void CharacterControllerMove()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        if (!_isDashing)
        {
            if (!_characterController.isGrounded)
            {
                Vector3 appliedGravity = gravity * Time.deltaTime * Vector3.down;
                _characterController.Move(appliedGravity);
            }
            
            _move = new Vector3(h, 0, v);
            _move = camera.transform.TransformDirection(_move);
            _move.y = 0;
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //_animator.SetBool("isWalking", false);
                //_animator.SetBool("isRunning", true);
                
                _animator.SetFloat("speed", currentSpeed);
                _audioSources[0].Stop();

                currentSpeed = runSpeed;
                _characterController.Move(_move / 10 * currentSpeed);
                
                if (!_audioSources[7].isPlaying)
                {
                    _audioSources[7].Play();
                }
            }
            else
            {
                //_animator.SetBool("isRunning", false);
                //_animator.SetBool("isWalking", true);
                
                _animator.SetFloat("speed", currentSpeed);
                _audioSources[7].Stop();

                currentSpeed = speed;
                _characterController.Move(_move / 10 * currentSpeed);
                if (!_audioSources[0].isPlaying)
                {
                    _audioSources[0].Play();
                }
            }

            if ((Mathf.Abs(h) > 0 && Mathf.Abs(v) > 0) || (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0))
            {
                _isMoving = true;
                RotatePlayer();
            }
            else
            {
                currentSpeed = 0;
                _audioSources[0].Stop();
                StopCoroutine(DashVFX());
            }
        }
        else
        {
            _animator.SetFloat("speed", currentSpeed);
            StartCoroutine(DashVFX());
            StartCoroutine(DashMove());
        }
    }

    void Dash()
    {
        if (_isMoving && dashCooldown >= 1f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dashCooldown = 0;
                _isDashing = true;
            }
        }
        else
        {
            dashCooldown += Time.deltaTime;
            dashCooldown = Mathf.Clamp(dashCooldown, 0, 2);
        }
    }

    IEnumerator DashMove()
    {
        if (!_audioSources[1].isPlaying)
        {
            _audioSources[1].Play();
        }

        currentSpeed = dashSpeed;
        _characterController.Move(_move / 10 * currentSpeed);
        yield return new WaitForSeconds(0.1f);
        _characterController.Move(_move / 10 * currentSpeed);

        _isDashing = false;
    }

    IEnumerator DashVFX()
    {
        dashVFX.Play();
        yield return new WaitForSeconds(0.2f);
        dashVFX.Stop();
    }

    void PlayerAttack()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            _animator.SetTrigger("isAttacking");
            
            if (!_audioSources[2].isPlaying)
            { 
                _audioSources[2].Play();
            }

            _colliders = Physics.OverlapSphere(katanaArea.position, katanaRange, LayerMask.GetMask("MeleeEnemy"));
            foreach (Collider hit in _colliders)
            {
                if (!_audioSources[3].isPlaying)
                { 
                    _audioSources[3].Play();
                }
                hit.GetComponent<MeleeEnemy>().MeleeEnemyGetHit(katanaDamage);
            }
            _colliders = null;
            
            _colliders = Physics.OverlapSphere(katanaArea.position, katanaRange, LayerMask.GetMask("RangedEnemy"));
            foreach (Collider hit in _colliders)
            {
                if (!_audioSources[3].isPlaying)
                { 
                    _audioSources[3].Play();
                }
                hit.GetComponent<RangedEnemy>().RangedEnemyGetHit(katanaDamage);
            }
            _colliders = null;
        }
        //_timeSinceLastMelee = 0;
    }

    void PlayerFire()
    {
        if (Input.GetMouseButton(1))
        {
            transform.LookAt(_camera.crosshair);

            if (shurikenMagazine > 0)
            {
                _timeSinceLastFire += Time.deltaTime;
                if (_timeSinceLastFire > fireRate)
                {
                    if (!_audioSources[4].isPlaying)
                    {
                        _audioSources[4].Play();
                    }

                    _animator.SetTrigger("shuriken");
                    Instantiate(shuriken, barrel.position, transform.rotation);
                    shurikenMagazine--;
                    _timeSinceLastFire = 0;
                }
            }
            else
            {
                //empty gun sound
            }
        }
       
    }

    void RageMode()
    {
        rageBarSlider.value = rageBar;

        if (!rageModeOn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            { 
                if (rageBar >= maxRageBar)
                {
                    rageModeOn = true;
                    _characterController.slopeLimit *= 1.5f;
                    //katanaCooldown /= 1.5f;
                    fireRate /= 1.5f;
                    
                    Debug.Log("rage on");
                }
                else
                { 
                    //empty bar sound
                }
            }
        }
        else
        {
            _currentRageModeCooldown -= Time.deltaTime;
            rageBar -= maxRageBar / _currentRageModeCooldown *Time.deltaTime;
            rageBarSlider.value = rageBar;
                    
            if (_currentRageModeCooldown <= 0)
            {
                rageModeOn = false;
                rageBar = 0;
                Debug.Log("rage off");
                _currentRageModeCooldown = rageModeCooldown;
            }
        }
    }

    public void PlayerGetHit(float enemyDamage)
    {
        //color can change
        
        _audioSources[6].Play();
        playerHealth -= enemyDamage;
        playerHeathSlider.value = playerHealth;
        
        if (playerHealth <= 0)
        {
            playerHeathSlider.value = 0;
            playerHealth = 0;
            
            _animator.SetBool("death", true);
           
            _audioSources[5].Play();
            isAlive = false;
            StartCoroutine(LoadDeathScene());
        }
    }
    
    IEnumerator LoadDeathScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("DeathScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.oldBoyTag))
        {
            GameObject.Find("Main Camera").GetComponent<CameraTry>().enabled = false;
            GameObject.Find("Main Camera").GetComponent<OldBoyCamera>().enabled = true;
            oldboy = true;
            Destroy(other.gameObject);
        }
    }
    

    
    
    
    
    
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(katanaArea.position, katanaRange);
    }
}