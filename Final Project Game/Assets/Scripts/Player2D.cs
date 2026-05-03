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

    public Transform spawnPoint;
    private int maxHealth = 3;
    public int currHealth;
    private Rigidbody rb;
    private PlayerInput controls;
    private Vector2 moveInput;
    private bool isGrounded;

    void Awake(){
        currHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        controls = new PlayerInput();
    }

    void OnEnable(){
        rb.useGravity = true;
        controls.Player.Enable();
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Interact.performed += OnInteractPerformed;
        Debug.Log("Subscribed to Interact: " + controls.Player.Interact.bindings.Count + " bindings");
    }

    void OnDisable(){
        rb.useGravity = false;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Interact.performed -= OnInteractPerformed;
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
        print("hit interact");
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.2f);
        
        if(hits.Length > 0){
            print("interactable found");
            var interactable = hits[0].GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }

    void Update(){
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius);
    }

    void FixedUpdate(){ // consistent jumping physics
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, 0f);
        rb.linearVelocity += Vector3.up * Physics.gravity.y * (1.5f) * Time.fixedDeltaTime;
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
}