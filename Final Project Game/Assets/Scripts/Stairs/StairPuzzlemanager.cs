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

    [Header("Bridge")]
    public GameObject bridgePiece;
    public Transform bridgeTarget;
    public float bridgeMoveSpeed = 3f;

    private bool moveBridge = false;
    private bool bridgeStarted = false;

    private void Update()
    {
        if (moveBridge && bridgePiece != null && bridgeTarget != null)
        {
            bridgePiece.transform.position = Vector3.MoveTowards(
                bridgePiece.transform.position,
                bridgeTarget.position,
                bridgeMoveSpeed * Time.deltaTime
            );
        }
    }

    public void TryAdvanceStage(int requiredStage, int nextStage)
    {
        Debug.Log("Trying trigger. Current Stage: " + currentStage +
                  " | Required Stage: " + requiredStage +
                  " | Next Stage: " + nextStage);

        if (currentStage == requiredStage)
        {
            currentStage = nextStage;
            Debug.Log("Puzzle advanced to stage: " + currentStage);

            UpdateBombs();

            if (currentStage == 10)
            {
                StartBridge();
            }
        }
        else
        {
            Debug.Log("Wrong move. Current stage is: " + currentStage);

            if (ScoringManager.Instance != null)
            {
                ScoringManager.Instance.WrongMovePenalty();
            }
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
            else
            {
                Debug.LogWarning("Empty bomb slot at stage " + currentStage);
            }
        }
    }

    private void StartBridge()
    {
        if (bridgeStarted)
        {
            return;
        }

        bridgeStarted = true;

        Debug.Log("Bridge is joining!");
        moveBridge = true;

        if (ScoringManager.Instance != null)
        {
            ScoringManager.Instance.PuzzleCompleteBonus();
        }
    }
}