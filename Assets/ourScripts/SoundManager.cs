using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Mirror;

public class SoundManager : NetworkBehaviour
{
    // I feel so stupid with my reference problems now xD.
    public static SoundManager self;
    public AudioClip[] sounds;
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioSource src2;

    public bool onOff() {
        src.mute = ! src.mute;
        src2.mute = !src2.mute;
        return ! src.mute;
    }

    void Start()
    {
        self = this;
        src = this.gameObject.GetComponent(typeof(AudioSource)) as AudioSource;
    }
    
    public void playRandomAudio(int startIndex, int endIndex) {
        int index = Random.Range(startIndex, endIndex+1);
        src.PlayOneShot(sounds[index], 0.7F);
        //src.clip = sounds[index];
        //src.Play();
    }

    public void playAudio(int index) {
        src.PlayOneShot(sounds[index], 0.7F);
        //src.clip = sounds[index];
        //src.Play();
    }
}
