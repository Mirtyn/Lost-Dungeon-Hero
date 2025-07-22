using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : ObjectBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public Transform Transform { get; private set; }
    private List<BaseEnemy> _enemies = new List<BaseEnemy>();
    public static event Action<BaseEnemy> OnEnemyDeath;
    public static event Action<float> OnEnemyDamagePlayer;
    private float updateTimer = 0.2f;
    private float updateTimerDelta = 0.2f;

    private void Awake()
    {
        Instance = this;
        Transform = this.transform;
    }

    private void Start()
    {
        ProjectileManager.OnEnemyHit += OnEnemyHit;
    }

    private void OnDestroy()
    {
        ProjectileManager.OnEnemyHit -= OnEnemyHit;
        Instance = null;
    }

    private void Update()
    {
        HandleEnemyAttacks();

        updateTimerDelta -= Time.deltaTime;
        if (updateTimerDelta > 0f) return;
        updateTimerDelta = updateTimer;

        HandleEnemyMovement();
    }

    private void OnEnemyHit(BaseEnemy enemy, float damage)
    {
        enemy.AudioPlayer.PlayHitSound();
        enemy.TakeDamage(damage);

        if (enemy.Health <= 0f)
        {
            enemy.AudioPlayer.PlayDeathSound();
            _enemies.Remove(enemy);

            enemy.NavMeshAgent.enabled = false;
            enemy.GetComponent<CapsuleCollider>().enabled = false;
            enemy.enabled = false;
            enemy.Transform.GetComponentInChildren<CreatureAnimator>().PlayDeath();
            Destroy(enemy.gameObject, 1f);

            OnEnemyDeath?.Invoke(enemy);
        }
    }

    private void HandleEnemyAttacks()
    {
        if (Game.IsPlayerDead) return;
        foreach (var enemy in _enemies)
        {
            enemy.AttackCooldownDelta -= Time.deltaTime;

#if UNITY_EDITOR
            var ray = new Ray(enemy.Transform.position + Vector3.up * 0.5f, Player.Instance.Transform.position - enemy.Transform.position);
            Debug.DrawRay(ray.origin, ray.direction * enemy.AttackDistance, Color.red, .2f);
#endif
            if (enemy.AttackCooldownDelta <= 0f)
            {
                TryAttack(enemy);
            }
        }
    }

    private void HandleEnemyMovement()
    {
        float wanderMaxDistance = 3f;

        foreach (var enemy in _enemies)
        {
            if (PowerManager.Instance.ActivePower.HasFlag(ActivePower.Invisibility) || Game.IsPlayerDead)
            {
                if (!enemy.IsIdle)
                {
                    var pos = enemy.Transform.position;
                    pos += new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * wanderMaxDistance;
                    enemy.NavMeshAgent.SetDestination(pos);
                    enemy.IsIdle = true;
                }
                else if (enemy.NavMeshAgent.remainingDistance < 0.5f || enemy.NavMeshAgent.remainingDistance > 1000f)
                {
                    var pos = enemy.Transform.position;
                    pos += new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * wanderMaxDistance;
                    enemy.NavMeshAgent.SetDestination(pos);
                }
                continue;
            }

            enemy.IsIdle = false;
            enemy.NavMeshAgent.SetDestination(Player.Instance.Transform.position);
        }
    }

    private void TryAttack(BaseEnemy enemy)
    {
        var ray = new Ray(enemy.Transform.position + Vector3.up * 0.5f, Player.Instance.Transform.position - enemy.Transform.position);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, enemy.AttackDistance, Layers.PLAYER | Layers.ENVIRONMENT | Layers.ENEMY))
        {
            if (hitInfo.transform.CompareTag(Tags.PLAYER))
            {
                OnEnemyDamagePlayer?.Invoke(enemy.AttackDamage);
                enemy.AttackCooldownDelta = enemy.AttackCooldown;
            }
        }
    }

    public void SpawnFodderEnemy(Vector3 position)
    {
        SpawnFodderEnemy(position, false);
    }

    public void SpawnHeavyEnemy(Vector3 position)
    {
        SpawnHeavyEnemy(position, false);
    }

    public void SpawnQuickEnemy(Vector3 position)
    {
        SpawnQuickEnemy(position, false);
    }

    public void SpawnFodderEnemy(Vector3 position, bool corrupt)
    {
        var enemyGO = Instantiate(Game.Assets.FodderEnemyPrefab, position, Quaternion.identity, Transform);
        var fodderEnemy = enemyGO.GetComponent<FodderEnemy>();

        if (corrupt)
        {
            fodderEnemy.Init(
                enemyGO.transform,
                FodderEnemy.CORRUPT_HEALTH,
                FodderEnemy.CORRUPT_MOVE_SPEED,
                FodderEnemy.CORRUPT_ROTATE_SPEED,
                FodderEnemy.CORRUPT_AI_PRIORITY,
                FodderEnemy.CORRUPT_ENERGY_REWARD,
                FodderEnemy.CORRUPT_ATTACK_COOLDOWN,
                FodderEnemy.CORRUPT_ATTACK_DAMAGE,
                corrupt: true);
        }
        else
        {
            fodderEnemy.Init(enemyGO.transform);
        }
        _enemies.Add(fodderEnemy);
    }

    public void SpawnHeavyEnemy(Vector3 position, bool corrupt)
    {
        var enemyGO = Instantiate(Game.Assets.HeavyEnemyPrefab, position, Quaternion.identity, Transform);
        var heavyEnemy = enemyGO.GetComponent<HeavyEnemy>();

        if (corrupt)
        {
            heavyEnemy.Init(
                enemyGO.transform,
                HeavyEnemy.CORRUPT_HEALTH,
                HeavyEnemy.CORRUPT_MOVE_SPEED,
                HeavyEnemy.CORRUPT_ROTATE_SPEED,
                HeavyEnemy.CORRUPT_AI_PRIORITY,
                HeavyEnemy.CORRUPT_ENERGY_REWARD,
                HeavyEnemy.CORRUPT_ATTACK_COOLDOWN,
                HeavyEnemy.CORRUPT_ATTACK_DAMAGE,
                corrupt: true);
        }
        else
        {
            heavyEnemy.Init(enemyGO.transform);
        }

        _enemies.Add(heavyEnemy);
    }

    public void SpawnQuickEnemy(Vector3 position, bool corrupt)
    {
        var enemyGO = Instantiate(Game.Assets.QuickEnemyPrefab, position, Quaternion.identity, Transform);
        var quickEnemy = enemyGO.GetComponent<QuickEnemy>();

        if (corrupt)
        {
            quickEnemy.Init(
                enemyGO.transform,
                QuickEnemy.CORRUPT_HEALTH,
                QuickEnemy.CORRUPT_MOVE_SPEED,
                QuickEnemy.CORRUPT_ROTATE_SPEED,
                QuickEnemy.CORRUPT_AI_PRIORITY,
                QuickEnemy.CORRUPT_ENERGY_REWARD,
                QuickEnemy.CORRUPT_ATTACK_COOLDOWN,
                QuickEnemy.CORRUPT_ATTACK_DAMAGE,
                corrupt: true);
        }
        else
        {
            quickEnemy.Init(enemyGO.transform);
        }

        _enemies.Add(quickEnemy);
    }
}
