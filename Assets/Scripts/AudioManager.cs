using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource footstepsSource;

    [SerializeField] private float bgmVolume = 0.1f;
    [SerializeField] private float sfxVolume = 0.4f;
    [SerializeField] private float footstepsVolume = 1.0f;

    [SerializeField] private AudioClip bgmClip;
    
    // Footstep clips for different terrains
    [SerializeField] private AudioClip[] soilFootsteps;
    [SerializeField] private AudioClip[] rockFootsteps;
    [SerializeField] private AudioClip[] waterFootsteps;
    
    [SerializeField] private AudioClip[] ambientClips;

  
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ❌ Remove this line if causing issues
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SetupAudioSources();
    }

    private void SetupAudioSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = sfxVolume;
        sfxSource.spatialBlend = 0.5f;

        footstepsSource = gameObject.AddComponent<AudioSource>();
        footstepsSource.volume = footstepsVolume;
    }

    public void PlayBGM()
    {
        if (bgmClip != null && !bgmSource.isPlaying)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
    }

    // ✅ NEW: Play footsteps based on terrain type
    public void PlayFootstep(string terrainType = "soil")
    {
        AudioClip[] selectedClips = terrainType switch
        {
            "rock" => rockFootsteps,
            "water" => waterFootsteps,
            "soil" => soilFootsteps,
            _ => soilFootsteps
        };

        if (selectedClips.Length > 0)
        {
            AudioClip randomClip = selectedClips[Random.Range(0, selectedClips.Length)];
            footstepsSource.PlayOneShot(randomClip);
        }
    }

    public void PlayProximityNoise(Vector3 position = default)
    {
        if (ambientClips.Length > 0)
        {
            AudioClip randomClip = ambientClips[Random.Range(0, ambientClips.Length)];
            
            if (position != default)
            {
                AudioSource.PlayClipAtPoint(randomClip, position, sfxVolume);
            }
            else
            {
                sfxSource.PlayOneShot(randomClip);
            }
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void SetFootstepsVolume(float volume)
    {
        footstepsVolume = Mathf.Clamp01(volume);
        footstepsSource.volume = footstepsVolume;
    }

    public void MuteAll(bool mute)
    {
        bgmSource.mute = mute;
        sfxSource.mute = mute;
        footstepsSource.mute = mute;
    }
}