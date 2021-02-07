using System.Collections.Generic;
using UnityEngine;

public class PUNObjectPoolBridge : SDSingleton<PUNObjectPoolBridge>, IPunPrefabPool
{
    public void Awake()
    {
        PhotonNetwork.PrefabPool = this;
        SetInstance(this, false);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        Debug.LogWarning("Instantiate Prefab :" + prefabId);
        var p = SDObjectPool.GetPool(prefabId);
        if (p != null)
            return p.ActiveObject(position, rotation.eulerAngles);
        else
        {
            var go = Resources.Load<GameObject>(prefabId);
            return go != null ? Instantiate(go, position, rotation) : null;
        }
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
