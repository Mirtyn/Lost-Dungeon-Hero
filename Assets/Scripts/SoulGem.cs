using UnityEngine;

public class SoulGem : MonoBehaviour
{
    public Transform Transform { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public Light Light { get; private set; }
    [field: SerializeField] public GameObject Visual { get; private set; }
    public float RemainingLifeTime { get; set; } = 15f; // Default lifetime for the soul gem
    public bool Visible { get; set; } = true;
    public float PreviousVisualStateChangeTime { get; set; } = 15f;
    public void Init()
    {
        Transform = this.transform;
        Rigidbody = GetComponent<Rigidbody>();
        if (Light == null)
        {
            Light = GetComponentInChildren<Light>();
        }
    }

    //public enum SoulGemVisualState
    //{
    //    Visible,
    //    Invisible,
    //}
}
