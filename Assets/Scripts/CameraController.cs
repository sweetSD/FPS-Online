using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Photon.PunBehaviour
{
    [Tooltip("카메라를 지정해주세요. null일 경우 Camera.main으로 대체됩니다.")]
    [SerializeField] private Camera m_Camera;
    public Camera Camera => m_Camera;
    [Tooltip("1인칭 시점 마우스 가속도입니다.")]
    [SerializeField] private float m_MouseSensivity = 5f;
    public float MouseSensivity
    {
        get => m_MouseSensivity;
        set => m_MouseSensivity = value;
    }
    [Tooltip("1인칭 시점 마우스 상하 최대 회전 각도입니다. (양수를 입력해주세요.)")]
    [SerializeField] private float m_MaxUpDownRangeDegree = 90;
    public float MaxUpDownRange
    {
        get => m_MaxUpDownRangeDegree;
        set => m_MaxUpDownRangeDegree = value;
    }
    [SerializeField] private Transform m_PlayerTransform;
    public Transform PlayerTransform
    {
        get
        {
            if (m_PlayerTransform == null) m_PlayerTransform = transform;
            return m_PlayerTransform;
        }
    }

    [SerializeField] private bool m_RotateX = true;
    [SerializeField] private bool m_RotateY = true;
    [SerializeField] private bool m_RotateXCamera = false;

    private Vector3 m_Rotate_Input;
    private float m_RotateX_Angle = 0f;

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.isMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!photonView.isMine)
        {
            m_Camera.tag = "Untagged";
            m_Camera.GetComponent<Camera>().enabled = false;
            m_Camera.GetComponent<AudioListener>().enabled = false;
            m_Camera.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().enabled = false;
            m_Camera.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Hidden");
            transform.Find("Third Person Graphic").gameObject.SetActive(true);
        }
        else
        {
            m_Camera.tag = "MainCamera";
            m_Camera.GetComponent<Camera>().enabled = true;
            m_Camera.GetComponent<AudioListener>().enabled = true;
            m_Camera.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().enabled = true;
            m_Camera.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("First Person");
            transform.Find("Third Person Graphic").gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (photonView.isMine)
        {
            RotateInput();
            RotateProcess();
        }
    }

    #region INPUT_AREA
    private void RotateInput()
    {
        m_Rotate_Input = new Vector3(m_RotateX ? Input.GetAxisRaw("Mouse X") : 0, m_RotateY ? Input.GetAxisRaw("Mouse Y") : 0);
    }
    #endregion

    #region PROCESS_AREA
    private void RotateProcess()
    {
        m_Rotate_Input *= MouseSensivity;
        m_RotateX_Angle -= m_Rotate_Input.y;
        m_RotateX_Angle = Mathf.Clamp(m_RotateX_Angle, -MaxUpDownRange, MaxUpDownRange);

        m_Camera.transform.localRotation = Quaternion.Euler(m_RotateX_Angle,
            m_RotateXCamera ? (m_Camera.transform.localEulerAngles.y + m_Rotate_Input.x) : 0,
            0);

        if (!m_RotateXCamera) PlayerTransform.Rotate(Vector3.up, m_Rotate_Input.x, Space.Self);
    }
    #endregion
}
