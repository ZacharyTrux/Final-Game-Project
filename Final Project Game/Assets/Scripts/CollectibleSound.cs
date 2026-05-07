using UnityEngine;

public class CollectibleSound : MonoBehaviour
{
    public AudioClip collectSound;
    public float volume = 1f;

    private Collider itemCollider;
    private Renderer itemRenderer;
    private bool collected = false;

    private void Start()
    {
        itemCollider = GetComponent<Collider>();
        itemRenderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (other.CompareTag("TopDownPlayer") || other.CompareTag("2DPlayer"))
        {
            collected = true;

            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
            }

            if (itemCollider != null)
                itemCollider.enabled = false;

            if (itemRenderer != null)
                itemRenderer.enabled = false;

            Destroy(gameObject, 0.2f);
        }
    }
}