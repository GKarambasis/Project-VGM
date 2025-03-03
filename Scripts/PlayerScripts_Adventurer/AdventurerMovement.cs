using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerMovement : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    [SerializeField] float m_walkSpeed = 3f;
    [SerializeField] float m_sprintSpeed = 10f;
    private float m_speed;

    [SerializeField] float mouseSensitivity = 100f;
    public Transform playerBody;

    private Rigidbody rb;
    private float xRotation = 0f;

    //Movement Smoothing
    [Header("Smooth Movement Settings")]
    [SerializeField] float m_smoothInputSpeed = 10f;
    private float m_previousHorizontal;
    private float m_previousVertical;

    [Header("Smooth Animation Settings")]
    //Animation
    [SerializeField] float a_sprintSpeedModifier = 1f;
    [SerializeField] float a_walkSpeedModifier = 0.3f;


    [Header("Stamina Settings")]
    public float sprintStaminaCost;
    private AdventurerAttack adventurerAttack;


    private Animator animator;
    private float animHorizontal, animVertical;
    private float a_previousVertical, a_previousHorizontal;

    void Start()
    {
        
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
        }
        
        adventurerAttack = GetComponent<AdventurerAttack>();
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate; 
        animator = GetComponentInChildren(typeof(Animator)) as Animator;
    }

    void FixedUpdate()
    {
        LookAround();
        Input_Move();
        Input_Sprint();
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerBody.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Input_Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        //SmoothInput by using Lerp
        float smoothHorizontal = Mathf.Lerp(m_previousHorizontal, moveX, Time.fixedDeltaTime * m_smoothInputSpeed);
        float smoothVertical = Mathf.Lerp(m_previousVertical, moveY, Time.fixedDeltaTime * m_smoothInputSpeed);

        //Get the Input Axis to Modify Animations
        animator.SetFloat("HorizontalSpeed", smoothHorizontal * animHorizontal);
        animator.SetFloat("VerticalSpeed", smoothVertical * animVertical);

        Vector3 move = transform.right * smoothHorizontal + transform.forward * smoothVertical;
        rb.MovePosition(rb.position + move * m_speed * Time.fixedDeltaTime);

        // Store previous values for smoothing
        m_previousHorizontal = smoothHorizontal;
        m_previousVertical = smoothVertical;
    }

    //Should change to Lerp to make it Smoother
    void Input_Sprint()
    {
        if (Input.GetButton("Fire3") && adventurerAttack.stamina - sprintStaminaCost >= 0)
        {
            adventurerAttack.stamina -= sprintStaminaCost;
            //Animation Lerp Transition to Sprinting
            float s_animHorizontal = Mathf.Lerp(a_previousHorizontal, a_sprintSpeedModifier, Time.fixedDeltaTime * m_smoothInputSpeed);
            float s_animVertical = Mathf.Lerp(a_previousVertical, a_sprintSpeedModifier, Time.fixedDeltaTime * m_smoothInputSpeed);

            animHorizontal = s_animHorizontal;
            animVertical = s_animVertical;
            

            a_previousHorizontal = animHorizontal;
            a_previousVertical = animVertical;

            //Movement Lerp Transition to Sprinting
            m_speed = m_sprintSpeed;
        }
        else
        {
            //Animation Lerp Transition to Walking
            float s_animHorizontal = Mathf.Lerp(a_previousHorizontal, a_walkSpeedModifier, Time.fixedDeltaTime * m_smoothInputSpeed);
            float s_animVertical = Mathf.Lerp(a_previousVertical, a_walkSpeedModifier, Time.fixedDeltaTime * m_smoothInputSpeed);

            animHorizontal = s_animHorizontal;
            animVertical = s_animVertical;

            a_previousHorizontal = animHorizontal;
            a_previousVertical = animVertical;

            //Movement Lerp Transition to Walking
            m_speed = m_walkSpeed;
        }
    }
}
