using System.Collections;
using UnityEngine;

public class SeamlessLoopMusicIntroLoop : MonoBehaviour
{
    public AudioSource audio1;
    public AudioSource audio2;

    private void Start() 
    {
        StartCoroutine(WaitForMusicStart());
    }

    IEnumerator WaitForMusicStart()
    {
        audio1.Play();
        yield return new WaitUntil(() => audio1.time >= audio1.clip.length);
        audio2.Play();
        Destroy(this);
    }
}
