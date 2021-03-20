using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterPathfinding : MonoBehaviour
{

    public Transform t;

    void Update()
    {
        GetComponent<NavMeshAgent>().SetDestination(t.position);
        
    }


}
