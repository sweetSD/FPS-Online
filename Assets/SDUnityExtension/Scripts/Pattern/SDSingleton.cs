using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by sweetSD.
/// 
/// 디자인 패턴 중 하나인 싱글톤 패턴입니다. SDSingleton<TYPE> 을 상속받아 사용하세요.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SDSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T I
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<T>();
            if (_instance == null)
            {
                throw new System.Exception($"{typeof(T)}의 인스턴스를 찾을 수 없습니다.");
            }
            return _instance;
        }
    }

    protected void SetInstance(T inst, bool dontDestroyOnLoad = true)
    {
        if (_instance != null)
            throw new System.Exception($"{typeof(T)}의 인스턴스가 이미 설정되어있습니다. 두개 이상의 싱글톤를 배치하셨나요?");
        _instance = inst;
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(_instance);
    }

}
