using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform playerTransform;
    public NewPlayerScript playerScript;
    public float smoothSpeed = 0.0125f;

    public float minCamX, maxCamX, minCamY, maxCamY;

    public bool cameraIsClamped = true;

    public float minLimitX, maxLimitX, minLimitY, maxLimitY;

    private Camera camera;
    private Rigidbody2D rigidbody2D;

    private float lerpNum = 1f;


    public Vector3 offset;

    // Use this for initialization
    void Start  ()  {

        camera = GetComponent<Camera>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void FixedUpdate () {


        if (cameraIsClamped) 
        {
            minCamX = camera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            maxCamX = camera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            minCamY = camera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
            maxCamY = camera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        }

        float radiusX = (maxCamX - minCamX) / 2;
        float radiusY = (maxCamY - minCamY) / 2;

        //Debug.Log("minCamX = " + minCamX + ", maxCamX = " + maxCamX + ", minCamY = " + minCamY + ", maxCamY = " + maxCamY);

        if (playerScript != null)
        {
            Vector3 DesiredPosition = playerTransform.position + offset;
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, DesiredPosition, smoothSpeed);

            if (cameraIsClamped)
            {
                float clampedX = Mathf.Clamp(SmoothedPosition.x, minLimitX + radiusX, maxLimitX - radiusX);
                float clampedY = Mathf.Clamp(SmoothedPosition.y, minLimitY + radiusY, maxLimitY - radiusY);

                Vector3 ClampedPosition = new Vector3(clampedX, clampedY, SmoothedPosition.z);

                transform.position = ClampedPosition;
            }
            else
            {
                transform.position = SmoothedPosition;
            }


        }

    }

    void Update()
    {
        lerpNum /= rigidbody2D.velocity.magnitude;

        camera.orthographicSize = Mathf.Lerp(20f, 5f, 0.7f);
    }
}
