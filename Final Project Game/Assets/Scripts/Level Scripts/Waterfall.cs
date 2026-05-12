using UnityEngine;
using System.Collections;

public class Waterfall : MonoBehaviour, IExecute{
    private AudioSource audioSrc;
    public AudioClip waterfallSound;
    void Awake(){
        audioSrc = GetComponent<AudioSource>();
    }

    public void Execute(){
        StartCoroutine(DisableWaterfall());
    }

    private IEnumerator DisableWaterfall(){
        audioSrc.clip = waterfallSound;
        audioSrc.Play();
        float startVolume = audioSrc.volume;
        while(audioSrc.volume > 0){
            audioSrc.volume -= startVolume * Time.deltaTime / waterfallSound.length;
            yield return null;
        }
        audioSrc.Stop();
        gameObject.SetActive(false);
    }
}    