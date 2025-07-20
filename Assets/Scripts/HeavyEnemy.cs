using UnityEngine;
using UnityEngine.AI;

public class HeavyEnemy : BaseEnemy
{
    public override void Init(Transform transform, float health = 400f, float moveSpeed = .75f, float rotateSpeed = 100f, int aiPriority = 30, float energyReward = 3f, float attackCooldown = 2f, float attackDamage = 8f, float attackDistance = .4f)
    {
        Transform = transform;
        NavMeshAgent = transform.GetComponent<NavMeshAgent>();
        Renderers = new Renderer[BodyParts.Length];
        AudioPlayer = transform.GetComponent<CreatureAudioPlayer>();
        for (int i = 0; i < BodyParts.Length; i++)
        {
            Renderers[i] = BodyParts[i].GetComponent<Renderer>();
        }
        Health = health;
        MoveSpeed = moveSpeed;
        RotateSpeed = rotateSpeed;
        AIPriority = aiPriority;
        PowerReward = energyReward;
        AttackCooldown = attackCooldown;
        AttackCooldownDelta = attackCooldown * 2;
        AttackDamage = attackDamage;
        AttackDistance = attackDistance;

        NavMeshAgent.speed = MoveSpeed;
        NavMeshAgent.angularSpeed = RotateSpeed;
        NavMeshAgent.avoidancePriority = AIPriority;
    }
}
