using UnityEngine;

public class GhostAlphaVisualEffect : MonoBehaviour
{
    [Header("Visual Renderers")]
    public Renderer[] visualRenderers;

    private Color[] originalColors;

    private void Awake()
    {
        if (visualRenderers == null || visualRenderers.Length == 0)
        {
            visualRenderers = GetComponentsInChildren<Renderer>();
        }

        SaveOriginalColors();
    }

    private void SaveOriginalColors()
    {
        if (visualRenderers == null) return;

        originalColors = new Color[visualRenderers.Length];

        for (int i = 0; i < visualRenderers.Length; i++)
        {
            Material mat = visualRenderers[i].material;

            if (mat.HasProperty("_BaseColor"))
            {
                originalColors[i] = mat.GetColor("_BaseColor");
            }
            else if (mat.HasProperty("_Color"))
            {
                originalColors[i] = mat.color;
            }
            else
            {
                originalColors[i] = Color.white;
            }
        }
    }

    public void SetGhostAlpha(float alpha)
    {
        if (visualRenderers == null || originalColors == null) return;

        for (int i = 0; i < visualRenderers.Length; i++)
        {
            Material mat = visualRenderers[i].material;
            Color c = originalColors[i];
            c.a = alpha;

            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", c);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.color = c;
            }
        }
    }

    public void ResetAlpha()
    {
        SetGhostAlpha(1f);
    }
}