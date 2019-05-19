using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableScript : MonoBehaviour {

    public GameObject player;
    public Transform grabTransform;
    private GrabItemScript playerScript;
    private Transform beakTransform;

    public bool grabbed;

	// Use this for initialization
	void Start () {

        playerScript = player.GetComponent<GrabItemScript>();


	}
	
	// Update is called once per frame
	void Update () {

        //beakTransform = playerScript.beakTransform;


        if(grabbed)
        {

        }
	}
}
