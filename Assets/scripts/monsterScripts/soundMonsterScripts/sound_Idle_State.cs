using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sound_Idle_State : StateMachineBehaviour
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
        soundMonsterController.attacktarget possTarget = monster.GetComponent<soundMonsterController>().fetchSound();


        if (possTarget.target == null)
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
            // if bellow the chase threshold, enter the locating state, else enter the run state
            if (possTarget.relativeSound >= monster.GetComponent<soundMonsterController>().chaseThreshold)
            {
                animator.SetTrigger("run");
            }
            else
            {
                animator.SetTrigger("locating");
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
