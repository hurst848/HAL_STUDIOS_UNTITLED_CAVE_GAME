using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightDetector : MonoBehaviour
{
    public float range;
    public float fov;
    public int interval;
    public int layers;

    public bool _enabled = false;
    public bool _debugOn = false;


    private RaycastHit vison;
    private float fovInterval;
    

    void Start()
    {
        fovInterval = (fov) / layers;  
    }
    private void Update()
    {
        fovInterval = (fov) / layers;
        look();
    }

    public Vector3 look()
    {
        bool lightFound = false;
        Vector3 target = new Vector3();

        for (int i = 0; i < layers; i++)
        {
            for (int j = 0; j < 360; j += interval)
            {
                float x = Mathf.Sin(j);
                float y = Mathf.Cos(j);
                
                Vector3 _dir = transform.rotation * new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + range);
                _dir.Normalize();
                _dir = new Vector3(_dir.x * (fovInterval * (i + 1)), _dir.y * (fovInterval * (i + 1)), transform.position.z + range);

                if (_debugOn) { Debug.DrawLine(transform.position, _dir, Color.red); }
                if (Physics.Raycast(transform.position, _dir, out vison, range))
                {
                    if (vison.collider.gameObject.tag == "light")
                    {
                        Debug.Log("light");
                    }
                }
            }
            if (lightFound) { break; }
        }

        return target;
    }

    
    


}
