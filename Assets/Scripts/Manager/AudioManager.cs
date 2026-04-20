using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Core;
using UnityEngine;
public enum ESFXType
{
    None,
    Click,
    Password,
    TrunLight,
    RedLight,
}
public enum EBGMType
{
    None,
    Start,
    Main,
    End,
}
public interface IAudioManager : ISingleton
{
    void PlayBGM(EBGMType bgmType = EBGMType.None);
    void StopBGM();
    void PlaySFX(ESFXType fxType);
}

public class AudioManager : MonoSingleton<AudioManager>, IAudioManager
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private List<AudioClip> musicClips;
    [SerializeField] private List<AudioClip> sfxClips;


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
                fxSource.PlayOneShot(sfxClips[0]);
                break;
            case ESFXType.Password:
                fxSource.PlayOneShot(sfxClips[1]);
                break;
            case ESFXType.TrunLight:
                fxSource.PlayOneShot(sfxClips[2]);
                break;
            case ESFXType.RedLight:
                fxSource.PlayOneShot(sfxClips[3]);
                break;
            default:
                break;
        }
    }

    public void PlayBGM(EBGMType bgmType = EBGMType.None)
    {
        AudioClip clipToPlay = bgmType switch
        {
            EBGMType.Start => musicClips[0],
            EBGMType.Main => musicClips[1],
            EBGMType.End => musicClips[2],
            _ => null
        };
        if (!musicSource.isPlaying)
        {
            musicSource.clip = clipToPlay;
            musicSource.Play();
            return;
        }
        musicSource.DOFade(0f, 1f).OnComplete(() =>
        {

            if (clipToPlay != null)
            {
                musicSource.clip = clipToPlay;
                musicSource.Play();
                musicSource.volume = 1f;
            }
        });
    }

    public void StopBGM()
    {
        musicSource.Stop();
    }
}
