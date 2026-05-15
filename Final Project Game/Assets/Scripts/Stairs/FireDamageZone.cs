using UnityEngine;

public class FireDamageZone : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private int scorePenalty = 100;

    private void OnTriggerEnter(Collider other)
    {
        Player2D player2D = other.GetComponent<Player2D>();
        PlayerTopDown playerTopDown = other.GetComponent<PlayerTopDown>();

        if (player2D != null || playerTopDown != null)
        {
            if (ScoringManager.Instance != null)
            {
                ScoringManager.Instance.AddPenalty(scorePenalty);
            }

            if (PlayerManager.Instance != null)
            {
                PlayerManager.Instance.TakeDamage();
            }

            Debug.Log(other.gameObject.name + " fell into fire.");
        }
    }
}
