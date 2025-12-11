using UnityEngine;
using UnityEngine.UI;

public class SoundsAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    void Update()
    {
        audioSource.PlayOneShot(audioClip);
    }
}

