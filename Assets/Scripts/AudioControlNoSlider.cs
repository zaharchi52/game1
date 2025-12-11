using UnityEngine;
using UnityEngine.UI;

public class AudioControlNoSlider : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
