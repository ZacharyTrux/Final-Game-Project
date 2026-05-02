using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 7f;

    [Header("Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody rb;
    private PlayerInput controls;
    private Vector2 moveInput;
    private bool isGrounded;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        controls = new PlayerInput();
    }

    void OnEnable(){
        rb.useGravity = true;
        controls.Player.Enable();
        controls.Player.Jump.performed += OnJumpPerformed;
    }

    void OnDisable(){
        rb.useGravity = false;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Disable();
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context){
        print(isGrounded);
        if(isGrounded){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context){
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.2f);
        if(hit != null){
            print("interactable found");
            var interactable = GetComponent<IInteractable>();
            interactable.Interact();
        }
    }

    void Update(){
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius);
    }

    void FixedUpdate(){ // consistent jumping physics
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
}