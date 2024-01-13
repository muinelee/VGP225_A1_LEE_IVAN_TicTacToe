using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    private Queue<AudioSource> threeDAudioPool;
    private Queue<AudioSource> twoDAudioPool;

    [SerializeField] private AudioSource threeDTemplate;
    [SerializeField] private AudioSource twoDTemplate;
    [SerializeField] private int numberOfPool = 15;
    [SerializeField] private AudioMixer mixer;

    protected override void Awake()
    {
        base.Awake();
        InitializeMixer();

        threeDAudioPool = new Queue<AudioSource>();
        twoDAudioPool = new Queue<AudioSource>();

        for (int i = 0; i < numberOfPool; i++)
        {
            AudioSource threeD = Instantiate(threeDTemplate, transform);
            threeDAudioPool.Enqueue(threeD);

            AudioSource twoD = Instantiate(twoDTemplate, transform);
            twoDAudioPool.Enqueue(twoD);
        }
    }

    private void OnEnable()
    {
        SettingsManager.Instance.OnSettingsChanged += InitializeMixer;
    }

    private void OnDisable()
    {
        SettingsManager.Instance.OnSettingsChanged -= InitializeMixer;
    }

    public void InitializeMixer()
    {
        SetMixerVolume("Master", SettingsManager.Instance.MasterVolume);
        SetMixerVolume("SFX", SettingsManager.Instance.SFXVolume);
        SetMixerVolume("Music", SettingsManager.Instance.MusicVolume);
    }

    public AudioSource GetTwoDimensionalSource()
    {
        AudioSource source = twoDAudioPool.Dequeue();
        twoDAudioPool.Enqueue(source);
        return source;
    }

    public void PlayAudioSFX(AudioClip clip)
    {
        AudioSource source = GetTwoDimensionalSource();
        source.clip = clip;
        source.Play();
    }

    public AudioSource GetThreeDimensionalSource(Vector3 origin)
    {
        if (threeDAudioPool.Count <= 0)
        {
            AudioSource threeD = Instantiate(threeDTemplate, transform);
            threeDAudioPool.Enqueue(threeD);
        }

        AudioSource source = threeDAudioPool.Dequeue();
        //threeDAudioPool.Enqueue(source);
        source.transform.position = origin;
        return source;
    }

    public void ReturnAudioSourceToPool(AudioSource source)
    {
        source.transform.SetParent(transform);

        source.pitch = 1;
        source.volume = 1;

        threeDAudioPool.Enqueue(source);
    }

    public void SetMixerVolume(string name, float value)
    {
        mixer.SetFloat(name, Mathf.Log10(value) * 20);
    }
}