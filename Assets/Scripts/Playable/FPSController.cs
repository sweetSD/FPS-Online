using UnityEngine;

[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(MoveController))]
public class FPSController : Photon.PunBehaviour
{
    [SerializeField] private CameraController m_CameraController;
    public CameraController CameraController => m_CameraController;

    [SerializeField] private MoveController m_MoveController;
    public MoveController MoveController => m_MoveController;

    [SerializeField] private Gun m_Gun;
    public Gun Gun => m_Gun;

    private float m_OriginFOV;

    private void Awake()
    {
        if (m_CameraController == null) m_CameraController = GetComponent<CameraController>();
        if (m_MoveController == null) m_MoveController = GetComponent<MoveController>();
        m_OriginFOV = CameraController.Camera.fieldOfView;
    }

    private void Update()
    {
        if (photonView.isMine) 
        {
            CameraController.Camera.fieldOfView = m_OriginFOV + m_Gun.FOVProgress * (m_Gun.ZoomFOV - m_Gun.DefaultFOV);
        }
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.isMine)
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (m_Gun == null)
            {
                m_Gun = PhotonNetwork.Instantiate("AK-47", Vector3.zero, Quaternion.identity, 0, new object[] { photonView.viewID }).transform.GetChild(1).GetComponent<Gun>();
            }
        }
    }
}
