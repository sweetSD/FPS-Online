using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by sweetSD. (with Singleton)
/// 
/// 사운드 관련 기능을 제공하는 사운드 매니저 클래스입니다.
/// 
/// </summary>
public class SDAudioManager : SDSingleton<SDAudioManager>
{
    public void FadeIn(AudioSource source, float duration, float volume = 1f)
    {
        StartCoroutine(CO_FadeIn(source, duration, volume));
    }

    public void FadeOut(AudioSource source, float duration, float volume = 0f)
    {
        StartCoroutine(CO_FadeOut(source, duration, volume));
    }

    public IEnumerator CO_FadeIn(AudioSource source, float duration, float volume)
    {
        float eleapsedTime = 0f;
        float beginVolume = 0;
        source.Play();
        while (eleapsedTime < duration)
        {
            eleapsedTime += Time.deltaTime;
            source.volume = SDMath.Map(eleapsedTime, 0, duration, beginVolume, volume);
            yield return null;
        }
        source.volume = volume;
    }

    public IEnumerator CO_FadeOut(AudioSource source, float duration, float volume)
    {
        float eleapsedTime = 0f;
        float beginVolume = 1;
        while (eleapsedTime < duration)
        {
            eleapsedTime += Time.deltaTime;
            source.volume = beginVolume - SDMath.Map(eleapsedTime, 0, duration, 0, 1 - volume);
            yield return null;
        }
        source.volume = volume;
        source.Stop();
    }
}
