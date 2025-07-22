using UnityEngine;
using UnityEngine.AI;

public class FodderEnemy : BaseEnemy
{
    public const float DEFAULT_HEALTH = 100f;
    public const float DEFAULT_MOVE_SPEED = 1.15f;
    public const float DEFAULT_ROTATE_SPEED = 150f;
    public const int DEFAULT_AI_PRIORITY = 50;
    public const float DEFAULT_ENERGY_REWARD = 1f;
    public const float DEFAULT_ATTACK_COOLDOWN = 1f;
    public const float DEFAULT_ATTACK_DAMAGE = 2f;
    public const float DEFAULT_ATTACK_DISTANCE = 0.4f;

    public const float CORRUPT_HEALTH = 200f;
    public const float CORRUPT_MOVE_SPEED = 1.5f;
    public const float CORRUPT_ROTATE_SPEED = 200f;
    public const int CORRUPT_AI_PRIORITY = 49;
    public const float CORRUPT_ENERGY_REWARD = 2f;
    public const float CORRUPT_ATTACK_COOLDOWN = 0.5f;
    public const float CORRUPT_ATTACK_DAMAGE = 3f;
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
