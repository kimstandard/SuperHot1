using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    public Transform cameraTransform;
    private Animator animator;

    private CharacterController controller;
    private float pitch = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    public GameObject dialoguePanel;

    [Header("총 정보")]
    public Weapon equippedWeapon;
    public GunRecoil gunRecoil;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            return;
        }

            // 공격 입력
            if (Input.GetButtonDown("Fire1"))
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.Attack();
                //gunRecoil.ApplyRecoil();
            }
            else if (animator != null)
            {
                //animator.SetTrigger("Punching");
            }
        }        
        

        // 시점 회전
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // 바닥 체크
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 이동 입력
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * moveSpeed * Time.deltaTime);

        animator.SetFloat("forwardSpeed", moveSpeed);

        // 점프 입력
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
}
