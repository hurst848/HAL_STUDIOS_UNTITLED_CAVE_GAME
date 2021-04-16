using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sightMonsterController : MonoBehaviour
{
    private enum State
    {
        Walking,
        Running,
        Searching,
        Idle,
        Attacking,
    }
    [Range(0.1f, 3.0f)]
    public float sensorPollRate = 3.0f;
    [Range(0.0f,360.0f)] public float fieldOfView = 30.0f;
    public float viewRadius = 5f;
    public float attackProximityThreshold = 0.5f;

    public LayerMask obstacleMask;
    public LayerMask targetMask;

    
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;

    private NavMeshAgent agent;
    private Animator animator;

    [HideInInspector] public GameObject target;

    [Range(0.0f, 1.0f)]
    public float monsterHealth = 1.0f;
    public bool monsterAlive = true;

    private State state = State.Idle;


    [HideInInspector] public Transform playerLoc;

    // Start is called before the first frame update
    void Start()
    {
        playerLoc = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, playerLoc.position) <= attackProximityThreshold)
        {
            animator.SetTrigger("attack");
        }
    }

    IEnumerator fetchLightSources()
    {
        List<GameObject> possibleTargets = new List<GameObject>();
        while (monsterAlive)
        {
            target = null;
            possibleTargets.Clear();
            Collider[] targetsWithinRange = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            for (int i =0; i < targetsWithinRange.Length; i++)
            {
                if (targetsWithinRange[i].tag == "light")
                {
                    Transform testTarget = targetsWithinRange[i].transform;
                    Vector3 directionToTest = (testTarget.position - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, directionToTest) < fieldOfView / 2)
                    {
                        float distanceToTest = Vector3.Distance(transform.position, testTarget.position);

                        if (!Physics.Raycast(transform.position, directionToTest, distanceToTest, targetMask))
                        {
                            possibleTargets.Add(targetsWithinRange[i].gameObject);
                        }
                    } 
                }
            }
            int closedIndex = 0;
            float closedDistance = viewRadius;
            for (int i = 0; i < possibleTargets.Count; i++)
            {
                if (Vector3.Distance(possibleTargets[0].transform.position, transform.position) <=closedDistance)
                {
                    closedIndex = i;
                    closedDistance = Vector3.Distance(possibleTargets[0].transform.position, transform.position);
                }
            }
            target = possibleTargets[closedIndex];

            yield return new WaitForSeconds(sensorPollRate);
        }

        yield return null;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
