using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxBackground : MonoBehaviour
{

    public Vector3 initialPosition;
    public Transform playerTransform;
    public float lerpNum = 0.1f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 processedPlayerPosition = new Vector3(playerTransform.position.x, initialPosition.y, 20);

        Vector3 newPosition = Vector3.Lerp(initialPosition, processedPlayerPosition, lerpNum);

        transform.position = newPosition;
    }

        


}
