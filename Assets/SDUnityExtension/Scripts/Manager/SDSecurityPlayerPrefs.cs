using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by sweetSD. (with Singleton)
/// 
/// Unity의 PlayerPrefs를 좀 더 안전하게 사용할 수 있도록
/// AES256을 이용하여 복호화하여 저장합니다.
/// 
/// </summary>
public class SDSecurityPlayerPrefs : SDSingleton<SDSecurityPlayerPrefs>
{
    private void Awake()
    {
        SetInstance(this, true);
    }

    #region Utility Functions

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(SDSecurityManager.I.Encrypt(key));
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(SDSecurityManager.I.Encrypt(key));
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }

    #endregion

    #region Setter Functions

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(SDSecurityManager.I.Encrypt(key), value);
    }

    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(SDSecurityManager.I.Encrypt(key), value);
    }

    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(SDSecurityManager.I.Encrypt(key), value);
    }

    #endregion

    #region Getter Functions

    public int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(SDSecurityManager.I.Decrypt(key), defaultValue);
    }

    public string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(SDSecurityManager.I.Decrypt(key), defaultValue);
    }

    public float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(SDSecurityManager.I.Decrypt(key), defaultValue);
    }

    #endregion
}
