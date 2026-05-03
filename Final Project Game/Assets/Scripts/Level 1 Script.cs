using UnityEngine;

public class LevelScript : MonoBehaviour{
    public GameObject tree;
    public GameObject waterfall;
    public GameObject bigTree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Execute(){
        waterfall.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
