using UnityEngine;

public class LeverPuzzle : MonoBehaviour, IExecute{
    public GameObject tree;
    public GameObject waterfall;

    public void Execute(){
        print("Waterfall gone");
        waterfall.SetActive(false);
    }
}    