using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CreatureAudioPlayer : ObjectBehaviour
{
    [SerializeField] private AudioClip[] _hitAudioClips;
    [SerializeField] private AudioClip[] _deathAudioClips;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayHitSound()
    {
        _audioSource.clip = _hitAudioClips[Random.Range(0, _hitAudioClips.Length)];
        _audioSource.pitch = Random.Range(0.95f, 1.05f);
        _audioSource.Play();
    }

    public void PlayDeathSound()
    {
        _audioSource.clip = _deathAudioClips[Random.Range(0, _deathAudioClips.Length)];
        _audioSource.pitch = Random.Range(0.95f, 1.05f);
        _audioSource.Play();
    }
}
