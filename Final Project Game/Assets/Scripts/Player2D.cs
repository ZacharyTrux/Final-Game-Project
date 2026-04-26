using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Detection")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private PlayerInput controls;
    private Vector2 moveInput;
    private bool isGrounded;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerInput();
    }

    void OnEnable(){
        controls.Player.Enable();
        controls.Player.Jump.performed += OnJumpPerformed;
        rb.gravityScale = 1f;
    }

    void OnDisable(){
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Disable();
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context){
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void Update(){
        // Read movement value from the strongly-typed property
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void FixedUpdate(){
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
}