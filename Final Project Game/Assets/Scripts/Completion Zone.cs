using UnityEngine;
using TMPro;
using System.Collections;

// DEBUG VERSION — check Unity Console for [CZ] prefixed logs

public class CompletionZone : MonoBehaviour
{
    public GameObject player2DTarget;
    public GameObject playerTDTarget;

    private bool present2D = false;
    private bool presentTD = false;
    private bool isCompleted = false;
    private TextMeshProUGUI hintText;

    private Transform player2DTransform;
    private Transform playerTDTransform;

    private float alignDuration = 1.0f;
    private float duration = 2f;
    private float spinSpeed = 1000f;

    private void Awake()
    {
        if (hintText == null)
        {
            hintText = GameObject.FindWithTag("Hint").GetComponent<TextMeshProUGUI>();
        }
    }

    // Called by LevelManager when a new sublevel loads — resets this zone for reuse
    public void ResetZone()
    {
        present2D = false;
        presentTD = false;
        isCompleted = false;
        Debug.Log($"[CZ] ResetZone called on {gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[CZ] OnTriggerEnter — object: {other.gameObject.name}, isCompleted: {isCompleted}, present2D: {present2D}, presentTD: {presentTD}");

        UIScript.Instance.ShowCompletionPrompt();

        if (other.GetComponent<PlayerTopDown>() != null)
        {
            presentTD = true;
            playerTDTransform = other.transform;
            Debug.Log("[CZ] TopDown player entered");
        }
        if (other.GetComponent<Player2D>() != null)
        {
            present2D = true;
            player2DTransform = other.transform;
            Debug.Log("[CZ] 2D player entered");
        }
        if (present2D && presentTD && !isCompleted)
        {
            Debug.Log("[CZ] ✅ Both players present — starting PortalAnimation");
            hintText.text = "";
            isCompleted = true;
            StartCoroutine(PortalAnimation());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Don't allow exit events to undo state once animation is running
        if (isCompleted)
        {
            Debug.Log($"[CZ] OnTriggerExit ignored (isCompleted=true) for: {other.gameObject.name}");
            return;
        }

        UIScript.Instance.ResetHintText();
        if (other.GetComponent<Player2D>() != null)
        {
            present2D = false;
            Debug.Log("[CZ] 2D player exited zone");
        }
        if (other.GetComponent<PlayerTopDown>() != null)
        {
            presentTD = false;
            Debug.Log("[CZ] TopDown player exited zone");
        }
    }

    private IEnumerator PortalAnimation()
    {
        Debug.Log("[CZ] PortalAnimation started");

        // --- Disable physics so Rigidbody doesn't fight transform animation ---
        Rigidbody rb2D = player2DTransform.GetComponent<Rigidbody>();
        Rigidbody rbTD = playerTDTransform.GetComponent<Rigidbody>();

        // Zero velocity BEFORE setting kinematic (avoids "kinematic body" error)
        rb2D.linearVelocity = Vector3.zero;
        rb2D.angularVelocity = Vector3.zero;
        rbTD.linearVelocity = Vector3.zero;
        rbTD.angularVelocity = Vector3.zero;

        rb2D.isKinematic = true;
        rbTD.isKinematic = true;

        // Reset rotation immediately so tilt can't carry forward
        player2DTransform.rotation = Quaternion.identity;
        playerTDTransform.rotation = Quaternion.identity;

        Debug.Log($"[CZ] Physics disabled — rb2D.isKinematic: {rb2D.isKinematic}");

        PlayerManager.Instance.enabled = false;
        Debug.Log("[CZ] PlayerManager disabled");

        // --- Phase 1: Move players to target spots ---
        Vector3 start2DPos = player2DTransform.position;
        Vector3 startTDPos = playerTDTransform.position;
        float alignElapsed = 0f;

        while (alignElapsed < alignDuration)
        {
            alignElapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(alignElapsed / alignDuration);
            player2DTransform.position = Vector3.Lerp(start2DPos, player2DTarget.transform.position, progress);
            playerTDTransform.position = Vector3.Lerp(startTDPos, playerTDTarget.transform.position, progress);
            yield return null;
        }

        player2DTransform.position = player2DTarget.transform.position;
        playerTDTransform.position = playerTDTarget.transform.position;
        Debug.Log($"[CZ] Align phase done — 2D at: {player2DTransform.position}");

        yield return new WaitForSeconds(1f);

        // --- Phase 2: Spin & shrink into portal ---
        start2DPos = player2DTransform.position;
        startTDPos = playerTDTransform.position;
        Vector3 portalCenter = transform.position;

        Vector3 startScale2D = player2DTransform.localScale;
        Vector3 startScaleTD = playerTDTransform.localScale;

        // Save identity as target — we ALWAYS want player upright on next level
        Quaternion saved2DRot = Quaternion.identity;
        Quaternion savedTDRot = Quaternion.identity;

        float timeElapsed = 0f;
        SoundManager.Play(SoundType.PORTAL);

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(timeElapsed / duration);
            float easeProgress = Mathf.Pow(progress, 3);

            player2DTransform.position = Vector3.Lerp(start2DPos, portalCenter, easeProgress);
            playerTDTransform.position = Vector3.Lerp(startTDPos, portalCenter, easeProgress);

            player2DTransform.localScale = Vector3.Lerp(startScale2D, Vector3.zero, easeProgress);
            playerTDTransform.localScale = Vector3.Lerp(startScaleTD, Vector3.zero, easeProgress);

            float currentSpin = Mathf.Lerp(0, spinSpeed, easeProgress);
            player2DTransform.Rotate(Vector3.forward, currentSpin * Time.deltaTime);
            playerTDTransform.Rotate(Vector3.up, currentSpin * Time.deltaTime);

            yield return null;
        }

        SoundManager.Stop(SoundType.PORTAL);
        Debug.Log("[CZ] Spin phase done");

        // --- Reset transform state BEFORE completing level ---
        // Reset transforms
        player2DTransform.localScale = startScale2D;
        playerTDTransform.localScale = startScaleTD;
        player2DTransform.rotation = saved2DRot;
        playerTDTransform.rotation = savedTDRot;

        // FIX: Restore kinematic HERE, before CompleteCurrSublevel,
        // so that GroupRespawn()'s transform.position assignment actually works
        rb2D.isKinematic = false;
        rbTD.isKinematic = false;

        yield return null; // one frame for physics to acknowledge non-kinematic state

        Debug.Log("[CZ] Calling CompleteCurrSublevel...");
        LevelManager.Instance.CompleteCurrSublevel();
    }
}