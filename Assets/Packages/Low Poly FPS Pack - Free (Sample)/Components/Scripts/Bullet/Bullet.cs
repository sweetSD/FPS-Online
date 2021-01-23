using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class KeyTransformPair
{
    public string m_Key;
    public Transform m_Transform;
}

public class Bullet : Photon.PunBehaviour {

    [Tooltip("총알의 기본 공격력")]
    [SerializeField] private float m_Damage = 10f;
    public float Damage
    {
        get => m_Damage;
        private set => m_Damage = value;
    }

	[Range(5, 100)]
	[Tooltip("생성된 후 파괴될 시간")]
	[SerializeField] private float m_DestroyAfter;
	[Tooltip("충돌시 총알 오브젝트 파괴 여부")]
    [SerializeField] private bool m_DestroyOnImpact = false;
	[Tooltip("오브젝트 파괴시 오브젝트 비활성화 여부")]
    [SerializeField] private bool m_DeactiveOnDestroy = false;
	[Tooltip("충돌 파괴시 최소 시간")]
    [SerializeField] private float m_MinDestroyTime;
	[Tooltip("충돌 파괴시 최대 시간")]
    [SerializeField] private float m_MaxDestroyTime;
    [Tooltip("충돌 레이어")]
    [SerializeField] private LayerMask m_OverlapLayer;

    [Header("피격 효과 프리팹")]
    [SerializeField] private KeyTransformPair[] m_ImpactPrefabs;

    private Collider[] m_OverlapColliders = new Collider[8];

    private Gun.HitScanType m_HitScanType;

	private void Start () 
	{
		StartCoroutine (DestroyAfter ());
	}

    public void Initialize(Gun.HitScanType type, float dmg)
    {
        if (type == Gun.HitScanType.Raycasting) Initialize_RPC(type, dmg);
        else photonView.RPC("Initialize_RPC", PhotonTargets.All, new object[] { type, dmg });
    }

    [PunRPC]
    private void Initialize_RPC(Gun.HitScanType type, float dmg)
    {
        m_HitScanType = type;
        m_Damage = dmg;
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.isMasterClient)
        {
            int count = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale * 0.5f, m_OverlapColliders, Quaternion.LookRotation(transform.forward), m_OverlapLayer);
            for (int i = 0; i < count; i++) CollisionCheck(m_OverlapColliders[i]);
        }
    }

    public void SetDamage(float dmg)
    {
        if (photonView.isMine)
            photonView.RPC("SetDamage_RPC", PhotonTargets.All, new object[] { dmg });
    }

    [PunRPC]
    private void SetDamage_RPC(float dmg)
    {
        m_Damage = dmg;
    }

    private void CollisionCheck (Collider collider)
    {
        m_ImpactPrefabs.ForEach((value) =>
        {
            if (collider.transform.tag == value.m_Key)
            {
                //Instantiate(value.m_Transform, transform.position,
                //    Quaternion.LookRotation(collider.contacts[0].normal));
                this.Destroy(m_DeactiveOnDestroy);
            }
        });
        Debug.Log(collider.transform.tag);

        if (collider.transform.tag == "Player")
        {
            collider.transform.gameObject.GetComponent
                <Character>().Damage(m_Damage);
        }

        if (collider.transform.tag == "Target") 
		{
			collider.transform.gameObject.GetComponent
				<TargetScript>().isHit = true;
        }
			
		if (collider.transform.tag == "ExplosiveBarrel") 
		{
			collider.transform.gameObject.GetComponent
				<ExplosiveBarrelScript>().explode = true;
		}

        // 피격시 총알 오브젝트 제거
        if (!m_DestroyOnImpact)
        {
            StartCoroutine(DestroyTimer());
        }
        else
        {
            this.Destroy(m_DeactiveOnDestroy);
        }
    }

	private IEnumerator DestroyTimer () 
	{
		yield return new WaitForSeconds
			(Random.Range(m_MinDestroyTime, m_MaxDestroyTime));
        this.Destroy(m_DeactiveOnDestroy);
    }

	private IEnumerator DestroyAfter () 
	{
		yield return new WaitForSeconds (m_DestroyAfter);
        this.Destroy(m_DeactiveOnDestroy);
    }

    private void Destroy(bool deactiveOnDestroy)
    {
        if (m_HitScanType == Gun.HitScanType.Raycasting)
        {
            if (deactiveOnDestroy) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
        else
        {
            SDGameObjectExtension.Destroy(this, deactiveOnDestroy);
        }
    }
}
