using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerTopDown : MonoBehaviour{
    public float moveSpeed = 5f;
    public float pickupRange = 1.5f;
    
    // layers
    public LayerMask boxLayer;
    // placement locations
    public Transform holdLocation;
    public Transform placeLocation;

    public Grid grid;

    private bool isMoving;
    private Rigidbody rb;
    private PlayerInput controls;
    private Vector2 moveInput;
    private GameObject heldBox;
    

    void Awake(){
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

        if (!isMoving && moveInput != Vector2.zero){
            Vector2 gridPos = SnapToCell(moveInput);
            Vector3Int currentCell = grid.WorldToCell(transform.position);
            Vector3Int targetCell = currentCell + new Vector3Int((int)gridPos.x, (int)gridPos.y);
            Vector3 targetPos = grid.GetCellCenterWorld(targetCell);
            targetPos.z = transform.position.z;
            StartCoroutine(MoveToCell(targetPos));
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

    Vector2 SnapToCell(Vector2 input){
        if(Mathf.Abs(input.x) >= Mathf.Abs(input.y)){
            return new Vector2(Mathf.Sign(input.x), 0);
        }
        else{
            return new Vector2(0, Mathf.Sign(input.y));
        }
    }

    IEnumerator MoveToCell(Vector3 cell){
        isMoving = true;
        while(Vector3.Distance(transform.position, cell) > 0.01f){
            transform.position = Vector3.MoveTowards(transform.position, cell, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = cell;
        isMoving = false;
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

