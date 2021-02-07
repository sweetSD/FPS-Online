using UnityEngine;
using System.Collections;

public class AutomaticGun : Gun
{
    [Header("Grenade Settings")]
    [SerializeField] private float m_GrenadeSpawnDelay = 0.35f;

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.G) && !m_isInspecting) ThrowGrenade();
    }

    private void ThrowGrenade()
    {
        StartCoroutine(GrenadeSpawnDelay());
        //Play grenade throw animation
        m_Animator.Play("GrenadeThrow", 0, 0.0f);
    }

    private IEnumerator GrenadeSpawnDelay()
    {
        yield return new WaitForSeconds(m_GrenadeSpawnDelay);
        Instantiate(m_Prefabs.grenadePrefab,
            m_Spawnpoints.grenadeSpawnPoint.transform.position,
            m_Spawnpoints.grenadeSpawnPoint.transform.rotation);
    }
}