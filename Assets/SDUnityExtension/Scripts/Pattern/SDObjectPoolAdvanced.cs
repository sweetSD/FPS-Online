using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by sweetSD.
/// 
/// 오브젝트 풀입니다.
/// 원하시는 오브젝트로 상속하여 사용할 수 있습니다.
/// </summary>
public class SDObjectPoolAdvanced : MonoBehaviour
{
    static Dictionary<string, SDObjectPoolAdvanced> m_Pools = new Dictionary<string, SDObjectPoolAdvanced>();

    [Header("Pool Info")]
    [Tooltip("오브젝트 풀 이름입니다.")]
    [SerializeField] protected string _poolName = "";

    [Header("Pool Objects")]
    [Tooltip("오브젝트 풀링에 사용될 오브젝트들입니다.")]
    [SerializeField] protected Stack<GameObject> _storedPool = null;
    public Stack<GameObject> StoredPool => _storedPool;

    [Tooltip("오브젝트 풀링에 사용중인 오브젝트들입니다.")]
    [SerializeField] protected List<GameObject> _usingPool = null;
    public List<GameObject> UsingPool => _usingPool;

    [Header("Pool Object")]
    [Tooltip("오브젝트 풀링에 사용될 오브젝트입니다.")]
    [SerializeField] protected GameObject _poolObject;

    [Header("Pool Object Count")]
    [Tooltip("기본 생성 오브젝트 수입니다.")]
    [SerializeField] protected int _initialSize = 32;

    [Header("Pool Root")]
    [Tooltip("오브젝트를 갖고 있을 부모 Transform 입니다.")]
    [SerializeField] protected Transform _poolRoot;

    [Header("Flexible")]
    [Tooltip("오브젝트가 모두 사용중일 경우 새로 생성할지 여부입니다.")]
    [SerializeField] protected bool _flexible = true;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 오브젝트 풀을 초기화합니다.
    /// </summary>
    public virtual void Initialize()
    {
        if (_poolRoot == null)
            _poolRoot = GetComponent<Transform>();

        _storedPool = new Stack<GameObject>();
        _usingPool = new List<GameObject>();

        for (int i = 0; i < _initialSize; i++)
        {
            var go = Instantiate(_poolObject, Vector3.zero, Quaternion.identity, _poolRoot);
            go.SetActive(false);
            _storedPool.Push(go);
        }

        if (!m_Pools.ContainsKey(_poolName))
        {
            m_Pools.Add(_poolName, this);
        }

    }

    /// <summary>
    /// 오브젝트 풀에서 오브젝트 하나를 가져옵니다.
    /// </summary>
    /// <param name="posVec3">위치</param>
    /// <param name="rotVec3">회전</param>
    /// <param name="sclVec3">스케일</param>
    /// <returns></returns>
    public virtual T ActiveObject<T>(Vector3 posVec3, Vector3 rotVec3, Vector3 sclVec3, bool doNotActive = false) where T : MonoBehaviour
    {
        return ActiveObject(posVec3, rotVec3, sclVec3, doNotActive).GetComponent<T>();
    }

    public virtual GameObject ActiveObject(Vector3 posVec3, Vector3 rotVec3, Vector3 sclVec3, bool doNotActive = false)
    {
        if(_storedPool.Count > 0)
        {
            var go = _storedPool.Pop();
            _usingPool.Add(go);
            var objTransform = go.transform;
            objTransform.localPosition = posVec3;
            objTransform.localEulerAngles = rotVec3;
            objTransform.localScale = sclVec3;
            objTransform.gameObject.SetActive(!doNotActive);
            return objTransform.gameObject;
        }
        else
        {
            if (_flexible)
            {
                var go = Instantiate(_poolObject, posVec3, Quaternion.Euler(rotVec3), _poolRoot);
                _usingPool.Add(go);
                return go;
            }
            else
                return null;
        }
    }

    public virtual void DeactiveObject(GameObject go)
    {
        go.SetActive(false);

        _usingPool.Remove(go);
        _storedPool.Push(go);
    }

    /// <summary>
    /// 오브젝트 풀을 비웁니다.
    /// </summary>
    public void DestroyPool()
    {
        while (_storedPool.Count > 0) Destroy(_storedPool.Pop());
        _storedPool.Clear();
    }

    #region STATIC_FUNCTIONS
    public static SDObjectPoolAdvanced GetPool(string name)
    {
        SDObjectPoolAdvanced p;
        m_Pools.TryGetValue(name, out p);
        return p;
    }
    #endregion
}
