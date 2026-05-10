using UnityEngine;

public class Waterfall : MonoBehaviour, IExecute{

    public void Execute(){
        gameObject.SetActive(false);
    }
}    