using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [Header("Renderer")]
    public Renderer ghostRenderer;

    [Header("Shader Graph Property Names")]
    public string flashAmountProperty = "_FloatAmount";
    public string flashColorProperty = "_FlashColor";

    [Header("Flash Settings")]
    [ColorUsage(true, true)]
    public Color flashColor = Color.yellow;

    public float flashTime = 1.5f;

    [Header("Flash Curve")]
    public AnimationCurve flashSpeedCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(1f, 0f)
    );

    private Material ghostMaterial;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (ghostRenderer == null)
        {
            ghostRenderer = GetComponentInChildren<Renderer>();
        }

        if (ghostRenderer != null)
        {
            ghostMaterial = ghostRenderer.material;

            Debug.Log("DamageFlash found renderer: " + ghostRenderer.name);
            Debug.Log("DamageFlash material: " + ghostMaterial.name);

            SetFlashColor(flashColor);
            SetFlashAmount(0f);
        }
        else
        {
            Debug.LogWarning("DamageFlash: No Renderer found on ghost.");
        }
    }

    public void CallDamageFlash()
    {
        if (ghostMaterial == null)
        {
            Debug.LogWarning("DamageFlash: ghostMaterial is null.");
            return;
        }

        Debug.Log("DamageFlash started.");

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        SetFlashColor(flashColor);

        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = elapsedTime / flashTime;
            float currentFlashAmount = flashSpeedCurve.Evaluate(normalizedTime);

            SetFlashAmount(currentFlashAmount);

            yield return null;
        }

        SetFlashAmount(0f);
        flashCoroutine = null;

        Debug.Log("DamageFlash finished.");
    }

    private void SetFlashAmount(float amount)
    {
        if (ghostMaterial == null) return;

        if (ghostMaterial.HasProperty(flashAmountProperty))
        {
            ghostMaterial.SetFloat(flashAmountProperty, amount);
        }
        else
        {
            Debug.LogWarning("Ghost material does not have property: " + flashAmountProperty);
        }
    }

    private void SetFlashColor(Color color)
    {
        if (ghostMaterial == null) return;

        if (ghostMaterial.HasProperty(flashColorProperty))
        {
            ghostMaterial.SetColor(flashColorProperty, color);
        }
        else
        {
            Debug.LogWarning("Ghost material does not have property: " + flashColorProperty);
        }
    }
}