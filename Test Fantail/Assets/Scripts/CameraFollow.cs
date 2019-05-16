using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform playerTransform;
    public NewPlayerScript playerScript;
    public float smoothSpeed = 0.0125f;

    private Camera camera;


    public Vector3 offset;

    // Use this for initialization
    void Start  ()  {

        camera = GetComponent<Camera>();

    }


    // Update is called once per frame
    void FixedUpdate () {
        if (playerScript != null)
        {
            Vector3 DesiredPosition = playerTransform.position + offset;
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, DesiredPosition, smoothSpeed);
            transform.position = SmoothedPosition;

            camera.orthographicSize = Mathf.Lerp(4f,10f,1f);
        }

    }
}
