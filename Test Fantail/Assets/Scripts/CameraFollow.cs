using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform playerTransform;
    public NewPlayerScript playerScript;
    public OldPlayerScript playerScriptAlternative;
    public float smoothSpeed = 0.0125f;



    public Vector3 offset;

    // Update is called once per frame
    void LateUpdate () {
        if (playerScript != null)
        {
            Vector3 DesiredPosition = playerTransform.position + offset;
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, DesiredPosition, smoothSpeed);
            transform.position = SmoothedPosition;
        }

        if(playerScriptAlternative != null)
        {
            Vector3 DesiredPosition = playerTransform.position + offset;
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, DesiredPosition, smoothSpeed);
            transform.position = SmoothedPosition;
        }

    }
}
