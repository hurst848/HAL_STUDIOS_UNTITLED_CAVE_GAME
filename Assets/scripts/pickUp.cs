using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{

    public Transform pickUpPosition;

    void OnMouseOver()
    {
        //Debug.Log("Press E to pickup");
        if (Input.GetKeyDown("e"))
        {
            Debug.Log("Picked up item");
            GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = pickUpPosition.transform.position;
            this.transform.rotation = pickUpPosition.transform.rotation;
            this.transform.parent = GameObject.Find("pickUp").transform;
        }
    } 
}
