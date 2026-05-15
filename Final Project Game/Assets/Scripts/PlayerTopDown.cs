using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerTopDown : MonoBehaviour{
    public float moveSpeed = 5f;
    public float pickupRange = 10f;

    private int maxHealth = 3;
    public int currHealth;

    
    // layers
    public LayerMask boxLayer;
    // placement locations
    public Transform holdLocation;
    public Transform placeLocation;

    private Transform spawnPoint;

    private Rigidbody rb;
    private PlayerInput controls;
    private Vector2 moveInput;
    private GameObject heldItem;
    private Quaternion heldItemRotation;
    private float heldItemZ; 
    

    void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        controls = new PlayerInput();
        heldItem = null;
        currHealth = maxHealth;
    }

    private void OnEnable(){
        GetComponent<BoxCollider>().enabled = true;
        controls.Enable();
        controls.Player.Interact.performed += OnInteractPerformed;
    }
    private void OnDisable()
    {
        GetComponent<BoxCollider>().enabled = false;
        controls.Player.Disable();
        controls.Player.Interact.performed -= OnInteractPerformed;
        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Update(){
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        HandleRotation();

        if(heldItem != null){
            heldItem.transform.localRotation = Quaternion.identity;
        }
    }

    private void FixedUpdate(){
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, moveInput.y * moveSpeed, 0f);
    }

    private void HandleRotation(){
        if (moveInput == Vector2.zero) return;

        Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0f);

        Quaternion targetRotation =
            Quaternion.LookRotation(Vector3.forward, moveDir) *
            Quaternion.Euler(0f, 0f, 180f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            20f * Time.deltaTime
        );
    }

    private void OnInteractPerformed(InputAction.CallbackContext context){
        if(heldItem == null){
            TryPickUp();
        }
        else{
            Drop();
        }
    }

    void TryPickUp(){
        Collider[] objects = Physics.OverlapSphere(transform.position, pickupRange, boxLayer);
        if(objects.Length == 0) return;

        Collider closest = null;
        float closestDist = Mathf.Infinity;
        foreach(Collider obj in objects){
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if(dist < closestDist){
                closestDist = dist;
                closest = obj;
            }
        }

        // store original info on item
        heldItem = closest.gameObject;
        heldItemRotation = heldItem.transform.rotation;
        heldItemZ = heldItem.transform.position.z;

        heldItem.GetComponent<Collider>().enabled = false;
        heldItem.transform.SetParent(holdLocation);
        heldItem.transform.localPosition = Vector3.zero;
    }

    public void Drop(){
        if(heldItem == null) return;
        heldItem.GetComponent<Collider>().enabled = true;
        heldItem.transform.SetParent(null);
        heldItem.transform.position = new Vector3(placeLocation.position.x, placeLocation.position.y, heldItemZ);
        heldItem.transform.rotation = heldItemRotation;
        heldItem = null;
    }

    public void TakeDamage(){
        PlayerManager.Instance.TakeDamage();
    }

    public void SetSpawn(Transform position){
        spawnPoint = position;
    }
    public Transform SpawnPoint => spawnPoint;

    public GameObject GetHeldItem(){
        return heldItem;
    }
}