using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    //public float playerHealth = 20;
    public int playerMaxHealth = 100;
    public float currentSpeed = 4;
    [SerializeField] private float speed = 4;
    [SerializeField] private float runSpeed = 7;
    public float dashSpeed = 200;
    [SerializeField] private float dashCooldown = 2;
    [SerializeField] private ParticleSystem dashVFX;

    [Header("Attack")]
    [SerializeField] private float katanaDamage;
    [SerializeField] private float katanaRange = 2;
    [SerializeField] private Transform katanaArea;
    [SerializeField] private GameObject shuriken;
    [SerializeField] float fireRate = 1;
    public float shurikenMagazine = 10;
    [SerializeField] private Text shurikenMagazineText;

    [Header("Rage Mode")]
    public float rageBar;
    [SerializeField] float maxRageBar = 100;
    [SerializeField] private float rageModeCooldown = 10;
    private float _currentRageModeCooldown = 5;
    [SerializeField] private GameObject rageModeFeedback;
    public bool rageModeOn;
    
    [Header("Components")]
    public Slider playerHeathSlider;
    [SerializeField] private Text playerHealthText;
    public Slider rageBarSlider;
    [SerializeField] private Transform barrel;
    public bool oldboy;
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private ParticleSystem katanaTrail;

    private CharacterController _characterController;
    private Vector3 _move;
    private Animator _animator;
    private bool _isMoving;
    private bool _isDashing;
    private Collider[] _colliders;
    public bool isAlive = true;
    private float _timeSinceLastFire;
    private float _timeSinceLastDash = 2;
    private float gravity = 9.8f;
    private GameObject _camera;
    private CameraTry _cameraTry;
    [SerializeField] private StatsSaves statsSaves;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _cameraTry = FindObjectOfType<CameraTry>();
        _animator = GetComponent<Animator>();
        dashVFX.Stop();
        katanaTrail.Stop();
    }

    void Start()
    {
        _camera = GameObject.FindWithTag(Constants.cameraTag);
        
        playerHeathSlider.maxValue = playerMaxHealth;
        playerHeathSlider.value = statsSaves.HealthVar;
        rageBarSlider.value = statsSaves.RageVar;
        rageBarSlider.maxValue = maxRageBar;
        _currentRageModeCooldown = rageModeCooldown;
        currentSpeed = speed;
    }

    void Update()
    {
        playerHealthText.text = statsSaves.HealthVar.ToString();
        shurikenMagazineText.text = statsSaves.ShurikenVar.ToString();

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
            SceneManager.LoadScene("DeathScene");
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

        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack") &&
            !_animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerLaser") &&
            !_animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRageMode"))
        {
            katanaTrail.Stop();
            
            if (!_isDashing)
            {
                if (!_characterController.isGrounded)
                {
                    Vector3 appliedGravity = gravity * Time.deltaTime * Vector3.down;
                    _characterController.Move(appliedGravity);
                }
                
                if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
                {
                    _isMoving = true;
                    RotatePlayer();
                }
                else
                {
                    _isMoving = false;
                    currentSpeed = 0;
                    audioSources[7].Stop();
                    audioSources[0].Stop();
                    StopCoroutine(DashVFX());
                }
                
                _move = new Vector3(h, 0, v);
                _move = _camera.transform.TransformDirection(_move);
                _move.y = 0;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _animator.SetFloat("speed", currentSpeed);
                    audioSources[0].Stop();

                    currentSpeed = runSpeed;
                    _characterController.Move(_move / 10 * currentSpeed);

                    if (!audioSources[7].isPlaying && _isMoving && !_isDashing)
                    {
                        audioSources[7].Play();
                    }
                }
                else
                {
                    _animator.SetFloat("speed", currentSpeed);
                    audioSources[7].Stop();

                    currentSpeed = speed;
                    _characterController.Move(_move / 10 * currentSpeed);
                    if (!audioSources[0].isPlaying)
                    {
                        audioSources[0].Play();
                    }
                }
            }
            else
            {
                audioSources[7].Stop();
                audioSources[0].Stop();
                _animator.SetFloat("speed", currentSpeed);
                StartCoroutine(DashVFX());
                StartCoroutine(DashMove());
            }
        }
    }

    void Dash()
    {
        _timeSinceLastDash += Time.deltaTime;
        if (_isMoving && dashCooldown < _timeSinceLastDash)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _timeSinceLastDash = 0;
                _isDashing = true;
                
                if (!audioSources[1].isPlaying)
                {
                    audioSources[1].Play();
                }
            }
        }

        if (_timeSinceLastDash >= dashCooldown - 0.1f && _timeSinceLastDash <= dashCooldown + 0.1f)
        {
            if (!audioSources[8].isPlaying)
            {
                audioSources[8].Play();
            }
        }
    }

    IEnumerator DashMove()
    {
        if (!audioSources[1].isPlaying)
        {
            audioSources[1].Play();
        }
        
        currentSpeed = dashSpeed;
        _characterController.Move(_move / 10 * currentSpeed);
        yield return new WaitForSeconds(0.2f);
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
            var rotationVector = _camera.transform.rotation.eulerAngles;
            rotationVector.x = 0;
            rotationVector.z = 0;
            transform.rotation = Quaternion.Euler(rotationVector);

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack"))
            {
                _animator.SetTrigger("isAttacking");
                katanaTrail.Play();

                if (!audioSources[2].isPlaying)
                { 
                    audioSources[2].Play();
                }

                _colliders = Physics.OverlapSphere(katanaArea.position, katanaRange, LayerMask.GetMask("MeleeEnemy"));
                foreach (Collider hit in _colliders)
                {
                    if (!audioSources[3].isPlaying)
                    { 
                        audioSources[3].Play();
                    }
                    hit.GetComponent<MeleeEnemy>().MeleeEnemyGetHit(katanaDamage);
                }
                _colliders = null;
            
                _colliders = Physics.OverlapSphere(katanaArea.position, katanaRange, LayerMask.GetMask("RangedEnemy"));
                foreach (Collider hit in _colliders)
                {
                    if (!audioSources[3].isPlaying)
                    { 
                        audioSources[3].Play();
                    }
                    hit.GetComponent<RangedEnemy>().RangedEnemyGetHit(katanaDamage);
                }
                _colliders = null;
            
                _colliders = Physics.OverlapSphere(katanaArea.position, katanaRange, LayerMask.GetMask("Turret"));
                foreach (Collider hit in _colliders)
                {
                    if (!audioSources[3].isPlaying)
                    { 
                        audioSources[3].Play();
                    }
                    hit.GetComponent<Turret>().TurretGetHit(katanaDamage);
                }
                _colliders = null;
            }
        }
    }

    void PlayerFire()
    {
        if (Input.GetMouseButton(1))
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerLaser"))
            {
                _cameraTry.Crosshair();
                var rotationVector = _camera.transform.rotation.eulerAngles;
                rotationVector.x = 0;
                rotationVector.z = 0;
                transform.rotation = Quaternion.Euler(rotationVector);

                if (statsSaves.ShurikenVar > 0)
                {
                    _timeSinceLastFire += Time.deltaTime;
                    if (_timeSinceLastFire > fireRate)
                    {
                        _animator.SetTrigger("shuriken");
                        statsSaves.ShurikenVar--;
                        _timeSinceLastFire = 0;
                    }
                }
                else
                {
                    //empty gun sound
                }
            }
        }
    }

    void ShootLaser()
    {
        if (!audioSources[4].isPlaying)
        {
            audioSources[4].Play();
        }
        Instantiate(shuriken, barrel.position, transform.rotation);
    }

    void RageMode()
    {
        rageBarSlider.value = rageBar;

        if (!rageModeOn)
        {
            if (rageBar >= maxRageBar)
            {
                if (rageBar > maxRageBar)
                {
                    rageBar = maxRageBar;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine("RageModeFeedBack");
                    _animator.SetTrigger("ragemode");
                    rageModeOn = true;
                    _characterController.slopeLimit *= 1.5f;
                    fireRate /= 1.5f;
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
            rageBar -= maxRageBar / _currentRageModeCooldown * Time.deltaTime;
            rageBarSlider.value = rageBar;
                    
            if (_currentRageModeCooldown <= 0)
            {
                rageModeOn = false;
                rageBar = 0;
                _currentRageModeCooldown = rageModeCooldown;
            }
        }
    }

    IEnumerator RageModeFeedBack()
    {
        rageModeFeedback.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        rageModeFeedback.SetActive(false);
    }

    public void PlayerGetHit(float enemyDamage)
    {
        //color can change
        
        audioSources[6].Play();
        statsSaves.HealthVar -= enemyDamage;
        playerHeathSlider.value = statsSaves.HealthVar;
        
        if (statsSaves.HealthVar <= 0)
        {
            playerHeathSlider.value = 0;
            statsSaves.HealthVar = 0;
            gameObject.layer = 0;
            
            _animator.SetTrigger("death");

            audioSources[5].Play();
            isAlive = false;
            StartCoroutine(LoadDeathScene());
        }
    }
    
    IEnumerator LoadDeathScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("DeathScene");
    }
    
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.oldBoyTag))
        {
            GameObject.Find("Main Camera").GetComponent<CameraTry>().enabled = false;
            GameObject.Find("Main Camera").GetComponent<OldBoyCamera>().enabled = true;
            oldboy = true;
            Destroy(other.gameObject);
        }
    }*/
    


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(katanaArea.position, katanaRange);
    }
}