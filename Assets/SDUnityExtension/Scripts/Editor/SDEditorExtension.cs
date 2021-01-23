using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Created by sweetSD. (Editor Extension)
/// 
/// 유니티 에디터 내부에서 사용할 수 있는 유틸리티 함수들이 있는 클래스입니다.
/// 
/// </summary>
public class SDEditorExtension : MonoBehaviour
{
    [MenuItem("SDUnityExtension/Create Manager Prefab")]
    public static void CreateManagerPrefab()
    {
        var mngObject = Resources.Load<GameObject>("Prefab/SDManager");
        var mngInst = PrefabUtility.InstantiatePrefab(mngObject);
        Selection.activeObject = mngInst;
        mngInst.name = "<-- SD Manager -->";
    }
}
