using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItemScript : MonoBehaviour {

    private NewPlayerScript playerScript;
    public SpriteRenderer normalSprite;
    public SpriteRenderer withTwigSprite;

    public bool grabbed;
    RaycastHit2D hit;
    public float distance = 2f;
    public Transform holdpoint;
    public float throwforce;
    public LayerMask layerForRaycast;
    private int directionalNum = 1;
    private Vector3 holdPointPos;

    public int setAngle = 20;

    // Use this for initialization
    void Start()
    {

        playerScript = GetComponent<NewPlayerScript>();
        holdPointPos = holdpoint.position;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("GrabItemScript is running");

        //holdpoint.position = new Vector3(holdPointPos.x * directionalNum, holdPointPos.y, holdPointPos.z);

        if(normalSprite.flipX)
        {

            directionalNum = -1;

        }
        else
        {

            directionalNum = 1;

        }

        if (playerScript.pecking)
        {

            if (!grabbed)
            {
                //Debug.Log("!grabbed is being checked");

                Physics2D.queriesStartInColliders = false;

                hit = Physics2D.Raycast(transform.position, new Vector2(directionalNum, -0.5f), distance, layerForRaycast);

                if(hit.collider != null)
                {
                    Debug.Log("hit " + hit.collider.name);
                }

                if (hit.collider != null && hit.collider.tag == "Grabbable")
                {

                    //Debug.Log("grabbed turning true");
                    grabbed = true;

                }


                //grab
            }
            else if (!Physics2D.OverlapPoint(holdpoint.position))
            {
                //Debug.Log("else if statement being checked");

                grabbed = false;

                if (hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {

                    hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(directionalNum, 1) * throwforce;

                }


                //throw
            }


        }

        normalSprite.enabled = !grabbed;

        withTwigSprite.enabled = grabbed;

        if (grabbed)
        {
            hit.collider.gameObject.GetComponentInChildren<Collider2D>().enabled = false;

            hit.collider.gameObject.transform.position = new Vector3(((holdpoint.position.x - transform.position.x)* directionalNum) + transform.position.x, holdpoint.position.y, 1);

            hit.collider.gameObject.transform.eulerAngles = new Vector3(0,0,normalSprite.transform.eulerAngles.z + setAngle);

            hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            //Debug.Log(normalSprite.transform.eulerAngles.z);

        }else
        {
            if (hit.collider.gameObject.GetComponent<Collider2D>() != null)
            {
                hit.collider.gameObject.GetComponentInChildren<Collider2D>().enabled = true;

                hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }

        }



    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(directionalNum, -0.5f,0) * directionalNum * distance);
    }
}
