using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Scroll Settings")]
    [Range(0f, 0.5f)]
    public float scrollSpeed = 0.05f;

    public bool scrollLeft = true;

    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend == null)
        {
            Debug.LogWarning(gameObject.name + ": No Renderer found!");
            return;
        }
    }

    private void Update()
    {
        if (rend == null) return;

        float offset = Mathf.Repeat(Time.time * scrollSpeed, 1f);
        float direction = scrollLeft ? -offset : offset;

        rend.material.mainTextureOffset = new Vector2(direction, 0f);
    }
}