using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour{
    
    [System.Serializable]
    public class SubLevel{
        public string name;
        public Transform spawnPoint2D;
        public Transform spawnPointTD;
        public bool isCompleted;
    }

    public SubLevel[] subLevels;
    private int currLevel = 0;
    private Player2D player2D;
    private PlayerTopDown playerTD;
    public Scene nextMajorLevel;

    public static LevelManager Instance {get; private set;}

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start(){
        player2D = GameObject.FindWithTag("2DPlayer").GetComponent<Player2D>();
        playerTD = GameObject.FindWithTag("TopDownPlayer").GetComponent<PlayerTopDown>();
        SetSubLevel();
    }

    public void SetSubLevel(){
        player2D.SetSpawn(subLevels[currLevel].spawnPoint2D);
        playerTD.SetSpawn(subLevels[currLevel].spawnPointTD);
        player2D.Respawn();
        playerTD.Respawn();
    }

    public void CompleteCurrSublevel(){
        SubLevel curr = subLevels[currLevel];
        
        if(curr.isCompleted) return;

        curr.isCompleted = true;
        currLevel += 1;
        if(currLevel < 2){
            SetSubLevel();
        }
        else{
            SceneManager.SetActiveScene(nextMajorLevel);
        }
    }

}