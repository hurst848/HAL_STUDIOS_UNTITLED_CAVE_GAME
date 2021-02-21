using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaLightEmmision : MonoBehaviour
{
    public float range;
    public bool lightOn = false;
    

    private Light _emmision;
    private SphereCollider _collider; 

    // Start is called before the first frame update
    void Start()
    {
        _emmision = GetComponent<Light>();
        _collider = GetComponent<SphereCollider>();
        _collider.radius = range;
        _emmision.range = range;
    }

    public void switchOn() { lightOn = true; _collider.enabled = true; }
    public void switchOff() { lightOn = false; _collider.enabled = false; }
}
