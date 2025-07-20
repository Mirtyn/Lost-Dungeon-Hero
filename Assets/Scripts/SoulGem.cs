using UnityEngine;

public class SoulGem : MonoBehaviour
{
    public Transform Transform { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    public void Init()
    {
        Transform = this.transform;
        Rigidbody = GetComponent<Rigidbody>();
    }
}
