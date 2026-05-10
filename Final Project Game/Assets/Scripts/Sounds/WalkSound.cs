using UnityEngine;

public class WalkSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip walkClip;

    public float stepDelay = 0.35f;
    private float stepTimer;

    public void PlayWalkSound()
    {
        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            audioSource.PlayOneShot(walkClip);
            stepTimer = stepDelay;
        }
    }
}
