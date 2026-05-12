using UnityEngine;

public class SpikeKill : MonoBehaviour
{
    public Transform respawnPoint;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bombSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TopDownPlayer") || other.CompareTag("2DPlayer"))
        {
            if (respawnPoint == null)
            {
                Debug.LogError("Respawn point is missing on " + gameObject.name);
                return;
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.position = respawnPoint.position;
            }
            else
            {
                other.transform.position = respawnPoint.position;
            }

            PlayBombSound();

            Debug.Log(other.name + " respawned.");

            if (ScoringManager.Instance != null)
            {
                ScoringManager.Instance.BombPenalty();
            }
        }
    }

    private void PlayBombSound()
    {
        if (audioSource != null && bombSound != null)
        {
            audioSource.PlayOneShot(bombSound);
        }
    }
}