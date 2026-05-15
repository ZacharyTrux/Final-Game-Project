using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

public class PlayerTopDown : MonoBehaviour{
    public float moveSpeed = 5f;
    public float pickupRange = 3f;

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
    private GameObject heldItem;
    private PickupObject currentTarget;
    

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
    private void OnDisable(){
        GetComponent<BoxCollider>().enabled = false;
        controls.Player.Disable();
        controls.Player.Interact.performed -= OnInteractPerformed;
        rb.linearVelocity = Vector3.zero;
    }

    private void Update(){
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        HandleRotation();
        //transform.position = new Vector3(transform.position.x, transform.position.y, spawnPoint.transform.position.z);

        CheckForNearbyObjects();
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
        if(currentTarget != null){
            currentTarget.Pickup(holdLocation);
            heldItem = currentTarget.gameObject;
            currentTarget = null;
        }
    }

    private void CheckForNearbyObjects(){
        if(heldItem != null) return;

        Collider[] objects = Physics.OverlapSphere(transform.position, pickupRange, boxLayer);
        
        PickupObject closestObject = null;
        float closestDist = Mathf.Infinity;

        foreach(Collider obj in objects){
            float dist = (transform.position - obj.transform.position).sqrMagnitude;
            if(dist < closestDist)
            {
                if(obj.TryGetComponent(out PickupObject pickupComponent))
                {
                    closestDist = dist;
                    closestObject = pickupComponent;
                }
            }
        }

        if(closestObject != currentTarget){
            if(currentTarget != null){
                currentTarget.ResetOutline();
            }
            currentTarget = closestObject;
            if(currentTarget != null){
                currentTarget.SetOutline();
            }
        }
    }

    public void Drop(){
        if(heldItem == null) return;
        heldItem.GetComponent<PickupObject>().Drop(holdLocation);
        heldItem = null;
    }

    public void TakeDamage(){
        PlayerManager.Instance.TakeDamage(false);
    }

    public void SetSpawn(Transform position){
        spawnPoint = position;
    }

    public GameObject GetHeldItem(){
        return heldItem;
    }

    public void Respawn(){
        transform.position = spawnPoint.position;
    }
}