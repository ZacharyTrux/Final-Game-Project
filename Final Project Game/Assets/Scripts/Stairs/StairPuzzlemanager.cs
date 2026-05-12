using UnityEngine;

public class StairPuzzleManager : MonoBehaviour
{
    public int currentStage = 0;

    [Header("Bombs to disable after each stage")]
    public GameObject[] disableAtStage1;
    public GameObject[] disableAtStage2;
    public GameObject[] disableAtStage3;
    public GameObject[] disableAtStage4;
    public GameObject[] disableAtStage5;
    public GameObject[] disableAtStage6;
    public GameObject[] disableAtStage7;
    public GameObject[] disableAtStage8;
    public GameObject[] disableAtStage9;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctStepSound;
    public AudioClip wrongMoveSound;
    public AudioClip puzzleCompleteSound;

    public void TryAdvanceStage(int requiredStage, int nextStage)
    {
        Debug.Log("Trying trigger. Current Stage: " + currentStage +
                  " | Required Stage: " + requiredStage +
                  " | Next Stage: " + nextStage);

        if (currentStage == requiredStage)
        {
            currentStage = nextStage;
            Debug.Log("Puzzle advanced to stage: " + currentStage);

            PlaySound(correctStepSound);
            UpdateBombs();
        }
        else
        {
            Debug.Log("Wrong move. Current stage is: " + currentStage);

            PlaySound(wrongMoveSound);

            if (ScoringManager.Instance != null)
            {
                ScoringManager.Instance.WrongMovePenalty();
            }
        }
    }

    public bool IsPuzzleReadyForPortal()
    {
        return currentStage >= 9;
    }

    public void CompletePuzzle()
    {
        Debug.Log("Stair puzzle completed through portal.");

        PlaySound(puzzleCompleteSound);

        if (ScoringManager.Instance != null)
        {
            ScoringManager.Instance.PuzzleCompleteBonus();
        }
    }

    private void UpdateBombs()
    {
        if (currentStage == 1) SetObjectsActive(disableAtStage1, false);
        if (currentStage == 2) SetObjectsActive(disableAtStage2, false);
        if (currentStage == 3) SetObjectsActive(disableAtStage3, false);
        if (currentStage == 4) SetObjectsActive(disableAtStage4, false);
        if (currentStage == 5) SetObjectsActive(disableAtStage5, false);
        if (currentStage == 6) SetObjectsActive(disableAtStage6, false);
        if (currentStage == 7) SetObjectsActive(disableAtStage7, false);
        if (currentStage == 8) SetObjectsActive(disableAtStage8, false);
        if (currentStage == 9) SetObjectsActive(disableAtStage9, false);
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null || objects.Length == 0)
        {
            Debug.LogWarning("No objects assigned for stage " + currentStage);
            return;
        }

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                Debug.Log("Turning " + obj.name + " active = " + active);
                obj.SetActive(active);
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}