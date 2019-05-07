using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorVisualizationScript : MonoBehaviour {

    public NewPlayerScript player;
    private Rigidbody2D rigidbody;

    public GameObject vectorVisual;

    [System.Serializable]
    public struct VectorVisualisation
    {
        public bool Instantiate;
        public string Name;
        public Vector2 Direction;
        public float Angle;
        public float Magnitude;
        public Color Colour;
    }

    public VectorVisualisation[] visualVectorArray;
    private GameObject[] visualVectorArrowsArray;

    private Vector2 plainVector = new Vector2(1, 0);

    // Use this for initialization
    void Start () {

        visualVectorArrowsArray = new GameObject[visualVectorArray.Length];
        rigidbody = GetComponent<Rigidbody2D>();

        for (int i = 0; i < visualVectorArray.Length; i++)
        {

            visualVectorArrowsArray[i] = Instantiate(vectorVisual, transform);


        }

    }
	
	// Update is called once per frame
	void Update () {

        //Completing missing info on visualVectorArray[0]
        {
            visualVectorArray[0].Direction = player.aeroForce / 3;

            visualVectorArray[0].Magnitude = visualVectorArray[0].Direction.magnitude;

            if (visualVectorArray[0].Magnitude > 0)
            {
                visualVectorArray[0].Angle = Vector2.SignedAngle(plainVector, visualVectorArray[0].Direction);
            }
        }

        //Completing missing info on visualVectorArray[1]
        {
            visualVectorArray[1].Direction = rigidbody.velocity / 3;

            visualVectorArray[1].Magnitude = visualVectorArray[1].Direction.magnitude;

            if (visualVectorArray[1].Magnitude > 0)
            {
                visualVectorArray[1].Angle = Vector2.SignedAngle(plainVector, visualVectorArray[1].Direction);
            }
        }

        //Completing missing info on visualVectorArray[2]
        {
            visualVectorArray[2].Direction = player.drag / 3;

            visualVectorArray[2].Magnitude = visualVectorArray[2].Direction.magnitude;

            if (visualVectorArray[2].Magnitude > 0)
            {
                visualVectorArray[2].Angle = Vector2.SignedAngle(plainVector, visualVectorArray[2].Direction);
            }
        }

        //Completing missing info on visualVectorArray[3]
        {
            visualVectorArray[3].Direction = player.lift / 3;

            visualVectorArray[3].Magnitude = visualVectorArray[3].Direction.magnitude;

            if (visualVectorArray[3].Magnitude > 0)
            {
                visualVectorArray[3].Angle = Vector2.SignedAngle(plainVector, visualVectorArray[3].Direction);
            }
        }

        //Completing missing info on visualVectorArray[4]
        {
            visualVectorArray[4].Direction = new Vector2(player.inputMouseX, player.inputMouseY);

            visualVectorArray[4].Magnitude = visualVectorArray[4].Direction.magnitude;

            if (visualVectorArray[4].Magnitude > 0)
            {
                visualVectorArray[4].Angle = Vector2.SignedAngle(plainVector, visualVectorArray[4].Direction);
            }
        }

        //Completing missing info on visualVectorArray[5]
        {
            visualVectorArray[5].Direction = new Vector2(player.inputJoystickX, player.inputJoystickY);

            visualVectorArray[5].Magnitude = visualVectorArray[5].Direction.magnitude;

            if (visualVectorArray[5].Magnitude > 0)
            {
                visualVectorArray[5].Angle = Vector2.SignedAngle(plainVector, visualVectorArray[5].Direction);
            }
        }

        //updates vector visualisation arrows
        for (int i = 0; i < visualVectorArray.Length; i++)
        {
            Vector2 originalVector = visualVectorArray[i].Direction;
            Vector2 processedVector = originalVector + rigidbody.position;
            float cursorRotation = visualVectorArray[i].Angle;
            float visualMagnitude = visualVectorArray[i].Magnitude;
            Color visualColour = visualVectorArray[i].Colour;
            Transform[] arrowTransformsArray;
            SpriteRenderer[] arrowSpriteRenderersArray;

            arrowSpriteRenderersArray = visualVectorArrowsArray[i].GetComponentsInChildren<SpriteRenderer>();
            arrowTransformsArray = visualVectorArrowsArray[i].GetComponentsInChildren<Transform>();

            for (int j = 0; j < arrowSpriteRenderersArray.Length; j++)
            {
                arrowSpriteRenderersArray[j].color = visualVectorArray[i].Colour;
                arrowSpriteRenderersArray[j].enabled = visualVectorArray[i].Instantiate;

            }

            arrowTransformsArray[2].position = processedVector;
            arrowTransformsArray[2].eulerAngles = new Vector3(0, 0, cursorRotation);

            arrowTransformsArray[1].position = originalVector / 2 + rigidbody.position;
            arrowTransformsArray[1].eulerAngles = new Vector3(0, 0, cursorRotation);
            arrowTransformsArray[1].transform.localScale = new Vector3(visualMagnitude / 2.5f, 1, 1);

            Debug.Log("Components found in element " + i + ": # of SpriteRenderers = " + arrowSpriteRenderersArray.Length + " and # of Transforms = " + arrowTransformsArray.Length);


        }

    }


}
