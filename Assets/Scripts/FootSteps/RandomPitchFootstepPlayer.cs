using UnityEngine;

public class RandomPitchFootstepPlayer : FootstepPlayer
{
    [SerializeField] private AudioClip _clip;
    [SerializeField] private float _minPitch = 0.9f;
    [SerializeField] private float _maxPitch = 1.1f;

    protected override void PlayStep()
    {
        if (_clip == null || AudioSource == null) return;

        AudioSource.pitch = Random.Range(_minPitch, _maxPitch);
        AudioSource.PlayOneShot(_clip);
    }
}