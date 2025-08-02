using UnityEngine;

public class StartAudio : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartAudi()
    {
        audioSource.Play();
    }
}
