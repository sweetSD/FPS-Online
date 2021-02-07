using UnityEngine;

public class MoveController : Photon.PunBehaviour, IPunObservable
{
    [Header("Movement Setting")]
    [Tooltip("기본적인 이동 속도입니다.")]
    [SerializeField] private float m_MoveSpeed = 10.0f;
    public float MoveSpeed
    {
        get => m_MoveSpeed;
        set => m_MoveSpeed = value;
    }
    [Tooltip("점프 파워입니다.")]
    [SerializeField] private float m_JumpHeight = 3f;
    public float JumpHeight
    {
        get => m_JumpHeight;
        set => m_JumpHeight = value;
    }
    [Tooltip("땅 체크 거리를 설정해주세요.")]
    [SerializeField] private float m_GroundCheckDistance = 0.1f;
    [Tooltip("계단 체크 거리를 설정해주세요. (Rigidbody 사용시)")]
    [SerializeField] private float m_StepOffset = 0.3f;
    public float StepOffset => isUseCharacterController ? m_CharacterController.stepOffset : m_StepOffset;
    [Tooltip("충돌체 반지름을 설정해주세요. (Rigidbody 사용시)")]
    [SerializeField] private float m_ColliderRadius = 0.5f;
    private float Radius => isUseCharacterController ? m_CharacterController.radius : m_ColliderRadius;
    [Tooltip("땅 레이어를 설정해주세요.")]
    [SerializeField] private LayerMask m_GroundMask;
    [Tooltip("땅 체크 피벗을 설정해주세요.")]
    [SerializeField] private Transform m_GrouncCheckPoint;
    private Transform GroundCheckPoint => m_GrouncCheckPoint ?? transform;
    [Tooltip("오를 수 있는 최대 경사로 각도를 설정해주세요.")]
    [SerializeField] private float m_SlopeLimit = 45f;

    [Header("CharacterController Setting")]
    [Tooltip("캐릭터 컨트롤러 사용을 원하시면 컴포넌트를 연결해주세요. (연결하지 않으면 Rigidbody가 사용됩니다.)")]
    [SerializeField] private CharacterController m_CharacterController;
    [Tooltip("캐릭터 컨트롤러를 사용한다면 중력 값을 입력해주세요. (기본값: 9.81)")]
    [SerializeField] private float m_Gravity = -9.81f;
    public float Gravity => m_Gravity;

    [Header("Network Setting")]
    [SerializeField] private float m_PositionSmooth = 5f;
    [SerializeField] private float m_RotationSmooth = 5f;
    [SerializeField] private float m_PositionSmoothLimit = 2f;
    private Vector3 m_RealPosition = Vector3.zero;
    private Quaternion m_RealRotation = Quaternion.identity;

    private Rigidbody m_Rigidbody;

    private Vector3 m_Move_Input = Vector3.zero;
    private Vector3 m_Velocity = Vector3.zero;
    private Vector3 m_SlopeVelocity = Vector3.zero;
    private Vector3 m_MoveVelocity = Vector3.zero;

    private float m_EleapsedSlopeSlide = 0f;
    private Vector3 m_DirectionSlope;
    private Vector3 m_HitFaceNormal;

    // 땅 위에 있는지 여부
    private bool m_isGround = false;
    public bool isGround => m_isGround;
    public bool canJump => m_isGround && m_EleapsedSlopeSlide == 0;
    public bool isUseCharacterController => m_CharacterController != null;

    private void OnEnable()
    {
        if (!isUseCharacterController && m_Rigidbody == null)
            m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (photonView.isMine)
        {
            CheckState();
            SlopeProcess();
            MoveInput();
            if (Input.GetButtonDown("Jump")) Jump();
        }
        else
            InterpolationProcess();
    }

    private void FixedUpdate()
    {
        if (photonView.isMine)
        {
            MoveProcess();
        }
    }

