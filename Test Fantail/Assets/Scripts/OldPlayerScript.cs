using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPlayerScript : MonoBehaviour {

    //private int framecount;

    public CameraFollow cameraScript;
    public Camera camera;
    private bool hasFlown;
    public float inFlightSmooth = 0.5f;
    public float normalSmooth = 0.0125f;

    public GameObject cursorHead;
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

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        deltaTime = Time.deltaTime;

        Vector2 transformRight2 = new Vector2(transform.right.x, transform.right.y);




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


        if (cursorPosVector.y < transform.position.y)
        {
            cursorAngle *= -1;

        }

        cursorHead.transform.eulerAngles = new Vector3(0, 0, cursorAngle);


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

    void UpdateAlpha(float DeltaTime, Vector2 CursorPosition)
    {
        Vector2 bodyDirection;
        float rawAlpha;
        float processedAlpha;

        /*
        float bodyAngle = 999f;
        float velocityAngle = 0f;
        float alpha;

        if(Vector2.SqrMagnitude(rigidbody.velocity) > 0)
        { 
            Vector3 velocity3 = new Vector3
        }



        if (flightSpriteObject.transform.eulerAngles.z < 0)
        {
            bodyAngle = flightSpriteObject.transform.eulerAngles.z + 360;
        }
        else
        {
            bodyAngle = flightSpriteObject.transform.eulerAngles.z;
        }



        Debug.Log("bodyAngle = " + bodyAngle + " and velocityAngle = " + velocityAngle);
        */

        flightSpriteObject.transform.eulerAngles = new Vector3(0, 0, cursorAngle);

        bodyDirection = CursorPosition;

        rawAlpha = Vector2.SignedAngle(rigidbody.velocity, bodyDirection);

        Debug.Log("rawAlpha = " + rawAlpha);

    }

    void UpdateVelocity(float DeltaTime)
    {

    }

    void UpdatePosition(float DeltaTime)
    {

    }
}
