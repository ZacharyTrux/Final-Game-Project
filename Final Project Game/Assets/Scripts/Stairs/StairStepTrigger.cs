using UnityEngine;

public class StairStepTrigger : MonoBehaviour
{
    public StairPuzzleManager puzzleManager;

    public int requiredStage;
    public int nextStage;

    public string requiredPlayerTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(requiredPlayerTag))
        {
            puzzleManager.TryAdvanceStage(requiredStage, nextStage);
        }
    }
}