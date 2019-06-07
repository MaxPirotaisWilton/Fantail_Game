using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSounds : MonoBehaviour {

    public AudioManager audioManager;
    public bool playChirp1;
    public bool playChirp2;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (playChirp1)
        {
            audioManager.Play("Chirp 1");
        }

        if(playChirp2)
        {
            audioManager.Play("Chirp 2");
        }

    }
}
