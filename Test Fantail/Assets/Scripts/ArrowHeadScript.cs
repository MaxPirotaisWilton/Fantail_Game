using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHeadScript : MonoBehaviour {

    public NewPlayerScript player;
    public SpriteRenderer[] arrowSegments;
    private Color WHITE = new Color(255f,255f,255f);
    public Color transparentColour;
    private int switchState = 0;

	// Use this for initialization
	void Start () {
        arrowSegments = GetComponentsInChildren<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

        switchState = player.zoomNum;

            switch (player.zoomNum)
            {
                case 0:
                    arrowSegments[1].color = transparentColour;
                    arrowSegments[2].color = transparentColour;
                    arrowSegments[3].color = transparentColour;
                    break;

                case 1:
                    arrowSegments[1].color = transparentColour;
                    arrowSegments[2].color = transparentColour;
                    arrowSegments[3].color = WHITE;
                    break;

                case 2:
                    arrowSegments[1].color = transparentColour;
                    arrowSegments[2].color = WHITE;
                    arrowSegments[3].color = WHITE;
                    break;
                case 3:
                    arrowSegments[1].color = WHITE;
                    arrowSegments[2].color = WHITE;
                    arrowSegments[3].color = WHITE;
                    break;
            }
	}
}
