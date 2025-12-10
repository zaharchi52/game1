using UnityEngine;
using UnityEngine.Serialization;

public abstract class FootstepPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _secondsToPlayStep = 0.4f;
    private bool _isMoving;
    private float _timer;

    public void SetMoving(bool isMoving)
    {
        _isMoving = isMoving;

        if (_isMoving==false)
        {
            _timer = _secondsToPlayStep;
        }
    }

    void Update()
    {
        if (!_isMoving)
            return;

        _timer += Time.deltaTime;
        if (_timer > _secondsToPlayStep)
        {
            _timer = 0f;
            PlayStep();
        }
    }

    protected AudioSource AudioSource => _audioSource;

    protected abstract void PlayStep();
}