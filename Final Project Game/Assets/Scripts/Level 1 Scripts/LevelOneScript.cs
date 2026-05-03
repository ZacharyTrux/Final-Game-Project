using UnityEngine;

public class LevelOneScript : MonoBehaviour, IExecute{
    public GameObject tree;
    public GameObject waterfall;
    public GameObject bigTree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Execute(){
        print("Waterfall gone");
        waterfall.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}    