using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by sweetSD.
/// 
/// GameObject 클래스의 Extension 함수들을 모아둔 static class 입니다.
/// </summary>
public static class GameObjectExtension
{
    /// <summary>
    /// GameObject의 SetActive를 seconds의 딜레이를 가지고 수행합니다.
    /// </summary>
    /// <param name="obj">설정 GameObject</param>
    /// <param name="active">GameObject 활성화 여부</param>
    /// <param name="seconds">active 상태 변경 딜레이 (초)</param>
    public static void SetActive(this GameObject obj, bool active, float seconds = 0f)
    {
        SDObjectManager.I.SetActiveAfterSeconds(obj, active, seconds);
    }
}

/// <summary>
/// Created by sweetSD. (with Singleton)
/// 
/// 모든 게임오브젝트를 관리하는 매니저 클래스입니다.
/// </summary>
public class SDObjectManager : SDSingleton<SDObjectManager>
{
    private Dictionary<GameObject, Coroutine> _setActiveCoroutines = new Dictionary<GameObject, Coroutine>();

    /// <summary>
    /// seconds 초 뒤에 해당 obj를 active 상태로 바꿉니다.
    /// </summary>
    /// <param name="obj">설정 GameObject</param>
    /// <param name="active">GameObject 활성화 여부</param>
    /// <param name="seconds">active 상태 변경 딜레이 (초)</param>
    public void SetActiveAfterSeconds(GameObject obj, bool active, float seconds = 0f)
    {
        _setActiveCoroutines[obj] = StartCoroutine(CO_SetActiveAfterSeconds(obj, active, seconds));
    }

    /// <summary>
    /// 대기중인 SetActive 코루틴을 종료합니다.
    /// </summary>
    /// <param name="obj"></param>
    public void StopSetActive(GameObject obj)
    {
        if (_setActiveCoroutines.ContainsKey(obj) && _setActiveCoroutines[obj] != null)
            StopCoroutine(_setActiveCoroutines[obj]);
    }

    /// <summary>
    /// SetActiveAfterSeconds() 의 기능을 수행하는 코루틴 함수입니다.
    /// </summary>
    /// <param name="obj">설정 GameObject</param>
    /// <param name="active">GameObject 활성화 여부</param>
    /// <param name="seconds">active 상태 변경 딜레이 (초)</param>
    /// <returns></returns>
    IEnumerator CO_SetActiveAfterSeconds(GameObject obj, bool active, float seconds = 0f)
    {
        float eleapsed = 0f;
        while((eleapsed += Time.deltaTime) < seconds) { yield return null; }
        obj.SetActive(active);
    }
}
