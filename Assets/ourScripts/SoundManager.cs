using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // I feel so stupid with my reference problems now xD.
    public static SoundManager self;
    public AudioClip[] sounds;
    private AudioSource src;
    public bool audioOn;

    void Start()
    {
        self = this;
        src = this.gameObject.GetComponent(typeof(AudioSource)) as AudioSource;
    }
    
    public void playRandomAudio(int startIndex, int endIndex) {
        int index = Random.Range(startIndex, endIndex+1);
        src.clip = sounds[index];
        src.Play();
    }

    public void playAudio(int index) {
        src.clip = sounds[index];
        src.Play();
    }
}
