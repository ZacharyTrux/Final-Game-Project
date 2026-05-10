using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]

public class Player2D : MonoBehaviour{   
    [Header("Movement Settings")]
    public float moveSpeed = 7f;

    [Header("Jump")]
    public float jumpForce = 22f;
    public float gravityMultiplier = 3f;
    public float fallMultiplier = 4.5f;

    [Header("Checks")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public Transform wallCheck;
    public float wallDistance = 0.5f;
    public LayerMask groundLayer;
    public LayerMask pickupLayer;

    [Header("Health")]
    public Transform spawnPoint;

    private Rigidbody rb;
    private PlayerInput controls;
    private float moveInput;
    private bool isGrounded;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        controls = new PlayerInput();
    }

    void OnEnable(){
        GetComponent<BoxCollider>().enabled = true;
        GetComponentInChildren<SpriteRenderer>().color = new Color(1f,1f,1f, 1f);
        rb.useGravity = true;
        controls.Player.Enable();
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Interact.performed += OnInteractPerformed;
    }

    void OnDisable(){
        GetComponent<BoxCollider>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().color = new Color(1f,1f,1f, 0.5f);
        rb.useGravity = false;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Interact.performed -= OnInteractPerformed;
        controls.Player.Disable();
        moveInput = 0f;
        rb.linearVelocity = Vector3.zero;
    }

    void Update(){
        moveInput = controls.Player.Move.ReadValue<Vector2>().x;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
    }

    void FixedUpdate(){
        bool hittingWallRight = moveInput > 0 && Physics.Raycast(wallCheck.position, Vector3.right, wallDistance, groundLayer);
        bool hittingWallLeft = moveInput < 0 && Physics.Raycast(wallCheck.position, Vector3.left, wallDistance, groundLayer);

        float targetVelocityX = moveInput * moveSpeed;

        if ((hittingWallRight && moveInput > 0) || (hittingWallLeft && moveInput < 0)){
            targetVelocityX = 0f;
        }
        
        rb.linearVelocity = new Vector3(targetVelocityX, rb.linearVelocity.y, 0f);

        if (!isGrounded){
            float currentMultiplier = (rb.linearVelocity.y < 0) ? fallMultiplier : gravityMultiplier;
            rb.AddForce(Physics.gravity * (currentMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context){
        if (isGrounded){
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context){
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.2f);
        
        if(hits.Length > 0){
            var interactable = hits[0].GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }

    public void TakeDamage(){
        PlayerManager.Instance.TakeDamage();
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
            // Draw a line to the right
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3.right * wallDistance));
            // Draw a line to the left
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3.left * wallDistance));
            
            // Draw small spheres at the end of the lines so you can see exactly where the check stops
            Gizmos.DrawWireSphere(wallCheck.position + (Vector3.right * wallDistance), 0.05f);
            Gizmos.DrawWireSphere(wallCheck.position + (Vector3.left * wallDistance), 0.05f);
        }
    }
    
}