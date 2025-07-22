using UnityEngine;

public class FireBall : ObjectBehaviour
{
    public Transform Transform { get; private set; }
    public float Speed { get; private set; }
    public float DefaultDamage { get; set; }
    public float PiercingDamagePenalty { get; private set; }
    public float AttackCooldown { get; private set; }
    public float AttackCooldownDelta { get; set; }
    public int PiercingsLeft { get; set; } = 1; // Default number of piercings for the fireball

    public void Init(Transform transform, float speed = 2.2f, float startDamage = 500f, float piercingDamagePenalty = 0.8f, float attackInterval = 0.2f, int maxPiercings = 10)
    {
        Transform = transform;
        Speed = speed;
        DefaultDamage = startDamage;
        PiercingDamagePenalty = piercingDamagePenalty;
        AttackCooldown = attackInterval;
        AttackCooldownDelta = AttackCooldown;
        PiercingsLeft = maxPiercings;
    }
}
