using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : Photon.PunBehaviour
{
    [Tooltip("캐릭터의 기본 체력")]
    [SerializeField] private float m_Def_HP = 100f;

    [Tooltip("캐릭터의 최대 체력")]
    [SerializeField] private float m_Max_HP = 100f;

    [Tooltip("캐릭터의 체력")]
    [SerializeField] private float m_HP = 100f;
    public float HP => m_HP;

    [SerializeField] private FPSController m_FPSController;
    public FPSController Controller
    {
        get
        {
            if (m_FPSController == null) m_FPSController = GetComponent<FPSController>();
            return m_FPSController;
        }
    }

    public UnityEvent m_OnDead = new UnityEvent();

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        m_HP = m_Def_HP;
        ApplyDefaultHP();
    }

    public void Damage(float dmg)
    {
        photonView.RPC("Damage_RPC", PhotonTargets.All, new object[] { dmg });    
    }

    [PunRPC]
    private void Damage_RPC(float dmg)
    {
        m_HP -= dmg;
        Debug.Log("My HP is " + m_HP.ToString());

        ApplyDefaultHP();

        if (m_HP <= 0)
        {
            m_HP = 0;
            m_OnDead?.Invoke();
        }
    }

    private void ApplyDefaultHP()
    {
        if (m_HP > m_Max_HP) m_HP = m_Max_HP;
    }
}
