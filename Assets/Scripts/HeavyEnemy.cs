using UnityEngine;
using UnityEngine.AI;

public class HeavyEnemy : BaseEnemy
{
    public const float DEFAULT_HEALTH = 400f;
    public const float DEFAULT_MOVE_SPEED = .75f;
    public const float DEFAULT_ROTATE_SPEED = 100f;
    public const int DEFAULT_AI_PRIORITY = 30;
    public const float DEFAULT_ENERGY_REWARD = 3f;
    public const float DEFAULT_ATTACK_COOLDOWN = 2f;
    public const float DEFAULT_ATTACK_DAMAGE = 8f;
    public const float DEFAULT_ATTACK_DISTANCE = 0.4f;

    public const float CORRUPT_HEALTH = 800f;
    public const float CORRUPT_MOVE_SPEED = 1f;
    public const float CORRUPT_ROTATE_SPEED = 200f;
    public const int CORRUPT_AI_PRIORITY = 29;
    public const float CORRUPT_ENERGY_REWARD = 6f;
    public const float CORRUPT_ATTACK_COOLDOWN = 1f;
    public const float CORRUPT_ATTACK_DAMAGE = 15f;
    public const float CORRUPT_ATTACK_DISTANCE = 0.4f;

    public override void Init(
        Transform transform,
        float health = DEFAULT_HEALTH,
        float moveSpeed = DEFAULT_MOVE_SPEED,
        float rotateSpeed = DEFAULT_ROTATE_SPEED,
        int aiPriority = DEFAULT_AI_PRIORITY,
        float energyReward = DEFAULT_ENERGY_REWARD,
        float attackCooldown = DEFAULT_ATTACK_COOLDOWN,
        float attackDamage = DEFAULT_ATTACK_DAMAGE,
        float attackDistance = DEFAULT_ATTACK_DISTANCE,
        bool corrupt = false)
    {
        Initialize(transform, health, moveSpeed, rotateSpeed, aiPriority, energyReward, attackCooldown, attackDamage, attackDistance, corrupt);
    }
}
