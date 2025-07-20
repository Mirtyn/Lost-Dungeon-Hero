using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileManager : ObjectBehaviour
{
    public static ProjectileManager Instance { get; private set; }
    public static event Action<BaseEnemy, float> OnEnemyHit;
    public Transform Transform { get; private set; }
    private List<Arrow> _arrows = new List<Arrow>();
    private List<Trap> _traps = new List<Trap>();
    private List<FireBall> _fireBalls = new List<FireBall>();

    private void Awake()
    {
        Instance = this;
        Transform = this.transform;
    }

    public void CreateArrow(Vector3 pos, Vector3 forwardDir, float speed, float damage)
    {
        var arrowGO = Instantiate(Game.Assets.ArrowPrefab, pos, Quaternion.LookRotation(forwardDir), Transform);
        var arrow = arrowGO.GetComponent<Arrow>();
        arrow.Init(speed, damage, arrowGO.transform);
        _arrows.Add(arrow);

        float distance = 0.1f;

        Ray ray = new Ray(arrowGO.transform.position + (forwardDir * -distance), forwardDir);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, Layers.ENVIRONMENT | Layers.ENEMY | Layers.ENEMY_WALKABLE_ENVIRONMENT))
        {
            if (hitInfo.transform.CompareTag(Tags.ENEMY))
            {
                OnEnemyHit?.Invoke(hitInfo.transform.GetComponent<BaseEnemy>(), arrow.Damage);
            }
            Destroy(arrow.gameObject);
            _arrows.Remove(arrow);
        }
    }

    public void CreateTrap(Vector3 pos, float attackCooldown = 0.2f, float damage = 40f)
    {
        var trapGO = Instantiate(Game.Assets.TrapPrefab, pos, Quaternion.identity, Transform);
        var trap = trapGO.GetComponent<Trap>();
        trap.Init(trapGO.transform, attackCooldown, damage);
        _traps.Add(trap);
    }

    public void CreateFireBall(Vector3 pos, Vector3 forwardDir)
    {
        var fireBallGO = Instantiate(Game.Assets.FireBallPrefab, pos, Quaternion.LookRotation(forwardDir), Transform);
        var fireBall = fireBallGO.GetComponent<FireBall>();
        fireBall.Init(fireBallGO.transform);
        _fireBalls.Add(fireBall);
    }

    private void Update()
    {
        UpdateArrows();
        UpdateTraps();
        UpdateFireballs();
    }

    private void UpdateArrows()
    {
        List<Arrow> arrowsToRemove = new List<Arrow>();
        foreach (var arrow in _arrows)
        {
            var arrowTransform = arrow.Transform;
            var moveDistance = Time.deltaTime * arrow.Speed;
            var moveDir = arrowTransform.forward;

            Ray ray = new Ray(arrowTransform.position, moveDir);

            if (!Physics.Raycast(ray, out RaycastHit hitInfo, moveDistance, Layers.ENVIRONMENT | Layers.ENEMY | Layers.ENEMY_WALKABLE_ENVIRONMENT))
            {
                arrowTransform.position += moveDir * moveDistance;
            }
            else
            {
                if (hitInfo.transform.CompareTag(Tags.ENEMY))
                {
                    OnEnemyHit?.Invoke(hitInfo.transform.GetComponent<BaseEnemy>(), arrow.Damage);
                }
                Destroy(arrow.gameObject);
                arrowsToRemove.Add(arrow);
            }
        }

        foreach (var arrow in arrowsToRemove)
        {
            _arrows.Remove(arrow);
        }
    }

    private void UpdateTraps()
    {
        List<Trap> trapsToRemove = new List<Trap>();
        foreach (var trap in _traps)
        {
            if (trap.LifeTime <= 0f)
            {
                Destroy(trap.gameObject);
                trapsToRemove.Add(trap);
                continue;
            }

            trap.LifeTime -= Time.deltaTime;
            trap.AttackCooldownDelta -= Time.deltaTime;

            if (trap.AttackCooldownDelta <= 0f)
            {

                var trapTransform = trap.Transform;
                var boxSize = new Vector3(.4f, 0.5f, 0.4f);

                var enemyColliders = Physics.OverlapBox(trapTransform.position, boxSize, Quaternion.identity, Layers.ENEMY, QueryTriggerInteraction.UseGlobal);

                if (enemyColliders.Length > 0)
                {
                    trap.AttackCooldownDelta = trap.AttackCooldown;
                    float damage = trap.Damage / enemyColliders.Length;

                    foreach (var collider in enemyColliders)
                    {
                        if (collider.transform.CompareTag(Tags.ENEMY))
                        {
                            var enemy = collider.GetComponent<BaseEnemy>();
                            OnEnemyHit?.Invoke(enemy, damage);
                        }
                    }
                }
            }
            
        }

        foreach (var trap in trapsToRemove)
        {
            _traps.Remove(trap);
        }
    }

    private void UpdateFireballs()
    {
        List<FireBall> fireBallsToRemove = new List<FireBall>();
        foreach (var fireBall in _fireBalls)
        {
            bool destroy = false;

            fireBall.AttackCooldownDelta -= Time.deltaTime;
            var fireBallTransform = fireBall.Transform;

            if (fireBall.AttackCooldownDelta <= 0f)
            {
                var enemyColliders = Physics.OverlapSphere(fireBallTransform.position, 0.5f, Layers.ENEMY, QueryTriggerInteraction.UseGlobal);

                if (enemyColliders.Length > 0)
                {
                    fireBall.AttackCooldownDelta = fireBall.AttackCooldown;

                    foreach (var collider in enemyColliders)
                    {
                        if (collider.transform.CompareTag(Tags.ENEMY))
                        {
                            float damage = fireBall.DefaultDamage;
                            fireBall.DefaultDamage *= fireBall.PiercingDamagePenalty;
                            fireBall.PiercingsLeft--;
                            var enemy = collider.GetComponent<BaseEnemy>();
                            OnEnemyHit?.Invoke(enemy, damage);

                            if (fireBall.PiercingsLeft <= 0)
                            {
                                destroy = true;
                                break; // Stop processing further enemies if the fireball has no piercings left
                            }
                        }
                    }
                }
            }

            if (destroy)
            {
                Destroy(fireBall.gameObject);
                fireBallsToRemove.Add(fireBall);
                continue;
            }

            var moveDistance = Time.deltaTime * fireBall.Speed;
            var moveDir = fireBallTransform.forward;

            Ray ray = new Ray(fireBallTransform.position, moveDir);

            if (!Physics.Raycast(ray, out RaycastHit hitInfo, moveDistance, Layers.ENVIRONMENT | Layers.ENEMY_WALKABLE_ENVIRONMENT))
            {
                fireBallTransform.position += moveDir * moveDistance;
            }
            else
            {
                Destroy(fireBall.gameObject);
                fireBallsToRemove.Add(fireBall);
            }
        }

        foreach (var fireBall in fireBallsToRemove)
        {
            _fireBalls.Remove(fireBall);
        }
    }
}
