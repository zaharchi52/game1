using UnityEngine;

public class SequentialFootstepPlayer : FootstepPlayer
{
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private int _currentIndex;

    protected override void PlayStep()
    {
        if (AudioSource == null || _clips == null || _clips.Length == 0) return;

        if (_currentIndex >= _clips.Length)
            _currentIndex = 0;

        var clip = _clips[_currentIndex];
        _currentIndex++;

        if (clip == null) return;

        AudioSource.PlayOneShot(clip);
    }
}