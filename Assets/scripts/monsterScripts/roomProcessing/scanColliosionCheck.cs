using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scanColliosionCheck : MonoBehaviour
{
    public bool obstacleDetected = false;

    private void OnCollisionEnter(Collision collision)
    {
        obstacleDetected = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        obstacleDetected = false;
    }
}
