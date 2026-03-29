using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("基础移动")]
    public float moveSpeed = 6f;
    public float sprintSpeed = 12f;
    public float crouchSpeed = 3f;
    
    [Header("跳跃设置")]
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f;
    public int maxJumpCount = 2;
    
    [Header("滑铲设置")]
    public float slideDuration = 1f;
    public float slideSpeed = 15f;
    public float slideHeight = 0.5f;
    
    [Header("地面检测")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    [Header("角色尺寸")]
    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    
    [Header("物理设置")]
    public float gravity = -25f;
    public float terminalVelocity = -50f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private int jumpCount = 0;
    private bool isSliding = false;
    private bool isCrouching = false;
    private float slideTimer = 0f;
    private float currentSpeed;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
    }
    
    void Update()
    {
        HandleGroundCheck();
        HandleCrouch();
        HandleSlide();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }
    
    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpCount = 0;
        }
    }
    
    void HandleMovement()
    {
        if (isSliding) return;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection = moveDirection.normalized;
        
        // 确定当前速度
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            currentSpeed = sprintSpeed;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
        
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }
    
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount && !isSliding)
        {
            float jumpPower = (jumpCount == 0) ? jumpForce : doubleJumpForce;
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
            jumpCount++;
        }
    }
    
    void HandleCrouch()
    {
        if (isSliding) return;
        
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            if (!isCrouching)
            {
                isCrouching = true;
                controller.height = crouchHeight;
                controller.center = new Vector3(0, crouchHeight / 2, 0);
            }
        }
        else
        {
            if (isCrouching)
            {
                // 检查上方是否有空间站起
                if (CanStandUp())
                {
                    isCrouching = false;
                    controller.height = normalHeight;
                    controller.center = new Vector3(0, normalHeight / 2, 0);
                }
            }
        }
    }
    
    void HandleSlide()
    {
        // 开始滑铲
        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && isGrounded && !isSliding)
        {
            StartSlide();
        }
        
        // 滑铲中
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            
            // 沿当前面向方向滑动
            Vector3 slideDirection = transform.forward;
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
            
            // 滑铲结束
            if (slideTimer <= 0 || Input.GetKeyDown(KeyCode.Space))
            {
                EndSlide();
            }
        }
    }
    
    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        controller.height = slideHeight;
        controller.center = new Vector3(0, slideHeight / 2, 0);
    }
    
    void EndSlide()
    {
        isSliding = false;
        
        if (CanStandUp())
        {
            controller.height = normalHeight;
            controller.center = new Vector3(0, normalHeight / 2, 0);
        }
        else
        {
            isCrouching = true;
            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 2, 0);
        }
    }
    
    bool CanStandUp()
    {
        // 使用射线检测上方是否有空间
        return !Physics.Raycast(transform.position, Vector3.up, normalHeight - controller.height + 0.1f);
    }
    
    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, terminalVelocity);
        controller.Move(velocity * Time.deltaTime);
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
    
    // 公共方法供其他脚本调用
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public bool IsSliding()
    {
        return isSliding;
    }
    
    public bool IsCrouching()
    {
        return isCrouching;
    }
    
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
