using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Clips")]
    public AudioClip coinSFX;
    public AudioClip healSFX;
    public AudioClip specialHitSFX;
    [Range(0f, 1f)]
    public float sfxVolume = 0.25f;

    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayCoinSFX() => PlaySFX(coinSFX);
    public void PlayHealSFX() => PlaySFX(healSFX);
    public void PlaySpecialHitSFX() => PlaySFX(specialHitSFX);
}