    #region INPUT_AREA
    private void MoveInput()
    {
        m_Move_Input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void Jump()
    {
        if (isUseCharacterController && canJump)
        {
            m_Velocity.y = Mathf.Sqrt(JumpHeight * -2f * m_Gravity);
        }
        else if (!isUseCharacterController && canJump)
            m_Rigidbody?.AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
    }
    #endregion

    #region PROCESS_AREA
    private void CheckState()
    {
        if(isUseCharacterController) m_isGround = Physics.CheckSphere(GroundCheckPoint.position + (Vector3.up * m_CharacterController.radius * 0.5f), m_CharacterController.radius, m_GroundMask);
        else m_isGround = Physics.CheckSphere(GroundCheckPoint.position, m_GroundCheckDistance, m_GroundMask);
    }

    private void SlopeProcess()
    {
        RaycastHit hit;

        if (Physics.BoxCast(GroundCheckPoint.position + Vector3.up * (m_CharacterController.radius * 1.5f), new Vector3(m_CharacterController.radius, 0.1f, m_CharacterController.radius) * 0.75f, Vector3.down, out hit, transform.rotation, m_CharacterController.radius * 2, m_GroundMask))
        {
            m_HitFaceNormal = hit.normal;

            var slopeAngle = Vector3.Angle(Vector3.up, m_DirectionSlope.normalized) - 90f;

            m_DirectionSlope = Vector3.Cross(m_HitFaceNormal, Vector3.Cross(m_HitFaceNormal, Vector3.up));
            m_DirectionSlope.Normalize();

            if (slopeAngle >= 90f)
                slopeAngle = 0;

            if (slopeAngle > m_SlopeLimit && m_HitFaceNormal != Vector3.up)
            {
                var multiply = SDMath.Map(slopeAngle, m_SlopeLimit, 90, 2.5f, 7.5f);
                //if (m_SlopeVelocity.normalized != m_DirectionSlope.normalized) m_EleapsedSlopeSlide = 0.1f;
                m_SlopeVelocity = m_DirectionSlope * m_EleapsedSlopeSlide * multiply;
                m_EleapsedSlopeSlide += Time.deltaTime;
            }
            else
            {
                m_SlopeVelocity = Vector3.Lerp(m_SlopeVelocity, Vector3.zero, Time.deltaTime * 4);
                if (m_SlopeVelocity.sqrMagnitude < 0.1f) m_SlopeVelocity = Vector3.zero;
                m_EleapsedSlopeSlide = 0f;
            }
        }
    }

    private void MoveProcess()
    {
        var moveDirection = transform.TransformDirection(m_Move_Input);
        m_MoveVelocity = moveDirection * MoveSpeed * Time.fixedDeltaTime;

        if (isUseCharacterController)
        {
            if (isGround && m_Velocity.y < 0) m_Velocity.y = -1f;
            m_CharacterController.Move(m_MoveVelocity);
            m_Velocity.y += m_Gravity * Time.fixedDeltaTime;
            if (m_SlopeVelocity.sqrMagnitude > 0) m_CharacterController.Move(m_SlopeVelocity * Time.fixedDeltaTime);
            m_CharacterController.Move(m_Velocity * Time.fixedDeltaTime);
        }
        else
        {
            m_Rigidbody.MovePosition(m_Rigidbody.transform.position + m_MoveVelocity);

            Vector3 horizontalMove = m_Rigidbody.velocity;
            horizontalMove.y = 0;
            float distance = horizontalMove.magnitude * Time.fixedDeltaTime * 5;
            horizontalMove.Normalize();
            RaycastHit hit;

            if (m_Rigidbody.SweepTest(horizontalMove, out hit, distance))
            {
                m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
            }
        }
    }

    private void InterpolationProcess()
    {
        InterpolateMovement();
    }

    private void InterpolateMovement()
    {
        if (Vector3.Distance(transform.position, m_RealPosition) > m_PositionSmoothLimit)
            transform.position = m_RealPosition;
        else
            transform.position = Vector3.Lerp(transform.position, m_RealPosition, m_PositionSmooth * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_RealRotation, m_RotationSmooth * Time.deltaTime);
    }
    #endregion

    #region NETWORK_AREA
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } 
        else
        {
            m_RealPosition = (Vector3)stream.ReceiveNext();
            m_RealRotation = (Quaternion)stream.ReceiveNext();

            transform.position = m_RealPosition;
            transform.rotation = m_RealRotation;
        }
    }
    #endregion
}
