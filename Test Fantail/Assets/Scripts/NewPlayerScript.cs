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

    private Vector2 drag;
    private Vector2 lift;
    private Vector2 aeroForce;
    private float alpha;

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

    private float cursorAngle;
    public float zoomLimit = 4f;

    private bool bottomCollides;
    private bool inWater;

    private Vector2 plainVector = new Vector2(1, 0);

    public GameObject normalSpriteObject;
    private SpriteRenderer normalSpriteRenderer;

    private TrailRenderer trail;
    public Animator animator;

    //Variables fed into Animator Variables
    public bool fidgeting;
    public bool hopping;
    public bool gliding;

    private int wait = 0;

    private float deltaTime;

    // Use this for initialization
    void Start()
    {

        rigidbody = GetComponent<Rigidbody2D>();
        normalSpriteRenderer = normalSpriteObject.GetComponent<SpriteRenderer>();

        cursorHeadSpriteRenderer = cursorHead.GetComponent<SpriteRenderer>();
        cursorLineSpriteRenderer = cursorLine.GetComponent<SpriteRenderer>();

        trail = GetComponent<TrailRenderer>();

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
        VectorVisualization();


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

            wait ++;

            if (bottomCollides)
            {
                hopping = false;
            }
            if(wait >= 500)
            {
                fidgeting = true;
                wait = -200;
            }

            if(wait >= 0)
            {
                fidgeting = false;
            }

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

        }
        else if (!inWater)
        {
            appliedPenaltyMultiplier = 1;

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

        if (hasFlown && Input.GetMouseButton(1))
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
        animator.SetBool("dashed", Input.GetMouseButtonDown(0));
        animator.SetBool("inWater", inWater);

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

        processedBodyAngle = (normalSpriteObject.transform.eulerAngles.z);
        /*if(processedBodyAngle < 0)
        {
            processedBodyAngle += 360;
        }*/


        normalSpriteObject.transform.eulerAngles = new Vector3(0, 0, cursorAngle);

        bodyDirection = CursorPosition;

        rawAlpha = Vector2.SignedAngle(rigidbody.velocity, bodyDirection);
        processedAlpha = rawAlpha;

        if (processedBodyAngle > 90 && processedBodyAngle < 270)
        {
            normalSpriteRenderer.flipY = true;
            processedAlpha = -rawAlpha;
        } else
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

    void VectorVisualization()
    {
        //Completing missing info on visualVectorArray[0]
        {
            visualVectorArray[0].Direction = aeroForce / 3;

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
            visualVectorArray[2].Direction = drag / 3;

            visualVectorArray[2].Magnitude = visualVectorArray[2].Direction.magnitude;

            if (visualVectorArray[2].Magnitude > 0)
            {
                visualVectorArray[2].Angle = Vector2.SignedAngle(plainVector, visualVectorArray[2].Direction);
            }
        }

        //Completing missing info on visualVectorArray[3]
        {
            visualVectorArray[3].Direction = lift / 3;

            visualVectorArray[3].Magnitude = visualVectorArray[3].Direction.magnitude;

            if (visualVectorArray[3].Magnitude > 0)
            {
                visualVectorArray[3].Angle = Vector2.SignedAngle(plainVector, visualVectorArray[3].Direction);
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
    }

}
