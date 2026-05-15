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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currLevel = 0;
        player2D = GameObject.FindWithTag("2DPlayer").GetComponent<Player2D>();
        playerTD = GameObject.FindWithTag("TopDownPlayer").GetComponent<PlayerTopDown>();

        for (int i = 0; i < subLevels.Length; i++)
        {
            subLevels[i].isCompleted = false; // ADD THIS
            if (subLevels[i].subLevelObjects != null)
                subLevels[i].subLevelObjects.SetActive(false);
        }

        SetSubLevel();
    }

    public void SetSubLevel()
    {
        Debug.Log($"[LM] ===== SetSubLevel START — index: {currLevel}, name: {subLevels[currLevel].name} =====");

        if (subLevels[currLevel].subLevelObjects != null)
            subLevels[currLevel].subLevelObjects.SetActive(true);
        else
            Debug.LogError($"[LM] SubLevel Objects missing for: {subLevels[currLevel].name}");

        Transform sp2D = subLevels[currLevel].spawnPoint2D;
        Transform spTD = subLevels[currLevel].spawnPointTD;

        if (sp2D == null)
            Debug.LogError($"[LM] ❌ 2D spawn point NULL for sublevel: {subLevels[currLevel].name}");
        else
            Debug.Log($"[LM] 2D spawn point position: {sp2D.position}");

        if (spTD == null)
            Debug.LogError($"[LM] ❌ TD spawn point NULL for sublevel: {subLevels[currLevel].name}");
        else
            Debug.Log($"[LM] TD spawn point position: {spTD.position}");

        player2D.SetSpawn(sp2D);
        playerTD.SetSpawn(spTD);
        Debug.Log($"[LM] SetSpawn called — player2D.SpawnPoint: {player2D.SpawnPoint?.position}");

        Rigidbody rb2D = player2D.GetComponent<Rigidbody>();
        Rigidbody rbTD = playerTD.GetComponent<Rigidbody>();

        Debug.Log($"[LM] Before restore — rb2D.isKinematic: {rb2D.isKinematic}, rbTD.isKinematic: {rbTD.isKinematic}");

        
        player2D.transform.rotation = Quaternion.identity;

        Debug.Log($"[LM] Physics restored — rb2D.isKinematic: {rb2D.isKinematic}");

        Debug.Log($"[LM] Calling Setup2D...");
        PlayerManager.Instance.Setup2D();

        Debug.Log($"[LM] Calling GroupRespawn — player2D pos BEFORE: {player2D.transform.position}");
        PlayerManager.Instance.GroupRespawn();
        Debug.Log($"[LM] GroupRespawn done — player2D pos AFTER: {player2D.transform.position}");

        if (sp2D != null)
        {
            float dist = Vector3.Distance(player2D.transform.position, sp2D.position);
            if (dist > 0.1f)
                Debug.LogWarning($"[LM] ⚠️ player2D is {dist:F3} units away from spawn point after GroupRespawn! Expected: {sp2D.position}, Got: {player2D.transform.position}");
            else
                Debug.Log($"[LM] ✅ player2D correctly at spawn point");
        }

        ScoringManager.Instance.StartLevelTracking();

        Debug.Log($"[LM] Enabling PlayerManager...");
        PlayerManager.Instance.enabled = true;

        Debug.Log($"[LM] ===== SetSubLevel END — player2D final pos: {player2D.transform.position} =====");
    }

    public void CompleteCurrSublevel()
    {
        Debug.Log($"[LM] CompleteCurrSublevel called — currLevel: {currLevel}, isTransitioning: {isTransitioning}");

        if (isTransitioning)
        {
            Debug.LogWarning("[LM] ⚠️ CompleteCurrSublevel blocked by isTransitioning!");
            return;
        }

        SubLevel curr = subLevels[currLevel];
        isTransitioning = true;
        curr.isCompleted = true;

        if (curr.subLevelObjects != null)
        {
            curr.subLevelObjects.SetActive(false);
            Debug.Log($"[LM] Deactivated sublevel objects: {curr.name}");
        }

        int prevLevel = currLevel;
        currLevel += 1;
        Debug.Log($"[LM] Advancing from level {prevLevel} → {currLevel}");

        ScoringManager.Instance.StopLevelTracking();

        if (currLevel <= subLevels.Length - 1)
        {
            SetSubLevel();
            Invoke(nameof(ResetTransition), 2f);
        }
        else
        {
            Debug.Log("[LM] All sublevels complete — loading Victory Screen");
            Destroy(UIScript.Instance.gameObject);
            SceneManager.LoadScene("Victory Screen");
        }
    }

    private void ResetTransition()
    {
        isTransitioning = false;
        Debug.Log("[LM] isTransitioning reset to false");
    }
}