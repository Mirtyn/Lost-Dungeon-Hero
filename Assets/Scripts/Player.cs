using UnityEngine;

public class Player : ObjectBehaviour
{
    public static Player Instance { get; private set; }
    public Transform Transform { get; private set; }
    public static event System.Action OnPlayerOverPower;

    private readonly float _defaultSpeed = 3.6f;
    private float _moveSpeed 
    { 
        get
        {
            if (PowerManager.Instance.ActivePower.HasFlag(ActivePower.Haste))
            {
                return _defaultSpeed * 1.5f;
            }

            return _defaultSpeed;
        } 
    }

    private float _rotateSpeed = 25f;

    private float _arrowSpawnDistance = 0.0f;
    private float _arrowSpawnHeight = 0.5f;
    private float _arrowSpeed = 9f;
    private float _arrowDamage = 100f;

    private float _defaultShootCooldown = 0.5f;
    private float _shootCooldownMultiplierOnLevelUp = 0.6f;
    private float _shootCooldown
    {
        get
        {
            if (PowerManager.Instance.ActivePower.HasFlag(ActivePower.RappidFire))
            {
                return _defaultShootCooldown / 2f;
            }

            return _defaultShootCooldown;
        }
    }
    private float _shootCooldownDelta = 0.5f;

    private float _power = 25f;
    private float _maxPower = 100f;
    private float _powerDecayRate = 0f;
    private float _maxPowerIncrementation = 150f;
    private float _maxMultuplyAddition = 1.5f;
    public int Kills { get; private set; }
    public int StonePickedUp { get; private set; }
    public float GetPower { get { return _power; } }
    public float GetMaxPower { get { return _maxPower; } }

    [SerializeField] private Transform _playerMesh1;
    [SerializeField] private Transform _playerMesh2;
    private Renderer _playerRenderer1 => _playerMesh1.GetComponent<Renderer>();
    private Renderer _playerRenderer2 => _playerMesh2.GetComponent<Renderer>();
    private CreatureAudioPlayer _audioPlayer;
    private ParticleSystem _lvlUp;

    private void Awake()
    {
        _lvlUp = GetComponentInChildren<ParticleSystem>();
        Instance = this;
        Transform = this.transform;
        _audioPlayer = GetComponent<CreatureAudioPlayer>();
    }

    private void Start()
    {
        EnemyManager.OnEnemyDeath += OnEnemyDeath;
        SoulGemManager.OnSoulGemCollected += () =>
        {
            StonePickedUp++;
            AddPower(1f);
        };
        EnemyManager.OnEnemyDamagePlayer += DamagePlayer;
        //_playerRenderer1 = _playerMesh1.GetComponent<Renderer>();
        //_playerRenderer2 = _playerMesh2.GetComponent<Renderer>();
    }

    private void OnDestroy()
    {
        EnemyManager.OnEnemyDeath -= OnEnemyDeath;
        SoulGemManager.OnSoulGemCollected -= () => { };
        EnemyManager.OnEnemyDamagePlayer -= DamagePlayer;
        Instance = null;
    }

    private void OnEnemyDeath(BaseEnemy enemy)
    {
        Kills++;
    }

    private void Update()
    {
        if (Game.IsPlayerDead)
        {
            return; // Do not update player if dead
        }

        HandleMovement();
        HandleShooting();
        HandleRotation();
        UpdatePower();
    }

    private void AddPower(float amount)
    {
        _power += amount;

        if (_power >= _maxPower)
        {
            _power = _maxPower;
            OverPower();
        }
    }

    private void OverPower()
    {
        _power += 0.5f;
        _maxPower += _maxPowerIncrementation;
        _maxPower *= _maxMultuplyAddition;
        _defaultShootCooldown *= _shootCooldownMultiplierOnLevelUp;
        _lvlUp.Play();
        OnPlayerOverPower?.Invoke();
    }

    public void DamagePlayer(float amount)
    {
        _power -= amount;
        _audioPlayer.PlayHitSound();
        if (_power < 0f)
        {
            _power = 0f;
            Death();
        }

        DamageColor();
        Invoke(nameof(UndoDamageColor), 0.15f);
    }

    private void DamageColor()
    {
        _playerRenderer1.material.color = Color.red;
        _playerRenderer2.material.color = Color.red;
    }

