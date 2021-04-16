using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class soundMonsterController : MonoBehaviour
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
    public float hearingRadius = 5f;
    public float attackProximityThreshold = 0.5f;
    public LayerMask targetMask;

    public float chaseThreshold = 1.0f;
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;


    [HideInInspector] public GameObject target;
    [HideInInspector] public float relVolumeOfTarget;

    [Range(0.0f, 1.0f)]
    public float monsterHealth = 1.0f;
    public bool monsterAlive = true;

    private State state = State.Idle;
    private bool wait = false;

    private NavMeshAgent agent;
    private Animator animator;

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


    IEnumerator fetchSound()
    {
        List<Collider> possibleTargets = new List<Collider>(); 
        while (monsterAlive)
        {
            // get all valid sound sources
            possibleTargets.Clear();
            target = null;
            Collider[] targetsWithinRange = Physics.OverlapSphere(transform.position, hearingRadius, targetMask);
            for (int i = 0; i < targetsWithinRange.Length; i++)
            {
                if (targetsWithinRange[i].tag == "sound")
                {
                    possibleTargets.Add(targetsWithinRange[i]);
                }
            }

            // evaluate all valid sound sources to deturmine the one to go for if there are any results
            if (possibleTargets.Count > 0)
            {
                int bestIndex = 0;
                float bestSound = 0.0f;
                for (int i = 0;  i < possibleTargets.Count; i++)
                {
                    float relSoundLevel = possibleTargets[i].GetComponent<audioOutputController>().monsterVolume / Vector3.Distance(transform.position, possibleTargets[i].transform.position);
                    if (relSoundLevel > bestSound)
                    {
                        bestSound = relSoundLevel;
                        bestIndex = i;
                    }
                }
                target = possibleTargets[bestIndex].gameObject;
                relVolumeOfTarget = bestSound;
            }
            else
            {
                target = null;
            }



            yield return new WaitForSeconds(3.0f);
        }

        yield return null;
    }

}
