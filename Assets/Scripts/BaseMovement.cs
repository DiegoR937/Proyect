using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    public CharacterController Controller;

    public float Speed;

    public float VelocitySmooth = 0.1f;
    public float SensitiveSmooth;

    public Transform Cam;

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;
    public Vector3 VelocidadGravedad;
    public float gravedad = -9.81f;
    public float HeightJump;

    public Animator animator;
    public bool isJump;
    public bool IsGround;
    public bool IsFalling;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jumping();
    }
    void Movement()
    {

        float Movx = Input.GetAxis("Horizontal");
        float Movz = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(Movx, 0, Movz).normalized;
        float Magnitud = Mathf.Clamp01(direction.magnitude);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Magnitud /= 0.5f;
        }

        animator.SetFloat("Input Magnitude", Magnitud, 0.05f, Time.deltaTime);



        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref SensitiveSmooth, VelocitySmooth);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            Controller.Move(moveDir.normalized * Speed * Time.deltaTime);

        }
    }
    void Jumping()
    {

        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, groundMask);

        if (isGrounded && VelocidadGravedad.y < 0)
        {
            VelocidadGravedad.y = -2f;
            animator.SetBool("IsGrounded", true);
            IsGround = true;
            animator.SetBool("IsJumping", false);
            isJump = false;
        }
        else
        {
            animator.SetBool("IsGrounded", false);
            IsGround = false;

            if (isJump && VelocidadGravedad.y < 0)
            {
                animator.SetBool("IsFalling", true);
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            VelocidadGravedad.y = Mathf.Sqrt(HeightJump * -2f * gravedad);
            animator.SetBool("IsJumping", true);
            isJump = true;
        }

        VelocidadGravedad.y += gravedad * Time.deltaTime;
        Controller.Move(VelocidadGravedad * Time.deltaTime);
    }
}
