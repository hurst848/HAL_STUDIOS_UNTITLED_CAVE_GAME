using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{

    public GameObject Item;

    public Transform pickUpPosition;
    public static int itemPickedUp;


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

            //if(this.GameObject.tag == "flashlight")
            {
                Debug.Log("its a flashlight!");
            }
        }
    } 
}
