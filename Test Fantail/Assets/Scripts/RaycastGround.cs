using UnityEngine;

public class RaycastGround : MonoBehaviour {

    public float distance = 1.0f; // distance to raycast downwards (i.e. between transform.position and bottom of object)
    public LayerMask hitMask; // which layers to raycast against

    void Update()
    {
        RaycastHit2D[] hits = new RaycastHit2D[2];
        int h = Physics2D.RaycastNonAlloc(transform.position, -Vector2.up, hits); //cast downwards
        if (h > 1)
        { //if we hit something do stuff
            Debug.Log(hits[1].normal);

            float angle = Mathf.Abs(Mathf.Atan2(hits[1].normal.x, hits[1].normal.y) * Mathf.Rad2Deg); //get angle
            Debug.Log(angle);

            if (angle > 30)
            {
                //DoSomething(); //change your animation
            }

        }
    }
}

