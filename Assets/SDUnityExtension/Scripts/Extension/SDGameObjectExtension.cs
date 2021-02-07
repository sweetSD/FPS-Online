using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SDGameObjectExtension
{
    public static void Destroy(this Photon.PunBehaviour obj, bool deactive = true)
    {
        if (PhotonNetwork.inRoom)
        {
            if (obj.photonView.isMine)
                PhotonNetwork.Destroy(obj.photonView);
        }
        else
        {
            if (deactive) obj.gameObject.SetActive(false);
            else GameObject.Destroy(obj.gameObject);
        }
    }

    public static void DestroyChildren(this Transform trans)
    {
        for(int i = trans.childCount; i >= 0; i--)
        {
            GameObject.Destroy(trans.GetChild(i).gameObject);
        }
    }
}
