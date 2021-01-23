using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// 데이터를 암호화 / 복호화하여 데이터를 관리하는 List입니다.
/// SDSecurityManager의 싱글톤이 씬 내에 존재하여야합니다. 
/// int / float / string / double / decimal 을 지원합니다.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SDSecurityList
{
    private List<string> _internalList = null;

    public SDSecurityList()
    {
        _internalList = new List<string>();
    }

    #region List Functions Override

    public int Count => _internalList.Count;

    /// <summary>
    /// [중요] 인덱서 연산자를 통해 get한 string은 T 형식으로 파싱하여 사용해주세요.
    /// 인덱서 연산자를 통해 set할 경우에는 자동으로 저장됩니다.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string this[int key]
    {
        get => SDSecurityManager.I.Decrypt(_internalList[key]);
        set => _internalList[key] = SDSecurityManager.I.Encrypt(value);
    }

    public void Add<T>(T value)
    {
        _internalList.Add(SDSecurityManager.I.Encrypt(value.ToString()));
    }

    public void AddRange<T>(IEnumerable<T> values)
    {
        _internalList.AddRange(values.Select((value) => SDSecurityManager.I.Encrypt(value.ToString())));
    }

    public void Remove<T>(T value)
    {
        _internalList.Remove(SDSecurityManager.I.Encrypt(value.ToString()));
    }

    public void RemoveAt(int index)
    {
        _internalList.RemoveAt(index);
    }

    public void Clear()
    {
        _internalList.Clear();
    }

    public void Reverse()
    {
        _internalList.Reverse();
    }

    #endregion

    #region Get Functions

    public int GetIntValue(int index)
    {
        return int.Parse(this[index]);
    }

    public float GetFloatValue(int index)
    {
        return float.Parse(this[index]);
    }

    public string GetStringValue(int index)
    {
        return this[index];
    }

    public double GetDoubleValue(int index)
    {
        return double.Parse(this[index]);
    }

    public decimal GetDecimalValue(int index)
    {
        return decimal.Parse(this[index]);
    }

    #endregion

}
