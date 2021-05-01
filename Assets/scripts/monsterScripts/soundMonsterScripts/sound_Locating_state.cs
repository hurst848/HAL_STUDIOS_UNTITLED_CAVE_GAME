using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sound_Locating_state : StateMachineBehaviour
{
    public float listeningRadiusMultiplier = 2.0f;

    private GameObject monster;
    private float waitTime;
    private bool doneWaiting = false;
    private float currentTimeWaited = 0.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("entered LOCATING state");
        animator.ResetTrigger("locating");
        monster = animator.gameObject;
        monster.GetComponent<NavMeshAgent>().enabled = false;
        waitTime = Random.Range(2.0f, 5.0f);

        // increase the listening radius lower the chase threshold
        monster.GetComponent<soundMonsterController>().hearingRadius *= listeningRadiusMultiplier;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!(monster.GetComponent<soundMonsterController>().target != null))
        {
            // wait for the 'timer' to end
            if (!doneWaiting)
            {
                currentTimeWaited += Time.deltaTime;
                if (currentTimeWaited >= waitTime)
                {
                    doneWaiting = true;
                }
            }
            // when the 'timer' ends, enter walking state 
            else
            {
                animator.SetTrigger("walk");
            }
        }
        else
        {
            // enter the run state
            if (monster.GetComponent<soundMonsterController>().relVolumeOfTarget >= monster.GetComponent<soundMonsterController>().chaseThreshold)
            {
                animator.SetTrigger("run");
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monster.GetComponent<soundMonsterController>().hearingRadius /= listeningRadiusMultiplier;
        monster.GetComponent<NavMeshAgent>().enabled = true;
    }

}
