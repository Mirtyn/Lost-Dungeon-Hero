using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CreatureAnimator : ObjectBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayDeath()
    {
        _animator.SetBool("IsDead", true);
    }
}