    private void UndoDamageColor()
    {
        _playerRenderer1.material.color = Color.white;
        _playerRenderer2.material.color = Color.white;
    }

    private void Death()
    {
        _audioPlayer.PlayDeathSound();
        Debug.Log("Player Died!");
        Game.IsPlayerDead = true;
        Transform.GetChild(0).GetComponent<CreatureAnimator>().PlayDeath();
        Transform.GetChild(1).GetComponent<Light>().enabled = false;
        Destroy(Transform.GetChild(0).gameObject, 1f);
    }

    private void HandleMovement()
    {
        var playerInput = GetPlayerMoveInputNormalized();

        float playerRadius = 0.175f;
        float playerHeight = 1.15f;
        float moveDistance = _moveSpeed * Time.deltaTime;

        var moveDir = new Vector3(playerInput.x, 0, playerInput.y);
        var dirX = new Vector3(playerInput.x, 0, 0);
        var dirY = new Vector3(0, 0, playerInput.y);

        bool hitSomethingX = Physics.CapsuleCast(
            this.transform.position,
            Transform.position + Vector3.up * playerHeight,
            playerRadius,
            dirX,
            moveDistance,
            Layers.ENVIRONMENT | Layers.ENEMY_WALKABLE_ENVIRONMENT
            );

        bool hitSomethingY = Physics.CapsuleCast(
            this.transform.position,
            Transform.position + Vector3.up * playerHeight,
            playerRadius,
            dirY,
            moveDistance,
            Layers.ENVIRONMENT | Layers.ENEMY_WALKABLE_ENVIRONMENT
            );

        if (hitSomethingX)
        {
            moveDir.x = 0;
        }

        if (hitSomethingY)
        {
            moveDir.z = 0;
        }

        if (moveDir == Vector3.zero)
        {
            return; // No movement input
        }

        transform.position += moveDir * moveDistance;
    }

    private void HandleRotation()
    {
        if (Game.PlayerInput.GetMouseWorldSpacePosition3D(
                out Vector3 hitPosition,
                Game.PlayerInput.GetMouseScreenSpacePosition(),
                100f,
                Layers.ENEMY | Layers.FLOOR
            ))
        {
            hitPosition.y = Transform.position.y;
            Vector3 forwardDir = (hitPosition - Transform.position).normalized;

            Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.LookRotation(forwardDir), _rotateSpeed * Time.deltaTime);
        }
    }

    private Vector2 GetPlayerMoveInputNormalized()
    {
        var input = new Vector2();

        if (Input.GetKey(KeyCode.W))
        {
            input.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input.y -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            input.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input.x += 1;
        }

        return input.normalized;
    }

    private void HandleShooting()
    {
        _shootCooldownDelta -= Time.deltaTime;
        if (_shootCooldownDelta > 0) return;


        if (Input.GetKey(KeyCode.Mouse0))
        {
            _shootCooldownDelta = _shootCooldown;

            if(Game.PlayerInput.GetMouseWorldSpacePosition3D(
                out Vector3 hitPosition,
                Game.PlayerInput.GetMouseScreenSpacePosition(),
                100f,
                Layers.ENEMY | Layers.FLOOR
            ))
            {
                hitPosition.y = Transform.position.y;
                Vector3 forwardDir = (hitPosition - Transform.position).normalized;

                ProjectileManager.Instance.CreateArrow(
                    Transform.position + forwardDir * _arrowSpawnDistance + new Vector3(0f, _arrowSpawnHeight, 0f),
                    forwardDir,
                    _arrowSpeed,
                    _arrowDamage
                );


            }
        }
    }

    //private void KillEnemiesInFront()
    //{
    //    var enemies = Physics.OverlapSphere(Transform.position, .35f, Layers.ENEMY);
    //    if (enemies.Length == 0) return;

    //    foreach (var enemyCollider in enemies)
    //    {
    //        var enemy = enemyCollider.GetComponent<BaseEnemy>();
    //        if (Vector3.Dot(Transform.position))
    //        {
    //            enemy.TakeDamage(enemy.Health); // Deal fatal damage to the enemy
    //        }
    //    }
    //}

    private void UpdatePower()
    {
        _power -= Time.deltaTime * _powerDecayRate;
    }
}
