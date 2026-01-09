using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;

    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;

    private Rigidbody rb;
    private CapsuleCollider col;
    private Transform cam;
    private float xRotation = 0f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        col = GetComponent<CapsuleCollider>();
        cam = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleCrouch();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float speed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift)) speed = runSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) speed = crouchSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 velocity = move * speed;
        velocity.y = rb.velocity.y; // сохранить вертикальную скорость (гравитацию)
        rb.velocity = velocity;

        // ѕровер€ем, есть ли контакт с землЄй
        isGrounded = Physics.Raycast(transform.position, Vector3.down, col.height / 2 + 0.1f);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            col.height = crouchHeight;
            Vector3 camPos = cam.localPosition;
            camPos.y = crouchHeight - 0.2f;
            cam.localPosition = camPos;
        }
        else
        {
            col.height = standingHeight;
            Vector3 camPos = cam.localPosition;
            camPos.y = standingHeight - 0.2f;
            cam.localPosition = camPos;
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
