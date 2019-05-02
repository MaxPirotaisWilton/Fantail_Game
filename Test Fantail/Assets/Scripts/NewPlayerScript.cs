using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerScript : MonoBehaviour
{

    public Vector2 drag;
    public Vector2 lift;
    public Vector2 aeroForce;
    private float alpha;

    //private int framecount;

    public CameraFollow cameraScript;
    public Camera camera;

    public float inFlightSmooth = 0.5f;
    public float normalSmooth = 0.0125f;

    public GameObject cursor;
    public GameObject cursorHead;
    private SpriteRenderer cursorHeadSpriteRenderer;
    public GameObject cursorLine;
    private SpriteRenderer cursorLineSpriteRenderer;
    public bool cursorHasMoved;

    public float cursorPositionX = 0;
    public float cursorPositionY = 0;

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

    private float cursorAngle;
    public float cursorLimit = 4f;
    private bool hasFlown;
    public int zoomNumLimit = 3;
    public int zoomNum;
    public bool allowZoom;

    private bool bottomCollides;
    private bool inWater;

    private Vector2 plainVector = new Vector2(1, 0);

    public GameObject normalSpriteObject;
    private SpriteRenderer normalSpriteRenderer;

    private TrailRenderer trail;
    public Animator animator;

    //Variables related to inputs

    public float inputMouseX;
    public float inputMouseY;

    //Variables fed into Animator Variables
    public bool fidgeting;
    public bool hopping;
    public bool gliding;

    private int wait = 0;

    private float deltaTime;

    // Use this for initialization
    void Start()
    {

        zoomNum = zoomNumLimit;

        rigidbody = GetComponent<Rigidbody2D>();
        normalSpriteRenderer = normalSpriteObject.GetComponent<SpriteRenderer>();

        cursorHeadSpriteRenderer = cursorHead.GetComponent<SpriteRenderer>();
        cursorLineSpriteRenderer = cursorLine.GetComponent<SpriteRenderer>();

        trail = GetComponent<TrailRenderer>();



        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {

        //Variables related to inputs

        float inputHorizontal = Input.GetAxis("Horizontal");
        inputMouseX = Input.GetAxis("Mouse X");
        inputMouseY = Input.GetAxis("Mouse Y");

        deltaTime = Time.deltaTime;

        Vector2 transformRight2 = new Vector2(transform.right.x, transform.right.y);




        //deals with all instances of player moving horizontally
        if (walking)
        {
            wait = 0;
            fidgeting = false;

            //only hops/goes upwards if the player is touching the ground.
            if (bottomCollides)
            {
                hopping = false;
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, hopSpeed);
            }
            else
            {
                hopping = true;
            }

            //reset walking bool to false if true
            walking = false;

            //Flips the player sprite when going 
            if (rigidbody.velocity.x < 0)
            {
                normalSpriteRenderer.flipX = true;
            }
            else
            {
                normalSpriteRenderer.flipX = false;
            }
        }
        else
        {

            wait++;

            if (bottomCollides)
            {
                hopping = false;
            }
            if (wait >= 500)
            {
                fidgeting = true;
                wait = -200;
            }

            if (wait >= 0)
            {
                fidgeting = false;
            }

        }


        //Hoping Left
        if (inputHorizontal < 0)
        {
            if (rigidbody.velocity.x >= -(horizontalSpeed * appliedPenaltyMultiplier * -inputHorizontal))
            {
                rigidbody.velocity += -transformRight2;
            }

            if (!walking)
            {
                walking = true;
            }

        }


        //Hoping Right
        if (inputHorizontal > 0)
        {
            if (rigidbody.velocity.x <= (horizontalSpeed * appliedPenaltyMultiplier * inputHorizontal))
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


        if(bottomCollides || inWater)
        {

            zoomNum = zoomNumLimit;

        }


        //Decreases a multiplier for speed when in Water 
        if (inWater)
        {
            appliedPenaltyMultiplier = penaltyMultiplier;

        }
        else if (!inWater)
        {
            appliedPenaltyMultiplier = 1;

        }


        //Calculations for flight vector


        cursorPositionX += inputMouseX;
        cursorPositionY += inputMouseY;
        Vector2 zoomVector = new Vector2(cursorPositionX, cursorPositionY);

        //Limits the vector
        if (zoomVector.magnitude > cursorLimit)
        {
            zoomVector *= (cursorLimit / zoomVector.magnitude);
            cursorPositionX = zoomVector.x;
            cursorPositionY = zoomVector.y;
        }

        Vector2 cursorPosVector = zoomVector + rigidbody.position;

        //Calculations for cursor angle


        float dotProduct = Vector2.Dot(plainVector, zoomVector);
        float cosAngle = dotProduct / Mathf.Sqrt(Vector2.SqrMagnitude(zoomVector));

        cursorAngle = (Mathf.Acos(cosAngle)) * Mathf.Rad2Deg;


        if (Input.GetButtonDown("Dash") && zoomNum > 0)
        {

           
            allowZoom = true;
            Debug.Log("Dash is initiated");
            zoomNum--;


        }
        else 
        {

            allowZoom = false;
             
        }


        //makes the player zoom towards the in-game cursor when the player clicks the left mouse button
        if (allowZoom)
        {

            rigidbody.velocity = zoomVector * zoomMultiplier * appliedPenaltyMultiplier;

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



        /*
         * 
         * Switches from normal motion to gliding motion when left clicking during flight.
         *
         */

        if (hasFlown && Input.GetButton("Glide/Peck"))
        {
            gliding = true;
            normalSpriteRenderer.flipX = false;

            //starts trail when flying
            trail.time = 0.4f;
            trail.emitting = true;

            UpdateAlpha(deltaTime, zoomVector);
            UpdateVelocity(deltaTime);

        }
        else
        {
            gliding = false;
            normalSpriteObject.transform.eulerAngles = new Vector3(0, 0, 0);
            normalSpriteRenderer.flipY = false;

            //Decreases trail length until zero, when it is disabled
            if (trail.time >= 0)
            {
                trail.time -= 0.01f;
            }
            else
            {
                trail.emitting = false;
            }
        }

        animator.SetBool("fidget", fidgeting);
        animator.SetBool("hopping", hopping);
        animator.SetBool("gliding", gliding);
        animator.SetBool("onGround", bottomCollides);
        animator.SetBool("dashed", allowZoom);
        animator.SetBool("inWater", inWater);

    }

    void LateUpdate()
    {
        if (allowZoom && !hasFlown && !inWater)

        {
            cameraScript.smoothSpeed = inFlightSmooth;
            hasFlown = true;
            animator.Play("Dash", -1, 0f);
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

        processedBodyAngle = (normalSpriteObject.transform.eulerAngles.z);

        float glidingBodyAngle = cursorAngle;


        normalSpriteObject.transform.eulerAngles = new Vector3(0, 0, glidingBodyAngle);

        bodyDirection = CursorPosition;

        rawAlpha = Vector2.SignedAngle(rigidbody.velocity, bodyDirection);
        processedAlpha = rawAlpha;

        if (processedBodyAngle > 90 && processedBodyAngle < 270)
        {
            normalSpriteRenderer.flipY = true;
            processedAlpha = -rawAlpha;
        }
        else
        {
            normalSpriteRenderer.flipY = false;
        }

        //Debug.Log("processedBodyAngle = " + processedBodyAngle + " , rawAlpha = " + rawAlpha + " , processedAlpha = " + processedAlpha);

        alpha = rawAlpha;

    }

    void UpdateVelocity(float DeltaTime)
    {
        if (rigidbody.velocity.sqrMagnitude > 0)
        {
            float theta = Mathf.Deg2Rad * alpha;
            Vector2 v = rigidbody.velocity;

            drag = -v * (1 - Mathf.Cos(theta));
            lift = new Vector2(-(v.y), (v.x)) * Mathf.Sin(theta);

            aeroForce = drag + lift * 2;

            rigidbody.AddForce(aeroForce);

        }
    }


}
