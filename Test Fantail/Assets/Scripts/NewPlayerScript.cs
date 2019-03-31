using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerScript : MonoBehaviour
{
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

    private Vector2 lift;

    //private int framecount;

    public CameraFollow cameraScript;
    public Camera camera;
    private bool hasFlown;
    public float inFlightSmooth = 0.5f;
    public float normalSmooth = 0.0125f;

    public GameObject cursor;
    public GameObject cursorHead;
    private SpriteRenderer cursorHeadSpriteRenderer;
    public GameObject cursorLine;
    private SpriteRenderer cursorLineSpriteRenderer;
    public bool cursorHasMoved;


    private Rigidbody2D rigidbody;
    public bool walking = false;
    public LayerMask groundLayer;
    public LayerMask waterLayer;
    public Transform bottomCheck;
    public Transform centerCheck;
    public float groundCheckRadius;
    public float centerCheckRadius;
    public float horizontalSpeed = 1.5f;
    public float hopSpeed = 1f;
    public float zoomMultiplier;

    private float appliedPenaltyMultiplier = 1f;
    public float penaltyMultiplier = 0.5f;
    private float additionalPenalty;

    private float cursorAngle;
    public float zoomLimit = 4f;

    private bool bottomCollides;
    private bool inWater;

    private Vector2 plainVector = new Vector2(1, 0);

    public GameObject flightSpriteObject;
    public GameObject normalSpriteObject;
    private SpriteRenderer flightSpriteRenderer;
    private SpriteRenderer normalSpriteRenderer;

    private float deltaTime;

    // Use this for initialization
    void Start()
    {

        rigidbody = GetComponent<Rigidbody2D>();
        flightSpriteRenderer = flightSpriteObject.GetComponent<SpriteRenderer>();
        normalSpriteRenderer = normalSpriteObject.GetComponent<SpriteRenderer>();

        cursorHeadSpriteRenderer = cursorHead.GetComponent<SpriteRenderer>();
        cursorLineSpriteRenderer = cursorLine.GetComponent<SpriteRenderer>();

        visualVectorArrowsArray = new GameObject[visualVectorArray.Length];

        for (int i = 0; i < visualVectorArray.Length; i++)
        {

            visualVectorArrowsArray[i] = Instantiate(cursor, transform);


        }

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        deltaTime = Time.deltaTime;

        Vector2 transformRight2 = new Vector2(transform.right.x, transform.right.y);

        //Completing missing info on visualVectorArray[0]
        { 
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

            //Debug.Log("Components found in element " + i +": # of SpriteRenderers = " + arrowSpriteRenderersArray.Length + " and # of Transforms = " + arrowTransformsArray.Length);


        }




        //deals with all instances of player moving horizontally
        if (walking)
        {

            //only changes hops/goes upwards if the player is touching the ground.
            if (bottomCollides)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, hopSpeed);
            }

            //reset walking bool to false if true
            walking = false;
        }


        //Hoping Left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (rigidbody.velocity.x >= -(horizontalSpeed * appliedPenaltyMultiplier))
            {
                rigidbody.velocity += -transformRight2;
            }

            if (!walking)
            {
                walking = true;
            }

        }


        //Hoping Right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (rigidbody.velocity.x <= (horizontalSpeed * appliedPenaltyMultiplier))
            {
                rigidbody.velocity += transformRight2;
            }
            if (!walking)
            {
                walking = true;
            }

        }





        //Handles detection of collision(s) with bool(s)
        bottomCollides = Physics2D.OverlapCircle(bottomCheck.position, groundCheckRadius, groundLayer);
        inWater = Physics2D.OverlapCircle(bottomCheck.position, groundCheckRadius, waterLayer);


        //Decreases a multiplier for speed when in Water 

        if (inWater)
        {
            appliedPenaltyMultiplier = penaltyMultiplier;
            additionalPenalty = 0.1f;
        }
        else if (!inWater)
        {
            appliedPenaltyMultiplier = 1;
            additionalPenalty = 1f;
        }


        //Calculations for flight vector

        Vector2 mousePosPlus = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);
        Vector2 zoomVector = mousePosPlus / 75;

        //Limits the vector
        if (zoomVector.magnitude > zoomLimit)
        {
            zoomVector *= (zoomLimit / zoomVector.magnitude);
        }

        Vector2 cursorPosVector = zoomVector + rigidbody.position;

        //Calculations for cursor angle


        float dotProduct = Vector2.Dot(plainVector, zoomVector);
        float cosAngle = dotProduct / Mathf.Sqrt(Vector2.SqrMagnitude(zoomVector));

        cursorAngle = (Mathf.Acos(cosAngle)) * Mathf.Rad2Deg;



        //makes the player zoom towards the in-game cursor when the player clicks the left mouse button
        if (Input.GetMouseButtonDown(0))
        {

            rigidbody.velocity = zoomVector * zoomMultiplier * appliedPenaltyMultiplier * additionalPenalty;

        }




        //resets camera smoothness when back on the ground after flying
        if ((hasFlown && bottomCollides) || inWater)
        {
            cameraScript.smoothSpeed = normalSmooth;
            hasFlown = false;
        }


        //assigning transforms to in-game cursor
        Vector3 intermediateVector = new Vector3(cursorPosVector.x, cursorPosVector.y, cursorHead.transform.position.z);
        cursorHead.transform.position = intermediateVector;
        cursorLine.transform.position = (new Vector3((zoomVector.x / 2) + transform.position.x, (zoomVector.y / 2) + transform.position.y, cursorLine.transform.position.z));
        cursorLine.transform.localScale = new Vector3(zoomVector.magnitude / 2.5f, 1, 1);


        if (cursorPosVector.y < transform.position.y)
        {
            cursorAngle *= -1;

        }

        cursorHead.transform.eulerAngles = new Vector3(0, 0, cursorAngle);
        cursorLine.transform.eulerAngles = new Vector3(0, 0, cursorAngle);

        //visualVectorArray[0].Direction = zoomVector;



        /*
         * 
         * Switches from normal motion to gliding motion when left clicking during flight.
         *
         */

        if (hasFlown && Input.GetMouseButton(1))
        {
            //rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

            flightSpriteRenderer.enabled = true;
            normalSpriteRenderer.enabled = false;



            UpdateAlpha(deltaTime, zoomVector);


        }
        else
        {
            //rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            flightSpriteRenderer.enabled = false;
            normalSpriteRenderer.enabled = true;

        }

    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !hasFlown && !inWater)

        {
            cameraScript.smoothSpeed = inFlightSmooth;
            hasFlown = true;
        }
    }

    /*When gliding, updates the bird's angle of attack (Alpha)

    @param DeltaTime    Change in time
    @param CursorPosition   Position of cursor as a 2D Vector (Vector2)

    */

    void UpdateAlpha(float DeltaTime, Vector2 CursorPosition)
    {
        Vector2 bodyDirection;
        float processedBodyAngle;
        float rawAlpha;
        float processedAlpha;

        processedBodyAngle = (flightSpriteObject.transform.rotation.z * 180);
        /*if(processedBodyAngle < 0)
        {
            processedBodyAngle += 360;
        }*/


        flightSpriteObject.transform.eulerAngles = new Vector3(0, 0, cursorAngle);

        bodyDirection = CursorPosition;

        rawAlpha = Vector2.SignedAngle(rigidbody.velocity, bodyDirection);
        processedAlpha = rawAlpha;

        if (processedBodyAngle > 90 || processedBodyAngle < -90)
        {
            processedAlpha = -rawAlpha;
        }

        Debug.Log("processedBodyAngle = " + processedBodyAngle + " , rawAlpha = " + rawAlpha + " , processedAlpha = " + processedAlpha);


    }

    void UpdateVelocity(float DeltaTime)
    {

    }

    void UpdatePosition(float DeltaTime)
    {

    }

}
