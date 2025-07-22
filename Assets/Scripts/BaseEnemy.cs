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
    public Color ThisNormalColor = NormalColor;

    public const float CORRUPT_COLOR_R = 1f;
    public const float CORRUPT_COLOR_G = .15f;
    public const float CORRUPT_COLOR_B = .6f;

    public static readonly Color CorruptColor = new Color(CORRUPT_COLOR_R, CORRUPT_COLOR_G, CORRUPT_COLOR_B);
    public static readonly Color NormalColor = Color.white;
    public static readonly Color DamageColor = Color.red;

    public abstract void Init(Transform transform, float health, float moveSpeed, float rotateSpeed, int aiPriority, float energyReward, float attackCooldown, float attackDamage, float attackDistance, bool corrupt);

    protected void Initialize(Transform transform, float health, float moveSpeed, float rotateSpeed, int aiPriority, float energyReward, float attackCooldown, float attackDamage, float attackDistance, bool corrupt)
    {
        Transform = transform;
        NavMeshAgent = transform.GetComponent<NavMeshAgent>();
        Renderers = new Renderer[BodyParts.Length];
        AudioPlayer = transform.GetComponent<CreatureAudioPlayer>();

        if (corrupt)
        {
            ThisNormalColor = CorruptColor;
        }
        else
        {
            transform.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
        }

        for (int i = 0; i < BodyParts.Length; i++)
        {
            Renderers[i] = BodyParts[i].GetComponent<Renderer>();

            Renderers[i].material.color = ThisNormalColor;
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

        // The lower the priority the more important they are.
        NavMeshAgent.avoidancePriority = AIPriority;
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        foreach (var renderer in Renderers)
        {
            renderer.material.color = DamageColor;
        }
        Invoke(nameof(UndoDamageColor), 0.15f);
    }

    private void UndoDamageColor()
    {
        foreach (var renderer in Renderers)
        {
            renderer.material.color = ThisNormalColor;
        }
    }
}