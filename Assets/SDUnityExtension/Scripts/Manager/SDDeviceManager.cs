using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Created by sweetSD. (with Singleton)
/// 
/// 디바이스 정보 매니저 스크립트입니다.
/// 
/// </summary>

public enum E_TOAST
{
    LENGTH_SHORT = 0,
    LENGTH_LONG = 1,
}

public class SDDeviceManager : SDSingleton<SDDeviceManager>
{
    [Header("Device")]

    /// <summary>
    /// 목표 FPS입니다.
    /// </summary>
    [Tooltip("목표 FPS")]
    [SerializeField] private int _targetFPS = 60;
    public int TargetFPS
    {
        get => _targetFPS;
        set
        {
            _targetFPS = value;
            Application.targetFrameRate = _targetFPS;
        }
    }

    /// <summary>
    /// 백그라운드 실행 여부
    /// </summary>
    [Tooltip("백그라운드 실행 여부")]
    [SerializeField] private bool _isRunInBackground = true;
    public bool runInBackground
    {
        get => _isRunInBackground;
        set
        {
            _isRunInBackground = value;
            Application.runInBackground = _isRunInBackground;
        }
    }

    /// <summary>
    /// 절전모드 실행 여부
    /// </summary>
    [Tooltip("절전모드 실행 여부")]
    [SerializeField] private bool _isNeverSleep = true;
    public bool isNeverSleep
    {
        get => _isNeverSleep;
        set
        {
            _isNeverSleep = value;
            Screen.sleepTimeout = _isNeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
        }
    }

    /// <summary>
    /// 화면 회전
    /// </summary>
    [Tooltip("화면 회전")]
    [SerializeField] private ScreenOrientation _screenOrientation = ScreenOrientation.Landscape;
    public ScreenOrientation ScreenOrientation
    {
        get => _screenOrientation;
        set
        {
            _screenOrientation = value;
            Screen.orientation = _screenOrientation;
        }
    }

    /// <summary>
    /// 화면 가로 길이
    /// </summary>
    public float ScreenWidth => Screen.width;

    /// <summary>
    /// 화면 세로 길이
    /// </summary>
    public float ScreenHeight => Screen.height;

    /// <summary>
    /// 기능 접근 권한 (필수적, 선택적)
    /// 
    /// Plugins/Android/AndroidManifest.xml 파일에 해당 권한을 추가해주세요.
    /// 
    /// ex: <uses-permission android:name="android.permission.CAMERA" />
    /// 
    /// Camera - android.permission.CAMERA
    /// Record Audio - android.permission.RECORD_AUDIO
    /// Fine Location - android.permission.ACCESS_FINE_LOCATION
    /// Coarse Location - android.permission.ACCESS_COARSE_LOCATION
    /// External Storage Read - android.permission.READ_EXTERNAL_STORAGE
    /// External Storage Write - android.permission.WRITE_EXTERNAL_STORAGE
    /// </summary>
    [Tooltip("필수 접근 권한 (ex. android.permission.READ_EXTERNAL_STORAGE)")]
    [SerializeField] private string[] _requiredPermission;
    [Tooltip("선택 접근 권한 (ex. android.permission.READ_EXTERNAL_STORAGE)")]
    [SerializeField] private string[] _optionalPermission;

    private void Awake()
    {
        SetInstance(this, true);

        Application.targetFrameRate = _targetFPS;
        Application.runInBackground = _isRunInBackground;
        Screen.sleepTimeout = _isNeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;

        Screen.orientation = _screenOrientation;

        if (Application.platform == RuntimePlatform.Android)
        {
            InitializeAndroidObjects();


            RequestRequiredPermissions();

            RequestOptionalPermissions();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if(!pause)
        {
            RequestRequiredPermissions();
        }
    }

    #region Logic Functions

    /// <summary>
    /// 필수적 접근 권한을 사용자에게 요청합니다.
    /// (필수적 권한이 취소되면 권한 세팅 페이지로 이동합니다.)
    /// </summary>
    private void RequestRequiredPermissions()
    {
        if (_requiredPermission == null || _requiredPermission.Length == 0)
            return;

        var checkResults = AndroidRuntimePermissions.CheckPermissions(_requiredPermission);
        if (checkResults.Any((element) => element != AndroidRuntimePermissions.Permission.Granted))
        {
            var results = AndroidRuntimePermissions.RequestPermissions(_requiredPermission);
            if (results.Any((result) => result != AndroidRuntimePermissions.Permission.Granted))
            {
                ShowToast("게임 실행에 꼭 필요한 권한입니다.", E_TOAST.LENGTH_LONG);
                AndroidRuntimePermissions.OpenSettings();
            }
        }
    }

    /// <summary>
    /// 선택적 접근 권한을 사용자에게 요청합니다.
    /// </summary>
    private void RequestOptionalPermissions()
    {
        if (_optionalPermission == null || _optionalPermission.Length == 0)
            return;
        var checkResults = AndroidRuntimePermissions.CheckPermissions(_optionalPermission);
        if (checkResults.Any((element) => element != AndroidRuntimePermissions.Permission.Granted))
        {
            AndroidRuntimePermissions.RequestPermissions(_requiredPermission);
        }
    }

#if UNITY_ANDROID
    AndroidJavaObject _currentActivity;
    AndroidJavaClass _unityPlayer;
    AndroidJavaObject _context;
    AndroidJavaObject _toastInstance;
#endif

    public void InitializeAndroidObjects()
    {
#if UNITY_ANDROID
        _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        _currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        _context = _currentActivity.Call<AndroidJavaObject>("getApplicationContext");
#endif
    }

    /// <summary>
    /// 안드로이드 Native Toast 메시지를 출력합니다.
    /// </summary>
    /// <param name="message">출력할 Toast 메시지 내용</param>
    /// <param name="length">Toast 메시지 출력 시간</param>
    public void ShowToast(string message, E_TOAST length = E_TOAST.LENGTH_SHORT)
    {
#if UNITY_ANDROID
        _currentActivity.Call
        (
            "runOnUiThread",
            new AndroidJavaRunnable(() =>
            {
                AndroidJavaClass toast = new AndroidJavaClass("android.widget.Toast");

                AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", message);

                _toastInstance = toast.CallStatic<AndroidJavaObject>
                (
                    "makeText", _context, javaString, toast.GetStatic<int>(length.ToString())
                );

                _toastInstance.Call("show");
            })
         );
#endif
    }

    /// <summary>
    /// 출력중인 Toast 메시지를 지웁니다.
    /// </summary>
    public void CancelToast()
    {
#if UNITY_ANDROID
        _currentActivity.Call("runOnUiThread",
            new AndroidJavaRunnable(() =>
            {
                if (_toastInstance != null) _toastInstance.Call("cancel");
            }));
#endif
    }


#endregion
}
