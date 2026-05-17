using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class SubLevel
    {
        public string name;
        public GameObject subLevelObjects;
        public Transform spawnPoint2D;
        public Transform spawnPointTD;
        public bool isCompleted;
    }

    public SubLevel[] subLevels;
    private int currLevel = 0;
    private Player2D player2D;
    private PlayerTopDown playerTD;
    public bool isTransitioning = false;


    public static LevelManager Instance { get; private set; }

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start(){
        currLevel = 0;
        player2D = GameObject.FindWithTag("2DPlayer").GetComponent<Player2D>();
        playerTD = GameObject.FindWithTag("TopDownPlayer").GetComponent<PlayerTopDown>();

        for (int i = 0; i < subLevels.Length; i++){
            subLevels[i].isCompleted = false;
            if(subLevels[i].subLevelObjects != null){
                subLevels[i].subLevelObjects.SetActive(false);
            }
        }
        SetSubLevel();
    }

    public void SetSubLevel(){

        if (subLevels[currLevel].subLevelObjects != null){
            subLevels[currLevel].subLevelObjects.SetActive(true);
        }

        Transform sp2D = subLevels[currLevel].spawnPoint2D;
        Transform spTD = subLevels[currLevel].spawnPointTD;

        player2D.SetSpawn(sp2D);
        playerTD.SetSpawn(spTD);
        Rigidbody rb2D = player2D.GetComponent<Rigidbody>();
        Rigidbody rbTD = playerTD.GetComponent<Rigidbody>();
        
        player2D.transform.rotation = Quaternion.identity;
        PlayerManager.Instance.Setup2D();
        PlayerManager.Instance.GroupRespawn();

        ScoringManager.Instance.StartLevelTracking();

        PlayerManager.Instance.enabled = true;

    }

    public void CompleteCurrSublevel(){
        if (isTransitioning) return;

        SubLevel curr = subLevels[currLevel];
        isTransitioning = true;
        curr.isCompleted = true;

        if (curr.subLevelObjects != null){
            curr.subLevelObjects.SetActive(false);
        }

        int prevLevel = currLevel;
        currLevel += 1;

        ScoringManager.Instance.StopLevelTracking();

        if (currLevel <= subLevels.Length - 1)
        {
            SetSubLevel();
            Invoke(nameof(ResetTransition), 2f);
        }
        else{
            Destroy(UIScript.Instance.gameObject);
            SceneManager.LoadScene("Victory Screen");
        }
    }
}