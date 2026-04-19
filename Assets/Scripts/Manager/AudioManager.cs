using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;
public enum ESFXType
{
    None,
    Click,
}
public interface IAudioManager : ISingleton
{
    void PlayBGM(string musicName = "");
    void StopBGM();
    void PlaySFX(ESFXType fxType);
}

public class AudioManager : MonoSingleton<AudioManager>, IAudioManager
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private AudioClip clickClip;


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

    public void PlaySFX(ESFXType fxType)
    {
        Debug.Log($"PlaySFX: {fxType}");
        switch (fxType)
        {
            case ESFXType.Click:
                fxSource.PlayOneShot(clickClip);
                break;
            default:
                break;
        }
    }

    public void PlayBGM(string musicName = "")
    {
        musicSource.Play();
        Debug.Log($"PlayBGM: {musicName}");
    }

    public void StopBGM()
    {
        musicSource.Stop();
    }
}
