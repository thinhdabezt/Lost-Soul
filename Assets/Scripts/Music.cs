using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource introSource;
    public AudioSource loopSource;
    void Start()
    {
        introSource.Play();
        loopSource.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);
    }
}
