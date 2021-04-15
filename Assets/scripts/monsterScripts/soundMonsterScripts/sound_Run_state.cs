using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sound_Run_state : StateMachineBehaviour
{
    private GameObject monster;
    private Vector3 destination;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("entered RUN state");
        animator.ResetTrigger("run");
        monster = animator.gameObject;
        monster.GetComponent<NavMeshAgent>().enabled = true;

        // set the pathfinding to run towards the target 
        destination = monster.GetComponent<soundMonsterController>().target.transform.position;
        monster.GetComponent<NavMeshAgent>().SetDestination(destination);

        // set speed to run 
        monster.GetComponent<NavMeshAgent>().speed = monster.GetComponent<soundMonsterController>().runSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (monster.GetComponent<soundMonsterController>().target != null)
        {
            // if bellow the chase threshold, enter the locating state, else enter the running state
            animator.SetTrigger("locating");
            
        }
        if (Vector3.Distance(monster.transform.position,destination) <= 0.5f )
        {
            animator.SetTrigger("locating");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

   
}
