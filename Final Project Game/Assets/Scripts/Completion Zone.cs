using UnityEngine;
using TMPro;
using System.Collections;

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

    private Coroutine portalCoroutine;

    private void Awake(){
        if (hintText == null){
            hintText = GameObject.FindWithTag("Hint").GetComponent<TextMeshProUGUI>();
        }
    }

    public void ResetZone(){
        present2D = false;
        presentTD = false;
        isCompleted = false;
    }

    private void OnTriggerEnter(Collider other){
        UIScript.Instance.ShowCompletionPrompt();

        if (other.GetComponent<PlayerTopDown>() != null){
            presentTD = true;
            playerTDTransform = other.transform;
        }
        if (other.GetComponent<Player2D>() != null){
            present2D = true;
            player2DTransform = other.transform;
        }
        if (present2D && presentTD && !isCompleted){
            hintText.text = "";
            isCompleted = true;
            portalCoroutine = StartCoroutine(PortalAnimation());
        }
    }

    private void OnTriggerExit(Collider other){
        if (isCompleted){
            return;
        }

        UIScript.Instance.ResetHintText();
        if (other.GetComponent<Player2D>() != null){
            present2D = false;
        }
        if (other.GetComponent<PlayerTopDown>() != null){
            presentTD = false;
        }
    }

    private IEnumerator PortalAnimation(){
        Rigidbody rb2D = player2DTransform.GetComponent<Rigidbody>();
        Rigidbody rbTD = playerTDTransform.GetComponent<Rigidbody>();

        rb2D.linearVelocity = Vector3.zero;
        rb2D.angularVelocity = Vector3.zero;
        rbTD.linearVelocity = Vector3.zero;
        rbTD.angularVelocity = Vector3.zero;

        rb2D.isKinematic = true;
        rbTD.isKinematic = true;

        player2DTransform.rotation = Quaternion.identity;
        playerTDTransform.rotation = Quaternion.identity;

        PlayerManager.Instance.enabled = false;

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

        yield return new WaitForSeconds(1f);

        start2DPos = player2DTransform.position;
        startTDPos = playerTDTransform.position;
        Vector3 portalCenter = transform.position;

        Vector3 startScale2D = player2DTransform.localScale;
        Vector3 startScaleTD = playerTDTransform.localScale;

        Quaternion saved2DRot = Quaternion.identity;
        Quaternion savedTDRot = Quaternion.identity;

        float timeElapsed = 0f;
        SoundManager.Play(SoundType.PORTAL);

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(timeElapsed / duration);
            float easeProgress = Mathf.Pow(progress, 3);

            if (player2DTransform != null)
                player2DTransform.position = Vector3.Lerp(start2DPos, portalCenter, easeProgress);
            if (playerTDTransform != null)
                playerTDTransform.position = Vector3.Lerp(startTDPos, portalCenter, easeProgress);

            if (player2DTransform != null)
                player2DTransform.localScale = Vector3.Lerp(startScale2D, Vector3.zero, easeProgress);
            if (playerTDTransform != null)
                playerTDTransform.localScale = Vector3.Lerp(startScaleTD, Vector3.zero, easeProgress);

            float currentSpin = Mathf.Lerp(0, spinSpeed, easeProgress);
            if (player2DTransform != null)
                player2DTransform.Rotate(Vector3.forward, currentSpin * Time.deltaTime);
            if (playerTDTransform != null)
                playerTDTransform.Rotate(Vector3.up, currentSpin * Time.deltaTime);

            yield return null;
        }

        SoundManager.Stop(SoundType.PORTAL);

        player2DTransform.localScale = startScale2D;
        playerTDTransform.localScale = startScaleTD;
        player2DTransform.rotation = saved2DRot;
        playerTDTransform.rotation = savedTDRot;

        rb2D.isKinematic = false;
        rbTD.isKinematic = false;

        LevelManager.Instance.CompleteCurrSublevel();

        rb2D.isKinematic = false;
        rbTD.isKinematic = false;
        PlayerManager.Instance.enabled = true;
    }
    private void OnDisable(){
        if (portalCoroutine != null){
            StopCoroutine(portalCoroutine);
            portalCoroutine = null;
        }
        player2DTransform = null;
        playerTDTransform = null;
    }
}
