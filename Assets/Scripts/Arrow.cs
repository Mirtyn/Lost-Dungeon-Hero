using UnityEngine;

public class Arrow : ObjectBehaviour
{
    public Transform Transform { get; private set; }
    public float Speed { get; private set; }
    public float Damage { get; private set; }

    public void Init(float speed, float damage, Transform transform)
    {
        Speed = speed;
        Damage = damage;
        Transform = transform;
    }
}
