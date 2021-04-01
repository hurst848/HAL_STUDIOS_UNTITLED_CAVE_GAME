using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testColliderThingy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.layer = LayerMask.NameToLayer("Ground");
        if (Physics.CheckBox(GetComponent<BoxCollider>().bounds.center, transform.GetComponent<BoxCollider>().size, transform.rotation, LayerMask.NameToLayer("roomGenDetection"), QueryTriggerInteraction.Collide))
        {
            Debug.Log("INTERSECTING");
            gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
        }
        gameObject.layer = LayerMask.NameToLayer("roomGenDetection");
    }
}