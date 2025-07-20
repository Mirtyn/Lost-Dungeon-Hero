using UnityEngine;
using UnityEngine.AI;
public abstract class BaseEnemy : ObjectBehaviour
{
    public Transform Transform { get; protected set; }
    public NavMeshAgent NavMeshAgent { get; protected set; }
    public Renderer[] Renderers { get; protected set; }
    [SerializeField] protected Transform[] BodyParts;
    public CreatureAudioPlayer AudioPlayer;
    public float Health = 100f;
    public float MoveSpeed = 1.5f;
    public float RotateSpeed = 100f;
    public int AIPriority = 50;
    public float PowerReward = 5f;
    public float AttackCooldown = 1f;
    public float AttackCooldownDelta = 4f;
    public float AttackDamage = 1f;
    public float AttackDistance = 0.6f;
    public bool IsIdle = false;

    public abstract void Init(Transform transform, float health, float moveSpeed, float rotateSpeed, int aiPriority, float energyReward, float attackCooldown, float attackDamage, float attackDistance);

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        foreach (var renderer in Renderers)
        {
            renderer.material.color = Color.red;
        }
        Invoke(nameof(UndoDamageColor), 0.15f);
    }

    private void UndoDamageColor()
    {
        foreach (var renderer in Renderers)
        {
            renderer.material.color = Color.white;
        }
    }
}