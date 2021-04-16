using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sight_Idle_state : StateMachineBehaviour
{
    private GameObject monster;
    private float waitTime;
    private bool doneWaiting = false;
    private float currentTimeWaited = 0.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("entered IDLE state");
        animator.ResetTrigger("idle");
        monster = animator.gameObject;
        monster.GetComponent<NavMeshAgent>().enabled = false;
        waitTime = Random.Range(1.0f, 5.0f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!(monster.GetComponent<sightMonsterController>().target != null))
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
            animator.SetTrigger("attack");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
