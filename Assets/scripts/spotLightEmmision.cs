using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spotLightEmmision : MonoBehaviour
{
    public float range;
    public float spotAngle;
    public bool lightOn = false;
    public GameObject colliders;
    private SphereCollider _endCollider;
    private Light _light;
    private float _radius;
    
    void Start()
    {
        _endCollider = colliders.GetComponent<SphereCollider>();
        _light = GetComponent<Light>();

        _light.range = range;
        _light.spotAngle = spotAngle;

        calcualteRadius();
        colliders.transform.position = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y,
            transform.localPosition.z + range);   
    }

    public void switchOn()  { lightOn = true;  }
    public void switchOff() { lightOn = false; }

    private void calcualteRadius()
    { 
        _endCollider.radius = (Mathf.Atan(Mathf.Deg2Rad * _light.spotAngle) * range)/1.75f;
    }
}
