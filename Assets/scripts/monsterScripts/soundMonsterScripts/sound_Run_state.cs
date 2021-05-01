using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sound_Run_state : StateMachineBehaviour
{
    private GameObject monster;
    private Vector3 destination;

    private Vector3 prevPos;
    private int breakState = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("entered RUN state");
        animator.ResetTrigger("run");
        monster = animator.gameObject;
        monster.GetComponent<NavMeshAgent>().enabled = true;

        // set the pathfinding to run towards the target 
        if (monster.GetComponent<soundMonsterController>().target == null)
        {
            GameObject tmp =  monster.GetComponent<soundMonsterController>().fetchSound().target;
            if (tmp == null)
            {
                animator.SetTrigger("locating");
            }
            else
            {
                destination = tmp.transform.position;
            }
        }
        else
        {
            destination = monster.GetComponent<soundMonsterController>().target.transform.position;
        }
        
        monster.GetComponent<NavMeshAgent>().SetDestination(destination);

        // set speed to run 
        monster.GetComponent<NavMeshAgent>().speed = monster.GetComponent<soundMonsterController>().runSpeed;
        prevPos = monster.transform.position;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        soundMonsterController.attacktarget possTarget = monster.GetComponent<soundMonsterController>().fetchSound();

        if (possTarget.target != null && (possTarget.relativeSound >= monster.GetComponent<soundMonsterController>().chaseThreshold))
        {
            // if bellow the chase threshold, enter the locating state, else enter the running state
            animator.SetTrigger("locating");
            
        }
        if (Vector3.Distance(monster.transform.position,destination) <= 0.5f )
        {
            animator.SetTrigger("locating");
        }
        if (Vector3.Distance(prevPos, monster.transform.position) <= 0.5f){ breakState++; }
        else { breakState = 0; }

        if (breakState > 1000)
        {
            animator.SetTrigger("walk");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

   
}
