using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

public interface IAudioManager : ISingleton
{
    void PlayBGM(string musicName);
    void StopBGM(string musicName);
    void PlaySFX(string fxName);
}

public class AudioManager : MonoSingleton<AudioManager>, IAudioManager
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource fxSource;


    public void OnSingletonInit()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (fxSource == null)
        {
            fxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySFX(string fxName)
    {
        Debug.Log($"PlaySFX: {fxName}");
    }

    public void PlayBGM(string musicName)
    {
        Debug.Log($"PlayBGM: {musicName}");
    }

    public void StopBGM(string musicName)
    {
        Debug.Log($"StopBGM: {musicName}");
    }
}
