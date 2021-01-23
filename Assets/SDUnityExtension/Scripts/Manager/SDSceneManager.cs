using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Created by sweetSD. (with Singleton)
/// 
/// 씬 이동을 관리하는 씬 매니저 클래스입니다.
/// </summary>
public class SDSceneManager : SDSingleton<SDSceneManager>
{
    private Coroutine _progressCoroutine = null;
    /// <summary>
    /// 씬이 로딩 되는 동안 불려지는 UnityEvent입니다. (float : 진행도)
    /// </summary>
    [SerializeField] private UnityEvent<float> _onSceneLoadProgress;
    /// <summary>
    /// 씬 로딩이 완료 되었을 때 불려지는 UnityEvent입니다. (float : 진행도)
    /// </summary>
    [SerializeField] private UnityEvent<float> _onSceneLoaded;

    private void Awake()
    {
        SetInstance(this, true);
    }

    #region Logic Functions

    /// <summary>
    /// Build Settings의 Scene index를 이용하여 씬을 로드합니다.
    /// </summary>
    /// <param name="index">Build Settings의 Scene index</param>
    public void LoadScene(int index)
    {
        var ao = SceneManager.LoadSceneAsync(index);
        if (_progressCoroutine != null)
            StopCoroutine(_progressCoroutine);
        _progressCoroutine = StartCoroutine(CO_SceneLoadProgress(ao));
    }

    /// <summary>
    /// Build Settings의 Scene name을 이용하여 씬을 로드합니다.
    /// </summary>
    /// <param name="name">Build Settings의 Scene name</param>
    public void LoadScene(string name)
    {
        var ao = SceneManager.LoadSceneAsync(name);
        if (_progressCoroutine != null)
            StopCoroutine(_progressCoroutine);
        _progressCoroutine = StartCoroutine(CO_SceneLoadProgress(ao));
    }

    /// <summary>
    /// 씬이 로딩되는 동안 이벤트를 발생시켜 줄 코루틴 함수입니다.
    /// </summary>
    /// <param name="operation">SceneManager.LoadSceneAsync의 AsyncOperation</param>
    /// <returns></returns>
    private IEnumerator CO_SceneLoadProgress(AsyncOperation operation)
    {
        while (operation.progress < 9.0f)
        {
            _onSceneLoadProgress?.Invoke(operation.progress);
            yield return null;
        }
        _onSceneLoaded?.Invoke(operation.progress);
    }

    #endregion

    #region Event Functions

    /// <summary>
    /// 씬 로딩 이벤트 함수 추가
    /// </summary>
    /// <param name="action"></param>
    public void AddProgressListener(UnityAction<float> action)
    {
        _onSceneLoadProgress.AddListener(action);
    }

    /// <summary>
    /// 씬 로딩 이벤트 함수 제거
    /// </summary>
    /// <param name="action"></param>
    public void RemoveProgressListener(UnityAction<float> action)
    {
        _onSceneLoadProgress.RemoveListener(action);
    }

    /// <summary>
    /// 씬 로딩 완료 이벤트 함수 추가
    /// </summary>
    /// <param name="action"></param>
    public void AddLoadedListener(UnityAction<float> action)
    {
        _onSceneLoaded.AddListener(action);
    }

    /// <summary>
    /// 씬 로딩 완료 이벤트 함수 제거
    /// </summary>
    /// <param name="action"></param>
    public void RemoveLoadedListener(UnityAction<float> action)
    {
        _onSceneLoaded.RemoveListener(action);
    }

    #endregion
}
