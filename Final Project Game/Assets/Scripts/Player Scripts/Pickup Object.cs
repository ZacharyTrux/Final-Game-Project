using Unity.VisualScripting;
using UnityEngine;

public class PickupObject : MonoBehaviour{
    private Quaternion prevRot;
    private float originalZ;
    private Collider collider;
    public Material defaultMaterial;
    public Material outlineMaterial;
    private MeshRenderer renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        originalZ = transform.position.z;
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    public void SetOutline(){
        renderer.material = outlineMaterial;
    }

    public void ResetOutline(){
        renderer.material = defaultMaterial;
    }

    public void Pickup(Transform holdLocation){
        ResetOutline();
        prevRot = transform.rotation;
        collider.enabled = false;
        transform.SetParent(holdLocation);
        transform.localPosition = Vector3.zero;
    }

    public void Drop(Transform holdLocation){
        collider.enabled = true;
        transform.SetParent(null);
        transform.rotation = prevRot; 
        transform.position = new Vector3(holdLocation.position.x, holdLocation.position.y, originalZ);
    }
}
