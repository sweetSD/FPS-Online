using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : Photon.PunBehaviour, IPunObservable
{
    public enum HitScanType
    {
        Raycasting, Projectile
    }

    [Header("Animator")]
    // 무기에 적용된 애니메이터
    protected Animator m_Animator;

    [Header("Gun Camera")]
    // 총을 출력하는 카메라
    [SerializeField] protected Camera m_GunCamera;

    [Header("Gun Camera Options")]
    // 조준시 카메라의 FOV변경 속도
    [Tooltip("조준시 카메라의 FOV변경 속도")]
    [SerializeField] protected float m_ZoomSpeed = 15.0f;
    // 기본 카메라 FOV
    [Tooltip("기본 카메라 FOV (default: 40).")]
    [SerializeField] protected float m_DefaultFov = 40.0f;
    public float DefaultFOV => m_DefaultFov;
    // 조준시 카메라 FOV
    [SerializeField] protected float m_ZoomFov = 25.0f;
    public float ZoomFOV => m_ZoomFov;

    public float FOVProgress => (m_GunCamera.fieldOfView - m_DefaultFov) / (m_ZoomFov - m_DefaultFov);

    [Header("UI Weapon Name")]
    [Tooltip("게임 UI에 출력될 무기 이름")]
    [SerializeField] protected string m_WeaponName;
    protected string storedWeaponName;

    [Header("Weapon Sway")]
    //Enables weapon sway
    [Tooltip("Toggle weapon sway.")]
    [SerializeField] protected bool m_WeaponSway;

    [SerializeField] protected float m_SwayAmount = 0.02f;
    [SerializeField] protected float m_MaxSwayAmount = 0.06f;
    [SerializeField] protected float m_SwaySmoothValue = 4.0f;

    protected Vector3 m_InitialSwayPosition;

    // 연사속도에 사용될 변수
    protected float m_LastFired;
    [Header("Weapon Settings")]
    [Tooltip("피격 판정 방식 설정")]
    [SerializeField] protected HitScanType m_HitScanType;
    [SerializeField] protected LayerMask m_RaycastLayerMask;
    // 연사속도
    [Tooltip("연사속도")]
    [SerializeField] protected float m_FireRate;
    // 탄창을 모두 사용시 자동으로 리로드할지 여부
    [Tooltip("탄창을 모두 사용시 자동으로 리로드 할지 여부")]
    [SerializeField] protected bool m_AutoReload;
    // 마지막 총알 발사 후 자동 리로드까지 딜레이
    [SerializeField] protected float m_AutoReloadDelay;
    // 리로드중인지 여부
    protected bool m_isReloading;

    // 무기를 집어넣은 상태인지 여부
    protected bool m_HasBeenHolstered = false;
    // 무기가 집어넣어졌는지 여부
    protected bool m_Holstered;
    // 달리는지 여부
    protected bool m_isRunning;
    // 조준중인지 여부
    protected bool m_isAiming;
    // 걷는중인지 여부
    protected bool m_isWalking;
    // 무기 관찰중이닞 여부
    protected bool m_isInspecting;

    // 남은 탄창수
    protected int m_CurrentAmmo;
    // 최대 탄창수
    [Tooltip("무기가 가질 수 있는 총알 수")]
    [SerializeField] protected int m_MaxAmmo;
    // 탄창을 모두 소모하였는지 여부
    protected bool m_isOutOfAmmo;

    [Header("Bullet Settings")]
    //Bullet
    [Tooltip("총알의 기본 공격력")]
    [SerializeField] private float m_Damage = 10f;
    public float Damage => m_Damage;
    [Tooltip("장전시 들어가는 총알 수")]
    [SerializeField] protected int m_SpairBullet = 1;
    [Tooltip("총알에 가해질 물리 힘")]
    [SerializeField] protected float m_BulletForce = 400.0f;
    [Tooltip("탄창교체 후 총알 모델이 보여지는 딜레이 " +
        "(탄창 모두 사용 후 애니메이션입니다.)")]
    [SerializeField] protected float m_ShowBulletInMagDelay = 0.6f;
    [Tooltip("탄창 안의 총알 모델입니다. 모든 무기에 사용되지는 않습니다.")]
    [SerializeField] protected SkinnedMeshRenderer m_BulletInMagRenderer;

    [Header("Muzzleflash Settings")]
    [SerializeField] protected bool m_RandomMuzzleflash = false;
    // 최소 랜덤 값 (1이 되는게 좋습니다)
    protected int m_MinRandomValue = 1;

    [Range(2, 25)]
    [SerializeField] protected int m_MaxRandomValue = 5;

    [SerializeField] protected bool m_EnableMuzzleflash = true;
    [SerializeField] protected ParticleSystem m_MuzzleParticles;
    [SerializeField] protected bool m_EnableSparks = true;
    [SerializeField] protected ParticleSystem m_SparkParticles;
    [SerializeField] protected int m_MinSparkEmission = 1;
    [SerializeField] protected int m_MaxSparkEmission = 7;

    [Header("Muzzleflash Light Settings")]
    [SerializeField] protected Light m_MuzzleflashLight;
    [SerializeField] protected float m_LightDuration = 0.02f;

    [Header("Audio Source")]
    // 메인 오디오 소스
    [SerializeField] protected AudioSource m_MainAudioSource;
    // 효과음 오디오 소스
    [SerializeField] protected AudioSource m_ShootAudioSource;

    [Header("UI Components")]
    [SerializeField] protected Text m_CurrentWeaponText;
    [SerializeField] protected Text m_CurrentAmmoText;
    [SerializeField] protected Text m_TotalAmmoText;

    [System.Serializable]
    public class Prefabs
    {
        [Header("Prefabs")]
        public Transform bulletPrefab;
        public Transform casingPrefab;
        public Transform grenadePrefab;
    }
    [SerializeField] protected Prefabs m_Prefabs;

    [System.Serializable]
    public class Spawnpoints
    {
        [Header("Spawnpoints")]
        //Array holding casing spawn points 
        //(some weapons use more than one casing spawn)
        //Casing spawn point array
        public Transform casingSpawnPoint;
        //Bullet prefab spawn from this point
        public Transform bulletSpawnPoint;

        public Transform grenadeSpawnPoint;
    }
    [SerializeField] protected Spawnpoints m_Spawnpoints;

    [System.Serializable]
    public class SoundClips
    {
        public AudioClip shootSound;
        public AudioClip takeOutSound;
        public AudioClip holsterSound;
        public AudioClip reloadSoundOutOfAmmo;
        public AudioClip reloadSoundAmmoLeft;
        public AudioClip aimSound;
    }
    [SerializeField] protected SoundClips m_SoundClips;

    protected bool m_SoundHasPlayed = false;

    protected Coroutine m_Co_AmmoAutoReload;
    protected Coroutine m_Co_MuzzleFlash;

    protected virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        storedWeaponName = m_WeaponName;
        // 최대 탄창 수로 적용
        m_CurrentAmmo = m_MaxAmmo;
        //Weapon sway
        m_InitialSwayPosition = transform.localPosition;
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        AttachTo((int)transform.parent.gameObject.GetPhotonView().instantiationData[0]);

        // 최대 탄창 수로 적용
        m_CurrentAmmo = m_MaxAmmo;
        // 무기 이름 ui 적용
        if (m_CurrentWeaponText != null) m_CurrentWeaponText.text = m_WeaponName;
        // 최대 탄창 수 ui 적용
        if (m_TotalAmmoText != null) m_TotalAmmoText.text = m_MaxAmmo.ToString();

        m_MuzzleflashLight.enabled = false;

        // 발사 효과음 적용
        m_ShootAudioSource.clip = m_SoundClips.shootSound;

        if (!photonView.isMine)
        {
            m_GunCamera.GetComponent<Camera>().enabled = false;
            m_GunCamera.GetComponent<AudioReverbZone>().enabled = false;
        }
        else
        {
            m_GunCamera.GetComponent<Camera>().enabled = true;
            m_GunCamera.GetComponent<AudioReverbZone>().enabled = true;
        }
    }

    public void AttachTo(int viewID)
    {
        var target = PhotonView.Find(viewID).transform;
        transform.parent.SetParent(target.GetChild(0).GetChild(0));
        transform.parent.localPosition = Vector3.zero;
        transform.parent.localRotation = Quaternion.identity;
    }

    protected virtual void LateUpdate()
    {
        if (photonView.isMine)
            WeaponSwayProcess();
    }

    protected void WeaponSwayProcess()
    {
        //Weapon sway
        if (m_WeaponSway == true)
        {
            float movementX = -Input.GetAxis("Mouse X") * m_SwayAmount;
            float movementY = -Input.GetAxis("Mouse Y") * m_SwayAmount;
            //Clamp movement to min and max values
            movementX = Mathf.Clamp
                (movementX, -m_MaxSwayAmount, m_MaxSwayAmount);
            movementY = Mathf.Clamp
                (movementY, -m_MaxSwayAmount, m_MaxSwayAmount);
            //Lerp local pos
            Vector3 finalSwayPosition = new Vector3
                (movementX, movementY, 0);
            transform.localPosition = Vector3.Lerp
                (transform.localPosition, finalSwayPosition +
                    m_InitialSwayPosition, Time.deltaTime * m_SwaySmoothValue);
        }
    }

    protected virtual void Update()
    {
        if (!photonView.isMine) return;

        if (Input.GetButton("Fire2") && !m_isReloading && !m_isRunning && !m_isInspecting) AimProcess();
        else AimReleaseProcess();

        // 현재 탄창 텍스트 ui 적용
        if (m_CurrentAmmoText != null) m_CurrentAmmoText.text = m_CurrentAmmo.ToString();

        // 애니메이션 상태 체크
        AnimationCheck();

        if (Input.GetKeyDown(KeyCode.Q) && !m_isInspecting) MeleeAttack1();
        if (Input.GetKeyDown(KeyCode.F) && !m_isInspecting) MeleeAttack2();

        AmmoCheckProcess();

        if (Input.GetMouseButton(0) && !m_isOutOfAmmo && !m_isReloading && !m_isInspecting && !m_isRunning) Fire();

        // T 입력시 총기 관찰 애니메이션 실행
        if (Input.GetKeyDown(KeyCode.T)) Inspect();

        // E 입력시 무기 집어넣기
        if (Input.GetKeyDown(KeyCode.E) && !m_HasBeenHolstered)
        {
            m_Holstered = true;

            m_MainAudioSource.clip = m_SoundClips.holsterSound;
            m_MainAudioSource.Play();

            m_HasBeenHolstered = true;
        }
        else if (Input.GetKeyDown(KeyCode.E) && m_HasBeenHolstered)
        {
            m_Holstered = false;

            m_MainAudioSource.clip = m_SoundClips.takeOutSound;
            m_MainAudioSource.Play();

            m_HasBeenHolstered = false;
        }
        // 총기 집어넣는 애니메이션 재생
        m_Animator.SetBool("Holster", m_Holstered);

        // 재장전 
        if (Input.GetKeyDown(KeyCode.R) && m_CurrentAmmo < m_MaxAmmo && !m_isReloading && !m_isInspecting)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKey(KeyCode.W) && !m_isRunning ||
            Input.GetKey(KeyCode.A) && !m_isRunning ||
            Input.GetKey(KeyCode.S) && !m_isRunning ||
            Input.GetKey(KeyCode.D) && !m_isRunning)
        {
            m_Animator.SetBool("Walk", true);
        }
        else
        {
            m_Animator.SetBool("Walk", false);
        }

        if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift)))
        {
            m_isRunning = true;
        }
        else
        {
            m_isRunning = false;
        }

        m_Animator.SetBool("Run", m_isRunning);
    }

    protected void AimProcess()
    {
        if (!m_Animator.GetBool("Aim") && (IsAnimationPlaying("Draw") || IsAnimationPlaying("Holster"))) return;

        m_isAiming = true;
        m_Animator.SetBool("Aim", true);

        m_GunCamera.fieldOfView = Mathf.Lerp(m_GunCamera.fieldOfView,
            m_ZoomFov, m_ZoomSpeed * Time.deltaTime);

        if (!m_SoundHasPlayed)
        {
            m_MainAudioSource.clip = m_SoundClips.aimSound;
            m_MainAudioSource.Play();

            m_SoundHasPlayed = true;
        }
    }

    protected void AimReleaseProcess()
    {
        m_GunCamera.fieldOfView = Mathf.Lerp(m_GunCamera.fieldOfView,
            m_DefaultFov, m_ZoomSpeed * Time.deltaTime);

        m_isAiming = false;
        m_Animator.SetBool("Aim", false);

        m_SoundHasPlayed = false;
    }

    protected void MeleeAttack1()
    {
        m_Animator.Play("Knife Attack 1", 0, 0f);
    }

    protected void MeleeAttack2()
    {
        m_Animator.Play("Knife Attack 2", 0, 0f);
    }

    protected void AmmoCheckProcess()
    {
        // 탄창 모두 소비시
        if (m_CurrentAmmo == 0)
        {
            //Show out of ammo text
            if (m_CurrentWeaponText != null) m_CurrentWeaponText.text = "OUT OF AMMO";
            m_isOutOfAmmo = true;
            // 자동 리로드 활성화시
            if (m_AutoReload == true && !m_isReloading && m_Co_AmmoAutoReload == null)
            {
                m_Co_AmmoAutoReload = StartCoroutine(AutoReload());
            }
        }
        else
        {
            // 총알이 있을 경우 ui 업데이트
            if (m_CurrentWeaponText != null) m_CurrentWeaponText.text = storedWeaponName.ToString();
            m_isOutOfAmmo = false;
            //anim.SetBool ("Out Of Ammo", false);
        }
    }

    protected void Fire()
    {
        // 총 발포
        if (Time.time - m_LastFired > 1 / m_FireRate)
        {
            m_LastFired = Time.time;

            m_CurrentAmmo -= 1;

            m_ShootAudioSource.clip = m_SoundClips.shootSound;
            m_ShootAudioSource.Play();

            photonView.RPC("OnFire", PhotonTargets.Others);

            if (!m_isAiming) // 조준중이지 않으면
            {
                m_Animator.Play("Fire", 0, 0f);
            }
            else // 조준시
            {
                m_Animator.Play("Aim Fire", 0, 0f);
            }

            if (!m_RandomMuzzleflash)
            {
                m_MuzzleParticles.Emit(1);
            }
            else
            {
                if (m_EnableSparks)
                {
                    // 랜덤 스파크 파티클 생성
                    m_SparkParticles.Emit(Random.Range(m_MinSparkEmission, m_MaxSparkEmission));
                }
            }

            if (m_EnableMuzzleflash == true)
            {
                m_MuzzleParticles.Emit(1);
                if (m_Co_MuzzleFlash != null)
                    StopCoroutine(m_Co_MuzzleFlash);
                m_Co_MuzzleFlash = StartCoroutine(MuzzleFlashLight());
            }

            var ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, float.MaxValue, m_RaycastLayerMask))
            {
                if (hit.collider.transform.tag == "Player")
                {
                    hit.collider.transform.gameObject.GetComponent
                        <Character>().Damage(m_Damage);
                }
            }

            Debug.DrawRay(ray.GetPoint(0), ray.direction);

            SpawnBullet(m_Spawnpoints.bulletSpawnPoint.transform.position, hit.collider != null ? hit.point : ray.direction * float.MaxValue, m_BulletForce);

            // 탄피 생성
            SDObjectPool.GetPool("Big_Casing").ActiveObject(m_Spawnpoints.casingSpawnPoint.transform.position, m_Spawnpoints.casingSpawnPoint.transform.rotation.eulerAngles);
        }
    }

    protected void SpawnBullet(Vector3 position, Vector3 destPosition, float velocity)
    {
        Bullet bullet;
        Vector3 dir = (destPosition - position).normalized;

        if (m_HitScanType == HitScanType.Raycasting)
        {
            bullet = SDObjectPool.GetPool("Bullet").ActiveObject<Bullet>(
                position,
                Quaternion.LookRotation(dir).eulerAngles
            );
        }
        else
        {
            bullet = PhotonNetwork.Instantiate(
                "Bullet",
                position,
                Quaternion.LookRotation(dir),
                0
            ).GetComponent<Bullet>();
        }

        bullet.Initialize(m_HitScanType, m_Damage);

        // 총알에 velocity 추가
        bullet.GetComponent<Rigidbody>().velocity =
            dir * velocity;
    }

    protected void Inspect()
    {
        m_Animator.SetTrigger("Inspect");
    }

    protected IEnumerator AutoReload()
    {
        yield return new WaitForSeconds(m_AutoReloadDelay);
        StartCoroutine(Reload());
        m_Co_AmmoAutoReload = null;
    }

    protected IEnumerator Reload()
    {
        m_isReloading = true;
        if (m_isOutOfAmmo == true)
        {
            // 탄창 모두 소진시 다른 애니메이션 재생
            m_Animator.Play("Reload Out Of Ammo", 0, 0f);

            m_MainAudioSource.clip = m_SoundClips.reloadSoundOutOfAmmo;
            m_MainAudioSource.Play();

            //If out of ammo, hide the bullet renderer in the mag
            //Do not show if bullet renderer is not assigned in inspector
            if (m_BulletInMagRenderer != null)
            {
                m_BulletInMagRenderer.GetComponent
                <SkinnedMeshRenderer>().enabled = false;
                //Start show bullet delay
                StartCoroutine(ShowBulletInMag());
            }
        }
        else
        {
            // 탄창이 남아 있으면 일반 재장전 애니메이션 재생
            m_Animator.Play("Reload Ammo Left", 0, 0f);

            m_MainAudioSource.clip = m_SoundClips.reloadSoundAmmoLeft;
            m_MainAudioSource.Play();

            //If reloading when ammo left, show bullet in mag
            //Do not show if bullet renderer is not assigned in inspector
            if (m_BulletInMagRenderer != null)
            {
                m_BulletInMagRenderer.GetComponent
                <SkinnedMeshRenderer>().enabled = true;
            }
        }
        yield return new WaitForSeconds(1);
        //Restore ammo when reloading
        m_CurrentAmmo = m_MaxAmmo;
        if (m_isOutOfAmmo) m_CurrentAmmo -= m_SpairBullet;
        m_isOutOfAmmo = false;
    }

    //Enable bullet in mag renderer after set amount of time
    protected IEnumerator ShowBulletInMag()
    {
        //Wait set amount of time before showing bullet in mag
        yield return new WaitForSeconds(m_ShowBulletInMagDelay);
        m_BulletInMagRenderer.GetComponent<SkinnedMeshRenderer>().enabled = true;
    }

    // 총기 발포 섬광 효과
    protected IEnumerator MuzzleFlashLight()
    {
        m_MuzzleflashLight.enabled = true;
        yield return new WaitForSeconds(m_LightDuration);
        m_MuzzleflashLight.enabled = false;
    }

    // 재생중인 애니메이션 체크
    protected void AnimationCheck()
    {
        // 재장전 애니메이션 재생 확인
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Reload Out Of Ammo") ||
            m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Reload Ammo Left"))
        {
            m_isReloading = true;
        }
        else
        {
            m_isReloading = false;
        }

        // 무기 관찰 애니메이션 재생 확인
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Inspect"))
        {
            m_isInspecting = true;
        }
        else
        {
            m_isInspecting = false;
        }
    }

    protected bool IsAnimationPlaying(string name) { return m_Animator.GetCurrentAnimatorStateInfo(0).IsName(name); }

    [PunRPC]
    public void OnFire()
    {
        m_ShootAudioSource.clip = m_SoundClips.shootSound;
        m_ShootAudioSource.Play();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
