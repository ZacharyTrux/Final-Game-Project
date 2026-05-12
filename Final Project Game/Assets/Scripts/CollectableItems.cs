using UnityEngine;

public enum CollectableType
{
    For2DPlayer,
    ForTopDownPlayer
}

public class CollectableItems : MonoBehaviour
{
    [Header("Score")]
    public int scoreAmount = 100;

    [Header("Who can collect this fruit")]
    public CollectableType collectableType;

    [Header("Player Tags")]
    public string player2DTag = "2DPlayer";
    public string topDownPlayerTag = "TopDownPlayer";

    private bool collected = false;
    private Collider itemCollider;

    private void Awake()
    {
        itemCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        UpdateColliderByCurrentPlayer();
    }

    private void UpdateColliderByCurrentPlayer()
    {
        if (PlayerManager.Instance == null || itemCollider == null) return;

        if (collectableType == CollectableType.For2DPlayer)
        {
            itemCollider.enabled = PlayerManager.Instance.Is2DActive();
        }
        else if (collectableType == CollectableType.ForTopDownPlayer)
        {
            itemCollider.enabled = PlayerManager.Instance.IsTopDownActive();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (!CanThisPlayerCollect(other)) return;

        collected = true;

        if (ScoringManager.Instance != null){
            ScoringManager.Instance.AddScore(scoreAmount);
        }
        else{
            Debug.LogWarning("No ScoringSystem found in the scene.");
        }
        Destroy(gameObject);
        SoundManager.Play(SoundType.PICKUP, other.GetComponent<AudioSource>(), false);
        
    }

    private bool CanThisPlayerCollect(Collider other)
    {
        if (collectableType == CollectableType.For2DPlayer)
        {
            return other.CompareTag(player2DTag);
        }

        if (collectableType == CollectableType.ForTopDownPlayer)
        {
            return other.CompareTag(topDownPlayerTag);
        }

        return false;
    }
}