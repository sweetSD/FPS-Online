using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhotonEvent : Photon.PunBehaviour
{
    [SerializeField] private List<PhotonView> m_PhotonViews;

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);

        foreach(var view in m_PhotonViews)
        {
            view.BroadcastMessage("OnPhotonInstantiate", info);
        }
    }
}
