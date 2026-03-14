using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }
    public bool IsMuted => isMuted;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Music / UI")]
    [SerializeField] private AudioClip mainMenuMusicClip;
    [SerializeField] private AudioClip roundStartClip;
    [SerializeField] private AudioClip defeatClip;

    [Header("Tower Shoot")]
    [SerializeField] private AudioClip archerShootClip;
    [SerializeField] private AudioClip mageShootClip;
    [SerializeField] private AudioClip freezerShootClip;
    [SerializeField] private AudioClip cannonShootClip;

    [Header("Enemy Hit")]
    [SerializeField] private AudioClip goblinHitClip;
    [SerializeField] private AudioClip orcHitClip;
    [SerializeField] private AudioClip ghostHitClip;

    [Header("Enemy Death")]
    [SerializeField] private AudioClip goblinDeathClip;
    [SerializeField] private AudioClip orcDeathClip;
    [SerializeField] private AudioClip ghostDeathClip;
    [Header("Base")]
    [SerializeField] private AudioClip baseHitClip;

    [Header("Volumes")]
    [SerializeField] [Range(0f, 1f)] private float shootVolume = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float enemyHitVolume = 0.45f;
    [SerializeField] [Range(0f, 1f)] private float deathVolume = 0.6f;
    [SerializeField] [Range(0f, 1f)] private float baseHitVolume = 0.75f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float roundStartVolume = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float defeatVolume = 0.9f;
    [SerializeField] private bool startMuted;

    private bool isMuted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        sfxSource.playOnAwake = false;

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        SetMuted(startMuted);
    }

    public void PlayTowerShoot(TowerType towerType)
    {
        AudioClip clip = towerType switch
        {
            TowerType.Archer => archerShootClip,
            TowerType.Mage => mageShootClip,
            TowerType.Freezer => freezerShootClip,
            TowerType.Cannon => cannonShootClip,
            _ => null
        };

        PlayOneShot(clip, shootVolume);
    }

    public void PlayEnemyHit(EnemyType enemyType)
    {
        AudioClip clip = enemyType switch
        {
            EnemyType.Goblin => goblinHitClip,
            EnemyType.Orc => orcHitClip,
            EnemyType.Ghost => ghostHitClip,
            _ => null
        };

        PlayOneShot(clip, enemyHitVolume);
    }

    public void PlayEnemyDeath(EnemyType enemyType)
    {
        AudioClip clip = enemyType switch
        {
            EnemyType.Goblin => goblinDeathClip,
            EnemyType.Orc => orcDeathClip,
            EnemyType.Ghost => ghostDeathClip,
            _ => null
        };

        PlayOneShot(clip, deathVolume);
    }

    public void PlayBaseHit()
    {
        PlayOneShot(baseHitClip, baseHitVolume);
    }

    public void PlayRoundStart()
    {
        PlayOneShot(roundStartClip, roundStartVolume);
    }

    public void PlayDefeat()
    {
        PlayOneShot(defeatClip, defeatVolume);
    }

    public void PlayMainMenuMusic()
    {
        if (musicSource == null || mainMenuMusicClip == null)
        {
            return;
        }

        if (musicSource.clip == mainMenuMusicClip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = mainMenuMusicClip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;

        if (sfxSource != null)
        {
            sfxSource.mute = muted;
        }

        if (musicSource != null)
        {
            musicSource.mute = muted;
        }

        AudioListener.pause = muted;
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
