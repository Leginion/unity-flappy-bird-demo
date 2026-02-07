using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音频管理器 - 单例模式
/// 使用方式：
///   AudioManager.PlaySFX("bird/jump");
///   AudioManager.PlayMusic("bgm/main");
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    Debug.LogError("[AudioManager] 场景中没有 AudioManager！");
                }
            }
            return instance;
        }
    }

    #region Configs

    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("音频源")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("音效配置")]
    [SerializeField] private SoundClip[] soundClips;

    [Header("音量设置")]
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.5f;

    private Dictionary<string, SoundClip> soundDictionary;

    #endregion

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSounds();
    }

    void InitializeSounds()
    {
        soundDictionary = new Dictionary<string, SoundClip>();

        foreach (var sound in soundClips)
        {
            if (!string.IsNullOrEmpty(sound.name) && sound.clip != null)
            {
                soundDictionary[sound.name] = sound;
            }
        }
    }

    public static void PlaySFX(string soundName, float volumeScale = 1f)
    {
        if (Instance == null || Instance.sfxSource == null) return;

        if (Instance.soundDictionary.TryGetValue(soundName, out SoundClip sound))
        {
            float finalVolume = sound.volume * volumeScale * Instance.sfxVolume;
            Instance.sfxSource.PlayOneShot(sound.clip, finalVolume);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] 音效 '{soundName}' 不存在！");
        }
    }

    public static void PlaySFXFrom(string soundName, float startMs, float volumeScale = 1f)
    {
        if (Instance == null || Instance.sfxSource == null) return;

        if (!Instance.soundDictionary.TryGetValue(soundName, out SoundClip sound) || sound.clip == null)
        {
            Debug.LogWarning($"[AudioManager] 音效 '{soundName}' 不存在！");
            return;
        }

        var clip = sound.clip;

        // 计算起播时间（秒）
        float startSec = Mathf.Max(0f, startMs / 1000f);
        if (startSec >= clip.length)
        {
            Debug.LogWarning($"[AudioManager] '{soundName}' 起播位置 {startMs}ms 超过音频长度 {clip.length * 1000f:F0}ms");
            return;
        }

        float finalVolume = sound.volume * volumeScale * Instance.sfxVolume;

        // 注意：不能用 PlayOneShot。需要直接设置 clip 和 time 再 Play
        Instance.sfxSource.Stop();
        Instance.sfxSource.clip = clip;
        Instance.sfxSource.volume = finalVolume;
        Instance.sfxSource.loop = false;

        // 用 timeSamples 更精确
        int startSample = Mathf.Clamp(
            Mathf.RoundToInt(startSec * clip.frequency),
            0,
            clip.samples - 1
        );

        Instance.sfxSource.timeSamples = startSample;
        Instance.sfxSource.Play();
    }

    public static void PlayMusic(string musicName, bool loop = true)
    {
        if (Instance == null || Instance.musicSource == null) return;

        if (Instance.soundDictionary.TryGetValue(musicName, out SoundClip sound))
        {
            Instance.musicSource.clip = sound.clip;
            Instance.musicSource.volume = sound.volume * Instance.musicVolume;
            Instance.musicSource.loop = loop;
            Instance.musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] 音乐 '{musicName}' 不存在！");
        }
    }

    public static void StopMusic()
    {
        if (Instance != null && Instance.musicSource != null)
        {
            Instance.musicSource.Stop();
        }
    }

    public static void SetSFXVolume(float volume)
    {
        if (Instance != null)
        {
            Instance.sfxVolume = Mathf.Clamp01(volume);
        }
    }

    public static void SetMusicVolume(float volume)
    {
        if (Instance != null)
        {
            Instance.musicVolume = Mathf.Clamp01(volume);
            if (Instance.musicSource != null)
            {
                Instance.musicSource.volume = Instance.musicVolume;
            }
        }
    }
}
