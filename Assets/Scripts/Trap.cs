using UnityEngine;

public class Trap : ObjectBehaviour
{
    public Transform Transform { get; private set; }
    public float AttackCooldown { get; private set; }
    public float AttackCooldownDelta { get; set; }
    public float LifeTime { get; set; } = 12f; // Default lifetime for the trap
    public float Damage { get; private set; }

    public void Init(Transform transform, float attackCooldown = 0.15f, float damage = 20f)
    {
        AttackCooldown = attackCooldown;
        AttackCooldownDelta = AttackCooldown;
        Damage = damage;
        Transform = transform;
    }
}
