using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItemScript : MonoBehaviour {

    public bool grabbed;
    RaycastHit2D hit;
    public float distance = 2f;
    public Transform holdpoint;
    public float throwforce;
    public LayerMask layerForRaycast;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("GrabItemScript is running");
        if (Input.GetButtonDown("Glide/Peck"))
        {

            if (!grabbed)
            {
                Debug.Log("!grabbed is being checked");

                Physics2D.queriesStartInColliders = false;

                hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance, layerForRaycast);

                if(hit.collider != null)
                {
                    Debug.Log("hit " + hit.collider.name);
                }

                if (hit.collider != null && hit.collider.tag == "Grabbable")
                {

                    Debug.Log("grabbed turning true");
                    grabbed = true;

                }


                //grab
            }
            else if (!Physics2D.OverlapPoint(holdpoint.position))
            {
                Debug.Log("else if statement being checked");

                grabbed = false;

                if (hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {

                    hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x, 1) * throwforce;
                }


                //throw
            }


        }

        if (grabbed)
        {
            hit.collider.gameObject.transform.position = holdpoint.position;
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * transform.localScale.x * distance);
    }
}
