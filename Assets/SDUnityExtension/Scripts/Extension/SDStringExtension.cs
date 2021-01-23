using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by sweetSD.
/// 
/// String 클래스를 좀 더 편리하게 사용할 수 있도록 도움을 주는 함수가 선언된 Extension 클래스입니다.
/// </summary>
public static class SDStringExtension
{
    public static bool IsNotEmpty(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    public static bool IsEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }
}
