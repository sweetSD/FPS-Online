using UnityEngine;
using System.Collections;

public class Casing : MonoBehaviour
{

    [Header("Force X")]
    [Tooltip("Minimum force on X axis")]
    public float m_MinimumXForce;
    [Tooltip("Maimum force on X axis")]
    public float m_MaximumXForce;
    [Header("Force Y")]
    [Tooltip("Minimum force on Y axis")]
    public float m_MinimumYForce;
    [Tooltip("Maximum force on Y axis")]
    public float m_MaximumYForce;
    [Header("Force Z")]
    [Tooltip("Minimum force on Z axis")]
    public float m_MinimumZForce;
    [Tooltip("Maximum force on Z axis")]
    public float m_MaximumZForce;
    [Header("Rotation Force")]
    [Tooltip("Minimum initial rotation value")]
    public float m_MinimumRotation;
    [Tooltip("Maximum initial rotation value")]
    public float m_MaximumRotation;
    [Header("Despawn Time")]
    [Tooltip("How long after spawning that the casing is destroyed")]
    public float m_DespawnTime;

    [Header("Audio")]
    public AudioClip[] m_CasingSounds;
    public AudioSource m_AudioSource;

    [Header("Spin Settings")]
    //How fast the casing spins
    [Tooltip("How fast the casing spins over time")]
    public float speed = 2500.0f;

    private Rigidbody m_RigidBody;
    public Rigidbody RigidBody
    {
        get
        {
            if (m_RigidBody == null) m_RigidBody = GetComponent<Rigidbody>();
            return m_RigidBody;
        }
    }

    //Launch the casing at start
    private void OnEnable()
    {
        RigidBody.velocity = Vector3.zero;
        RigidBody.angularVelocity = Vector3.zero;

        //Random rotation of the casing
        RigidBody.AddRelativeTorque(
            Random.Range(m_MinimumRotation, m_MaximumRotation), //X Axis
            Random.Range(m_MinimumRotation, m_MaximumRotation), //Y Axis
            Random.Range(m_MinimumRotation, m_MaximumRotation)  //Z Axis
            * Time.deltaTime);

        //Random direction the casing will be ejected in
        RigidBody.AddRelativeForce(
            Random.Range(m_MinimumXForce, m_MaximumXForce),  //X Axis
            Random.Range(m_MinimumYForce, m_MaximumYForce),  //Y Axis
            Random.Range(m_MinimumZForce, m_MaximumZForce)); //Z Axis		

        //Start the remove/destroy coroutine
        StartCoroutine(RemoveCasing());
        //Set random rotation at start
        transform.rotation = Random.rotation;
        //Start play sound coroutine
        StartCoroutine(PlaySound());
    }

    private void FixedUpdate()
    {
        //Spin the casing based on speed value
        transform.Rotate(Vector3.right, speed * Time.deltaTime);
        transform.Rotate(Vector3.down, speed * Time.deltaTime);
    }

    private IEnumerator PlaySound()
    {
        //Wait for random time before playing sound clip
        yield return new WaitForSeconds(Random.Range(0.25f, 0.85f));
        //Get a random casing sound from the array 
        m_AudioSource.clip = m_CasingSounds
            [Random.Range(0, m_CasingSounds.Length)];
        //Play the random casing sound
        m_AudioSource.Play();
    }

    private IEnumerator RemoveCasing()
    {
        //Destroy the casing after set amount of seconds
        yield return new WaitForSeconds(m_DespawnTime);
        //Destroy casing object
        gameObject.SetActive(false);
    }
}