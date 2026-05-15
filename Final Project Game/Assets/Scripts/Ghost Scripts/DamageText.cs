using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public static DamageText Instance { get; private set; }

    [Header("Fade Settings")]
    public float displayDuration = 0.5f;  
    public float fadeDuration = 0.8f;     

    private TextMeshProUGUI tmp;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        Instance = this;
        tmp = GetComponent<TextMeshProUGUI>();

        SetAlpha(0f);
    }

    public void Show(string message)
    {
        if (tmp == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        tmp.text = message;
        SetAlpha(1f);   

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        fadeCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }
}