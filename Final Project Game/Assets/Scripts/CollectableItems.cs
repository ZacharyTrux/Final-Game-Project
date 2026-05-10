using UnityEngine;

public class CollectableItems : MonoBehaviour
{
    [Header("Score")]
    public int scoreAmount = 100;

    [Header("Who can collect this")]
    public LayerMask playerLayers;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        bool isPlayerLayer = (playerLayers.value & (1 << other.gameObject.layer)) != 0;
        if (!isPlayerLayer) return;

        collected = true;

        if (ScoringManager.Instance != null)
        {
            ScoringManager.Instance.AddScore(scoreAmount);
        }
        else
        {
            Debug.LogWarning("No ScoringSystem found in the scene.");
        }

        Destroy(gameObject);
    }
}