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
    private bool isTransitioning = false;

    public static LevelManager Instance { get; private set; }

    void Awake(){
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start(){
        player2D = GameObject.FindWithTag("2DPlayer").GetComponent<Player2D>();
        playerTD = GameObject.FindWithTag("TopDownPlayer").GetComponent<PlayerTopDown>();

        // Turn off all sublevels first
        for (int i = 0; i < subLevels.Length; i++)
        {
            if (subLevels[i].subLevelObjects != null)
            {
                subLevels[i].subLevelObjects.SetActive(false);
            }
        }

        SetSubLevel();
    }

    public void SetSubLevel()
    {
        Debug.Log("Loading sublevel index: " + currLevel + " Name: " + subLevels[currLevel].name);

        // Turn on current sublevel
        if (subLevels[currLevel].subLevelObjects != null){
            subLevels[currLevel].subLevelObjects.SetActive(true);
        }
        else{
            Debug.LogError("SubLevel Objects missing for: " + subLevels[currLevel].name);
        }

        if (subLevels[currLevel].spawnPoint2D == null){
            Debug.LogError("2D spawn point missing for: " + subLevels[currLevel].name);
        }

        if (subLevels[currLevel].spawnPointTD == null){
            Debug.LogError("TopDown spawn point missing for: " + subLevels[currLevel].name);
        }

        player2D.SetSpawn(subLevels[currLevel].spawnPoint2D);
        playerTD.SetSpawn(subLevels[currLevel].spawnPointTD);

        PlayerManager.Instance.Setup2D();
        ScoringManager.Instance.StartLevelTracking();
        PlayerManager.Instance.GroupRespawn();
    }

    public void CompleteCurrSublevel()
    {
        if (isTransitioning) return;

        SubLevel curr = subLevels[currLevel];
        if (curr.isCompleted) return;

        isTransitioning = true;
        curr.isCompleted = true;

        if (curr.subLevelObjects != null)
        {
            curr.subLevelObjects.SetActive(false);
        }

        currLevel += 1;
        ScoringManager.Instance.StopLevelTracking();

        if (currLevel <= subLevels.Length - 1)
        {
            SetSubLevel();
            Invoke(nameof(ResetTransition), 0.5f);
        }
        else
        {
            Destroy(UIScript.Instance.gameObject);
            SceneManager.LoadScene("Victory Screen");
        }
    }

    private void ResetTransition()
    {
        isTransitioning = false;
    }
}