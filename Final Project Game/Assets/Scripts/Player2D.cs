using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]

public class Player2D : MonoBehaviour
{   
    [Header("Visual")]
    [SerializeField] private Transform visualModel;
    [SerializeField] private float frontDelay = 0.15f;

    private float lastMoveTime;


    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float accel = 12;
    public float decel = 16f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Wall")]
    public float wallSpeed = 2f;

    [Header("Checks")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public Transform wallCheck;
    public float wallDistance = 0.5f;
    public LayerMask groundLayer;
    public LayerMask pickupLayer;

    [Header("Health")]
    public Transform spawnPoint;
    public int maxHealth = 3;
    public int currHealth;

    private Rigidbody rb;
    private PlayerInput controls;
    private float moveInput;
    private bool jumpPressed;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isGrounded;
    private bool isTouchingWall;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currHealth = maxHealth;
        controls = new PlayerInput();
    }

    void OnEnable(){
        rb.useGravity = true;
        controls.Player.Enable();
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Interact.performed += OnInteractPerformed;
    }

    void OnDisable(){
        rb.useGravity = false;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Interact.performed -= OnInteractPerformed;
        controls.Player.Disable();
        moveInput = 0f;
        rb.linearVelocity = Vector3.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context){
        jumpPressed = true;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context){
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.2f);
        
        if(hits.Length > 0){
            var interactable = hits[0].GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }

    void Update(){
        moveInput = controls.Player.Move.ReadValue<Vector2>().x;
        CheckEnvironment();
        HandleTimers();
        HandleJump();
        jumpPressed = false;
    }

    void FixedUpdate(){ // consistent jumping physics
        HandleMovement();
        HandleGravity();
        HandleWallSlide();
    }

    private void CheckEnvironment(){
        LayerMask ground = groundLayer | pickupLayer;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, ground);
        Vector3 facingDir = transform.right;
        isTouchingWall = Physics.Raycast(wallCheck.position, facingDir, wallDistance);
    }

    private void HandleTimers(){
        if(isGrounded){
            coyoteTimer = coyoteTime;
        }
        else{
            coyoteTimer -= Time.deltaTime;
        }

        if(jumpPressed){
            jumpBufferTimer = jumpBufferTime;
        }
        else{
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    private void HandleMovement(){
        float targetSpeed = moveInput * moveSpeed;

        if(isTouchingWall){
            targetSpeed = 0f;
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.1f) ? accel : decel;
        rb.AddForce(Vector3.right * speedDiff * accelRate);

        if (moveInput > 0.01f)
        {
            visualModel.localRotation = Quaternion.Euler(0f, -90f, 0f);
            lastMoveTime = Time.time;
        }
        else if (moveInput < -0.01f)
        {
            visualModel.localRotation = Quaternion.Euler(0f, 90f, 0f);
            lastMoveTime = Time.time;
        }
        else if (Time.time - lastMoveTime > frontDelay)
        {
            visualModel.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    private void HandleJump(){
        if (jumpPressed)
        {
            Debug.Log("Jump pressed. isGrounded=" + isGrounded + 
                    " coyoteTimer=" + coyoteTimer + 
                    " jumpBufferTimer=" + jumpBufferTimer);
        }
        if(jumpBufferTimer > 0 && coyoteTimer > 0){
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
            jumpBufferTimer = 0;
            coyoteTimer = 0;
        }
    }

    void HandleGravity(){
        if (isTouchingWall && !isGrounded) return; // wall slide handles this

        float yVel = rb.linearVelocity.y;

        if (yVel < 0)
            rb.AddForce(Vector3.up * Physics.gravity.y * (fallMultiplier - 1f), ForceMode.Acceleration);
        else if (yVel > 0)
            rb.AddForce(Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f), ForceMode.Acceleration);
    }

    private void HandleWallSlide(){
        if(isTouchingWall && !isGrounded && rb.linearVelocity.y < 0){
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallSpeed, 0f);
        }
    }

    public void TakeDamage(){
        currHealth -= 1;
        if(currHealth < 0){
            return;
        }
        Respawn();
    }

    public void Respawn(){
        rb.linearVelocity = Vector3.zero;
        transform.position = spawnPoint.position;
    }

    public void SetSpawn(Transform position){
        spawnPoint = position;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                wallCheck.position,
                wallCheck.position + transform.right * wallDistance
            );
        }
    }
}