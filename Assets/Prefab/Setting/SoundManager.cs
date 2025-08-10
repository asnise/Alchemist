using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource soundEffectSourcePlayer;
    public AudioSource soundEffectSource;
    public AudioSource soundtrackSource;
    public AudioSource backgroundMusicSource;

    [Header("Volume Settings")]
    [Range(0, 1)] public float MasterVolume = 1f;
    [Range(0, 1)] public float soundEffectVolume = 1f;
    [Range(0, 1)] public float soundEffectPlayerVolume = 1f;
    [Range(0, 1)] public float soundtrackVolume = 0.8f;
    [Range(0, 1)] public float backgroundMusicVolume = 0.6f;

    [Header("UI Sliders")]
    public Slider soundEffectSlider;
    public Slider soundEffectPlayerSlider;
    public Slider soundtrackSlider;
    public Slider backgroundMusicSlider;

    [Header("Background Music Clips")]
    public List<AudioClip> backgroundMusicClips;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!soundEffectSource || !soundtrackSource || !backgroundMusicSource)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("MainCamera").transform.GetChild(0).gameObject;
            soundEffectSource = obj.GetComponent<AudioSource>();
            soundtrackSource = obj.GetComponent<AudioSource>();
            backgroundMusicSource = obj.GetComponent<AudioSource>();
        }

        LoadSettings();
        ApplyVolumeSettings();
        PlayRandomBackgroundMusic();

        // Initialize slider values and listeners
        soundEffectSlider.value = soundEffectVolume;
        soundEffectPlayerSlider.value = soundEffectPlayerVolume;
        soundtrackSlider.value = soundtrackVolume;
        backgroundMusicSlider.value = backgroundMusicVolume;

        soundEffectSlider.onValueChanged.AddListener(value => {
            soundEffectVolume = value;
            ApplyVolumeSettings();
            SaveSettings();
        });

        soundEffectPlayerSlider.onValueChanged.AddListener(value => {
            soundEffectPlayerVolume = value;
            ApplyVolumeSettings();
            SaveSettings();
        });

        soundtrackSlider.onValueChanged.AddListener(value => {
            soundtrackVolume = value;
            ApplyVolumeSettings();
            SaveSettings();
        });

        backgroundMusicSlider.onValueChanged.AddListener(value => {
            backgroundMusicVolume = value;
            ApplyVolumeSettings();
            SaveSettings();
        });
    }

    private void Update()
    {
        if (!backgroundMusicSource.isPlaying)
        {
            PlayRandomBackgroundMusic();
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        soundEffectSource.PlayOneShot(clip, soundEffectVolume);
    }

    public void PlaySoundEffectOnPlayer(AudioClip clip)
    {
        soundEffectSourcePlayer.PlayOneShot(clip, soundEffectPlayerVolume);
    }

    public void PlaySoundtrack(AudioClip clip)
    {
        if (soundtrackSource.clip != clip)
        {
            soundtrackSource.clip = clip;
            soundtrackSource.loop = true;
            soundtrackSource.Play();
        }
    }

    public void StopSoundtrack()
    {
        soundtrackSource.Stop();
        soundtrackSource.clip = null;
    }

    public void ApplyVolumeSettings()
    {
        soundEffectSource.volume = soundEffectVolume;
        soundEffectSourcePlayer.volume = soundEffectPlayerVolume;
        soundtrackSource.volume = soundtrackVolume;
        backgroundMusicSource.volume = backgroundMusicVolume;
    }

    public void PlayRandomBackgroundMusic()
    {
        if (backgroundMusicClips.Count == 0) return;

        int randomIndex = Random.Range(0, backgroundMusicClips.Count);
        backgroundMusicSource.clip = backgroundMusicClips[randomIndex];
        backgroundMusicSource.loop = false;
        backgroundMusicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        backgroundMusicSource.Stop();
        backgroundMusicSource.clip = null;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("SoundEffectVolume", soundEffectVolume);
        PlayerPrefs.SetFloat("SoundEffectPlayerVolume", soundEffectPlayerVolume);
        PlayerPrefs.SetFloat("SoundtrackVolume", soundtrackVolume);
        PlayerPrefs.SetFloat("BackgroundMusicVolume", backgroundMusicVolume);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        soundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume", 1f);
        soundEffectPlayerVolume = PlayerPrefs.GetFloat("SoundEffectPlayerVolume", 1f);
        soundtrackVolume = PlayerPrefs.GetFloat("SoundtrackVolume", 0.8f);
        backgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.6f);
    }


}
