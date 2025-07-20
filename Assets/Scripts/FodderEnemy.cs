using UnityEngine;
using UnityEngine.AI;

public class FodderEnemy : BaseEnemy
{
    public override void Init(Transform transform, float health = 100f, float moveSpeed = 1.15f, float rotateSpeed = 150f, int aiPriority = 50, float energyReward = 1f, float attackCooldown = 1f, float attackDamage = 2f, float attackDistance = .4f)
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
