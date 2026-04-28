using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTopDown : MonoBehaviour{
    public float moveSpeed = 5f;
    public float pickupRange = 1.5f;
    public LayerMask boxLayer;
    public Transform holdLocation;
    public Transform placeLocation;

    private Rigidbody rb;
    private PlayerInput controls;
    private Vector2 moveInput;
    private GameObject heldBox;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerInput();
        rb.useGravity = false;
        heldBox = null;
        //rb.constraints = RigidbodyConstraints.FreezeRotationX;
    }

    void OnEnable() => controls.Enable();
    void OnDisable(){
        controls.Player.Disable();
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector3.zero;
    }

    void Update(){
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            rb.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        if(controls.Player.Interact.WasPressedThisFrame()){
            if(heldBox == null){
                TryPickUp();
            }
            else{
                Drop();
            }
        }
    }

    void TryPickUp(){
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, boxLayer);
        if(hits.Length > 0){
            heldBox = hits[0].gameObject;
            heldBox.transform.SetParent(holdLocation); // have object move with player 
            heldBox.transform.localPosition = Vector3.zero;
            heldBox.transform.rotation = Quaternion.identity;
        }
    }

    void Drop(){
        heldBox.transform.SetParent(null);
        Vector3 targetPos = placeLocation.position;
        heldBox.transform.position = targetPos;
        heldBox = null;
    }

    void FixedUpdate(){
        rb.linearVelocity = moveInput * moveSpeed;
    }
}

