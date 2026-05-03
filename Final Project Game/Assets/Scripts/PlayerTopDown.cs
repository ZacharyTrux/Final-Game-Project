using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerTopDown : MonoBehaviour{
    public float moveSpeed = 5f;
    public float pickupRange = 1.5f;

    private int maxHealth = 3;
    public int currHealth;

    
    // layers
    public LayerMask boxLayer;
    // placement locations
    public Transform holdLocation;
    public Transform placeLocation;

    public Transform spawnPoint;
    
    private Rigidbody rb;
    private PlayerInput controls;
    private Vector2 moveInput;
    private GameObject heldBox;
    

    void Awake(){
        rb = GetComponent<Rigidbody>();
        controls = new PlayerInput();
        rb.useGravity = false;
        rb.freezeRotation = true;
        heldBox = null;
        currHealth = maxHealth;
    }

    void OnEnable(){
        controls.Enable();
        controls.Player.Interact.performed += OnInteractPerformed;
    } 
    void OnDisable(){
        controls.Player.Disable();
        controls.Player.Interact.performed -= OnInteractPerformed;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector3.zero;
    }

    void Update(){
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        HandleRotation();
    }

    void FixedUpdate(){
            rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, moveInput.y * moveSpeed, 0f);
    }

    private void HandleRotation(){
        if(moveInput == Vector2.zero) return;
    Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0f);
    Quaternion tRotation = Quaternion.LookRotation(Vector3.forward, moveDir);
    transform.rotation = Quaternion.Slerp(transform.rotation, tRotation, 20 * Time.deltaTime);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context){
        if(heldBox == null){
            TryPickUp();
        }
        else{
            Drop();
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

