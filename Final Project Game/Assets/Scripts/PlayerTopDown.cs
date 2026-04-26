using UnityEngine;

public class PlayerTopDown : MonoBehaviour{
    public float moveSpeed = 5f;
    
    private Rigidbody2D rb;
    private PlayerInput controls;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerInput();
        rb.gravityScale = 0f;
    }

    void OnEnable() => controls.Enable();
    void OnDisable(){
        controls.Player.Disable();
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            rb.rotation = targetAngle;
        }
    }

    void FixedUpdate(){
        rb.linearVelocity = moveInput * moveSpeed;
    }
}

