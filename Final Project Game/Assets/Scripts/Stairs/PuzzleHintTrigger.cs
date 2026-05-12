using UnityEngine;
using TMPro;

public class PuzzleHintTrigger : MonoBehaviour
{
    [TextArea]
    public string hintMessage = "Hint: Watch the stair order. One player must move first to make the other player's path safe.";

    private TextMeshProUGUI hintText;

    private void Start()
    {
        GameObject hintObject = GameObject.FindWithTag("Hint");

        if (hintObject != null)
        {
            hintText = hintObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("No object with tag 'Hint' found.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("2DPlayer") || other.CompareTag("TopDownPlayer"))
        {
            if (hintText != null)
            {
                hintText.text = hintMessage;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("2DPlayer") || other.CompareTag("TopDownPlayer"))
        {
            if (hintText != null)
            {
                hintText.text = "";
            }
        }
    }
}
