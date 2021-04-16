using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class sight_Run_state : StateMachineBehaviour
{
    private GameObject monster;
    private Vector3 destination;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("run");
        monster = animator.gameObject;
        monster.GetComponent<NavMeshAgent>().enabled = true;

        // set the pathfinding to run towards the target 
        destination = monster.GetComponent<sightMonsterController>().target.transform.position;
        monster.GetComponent<NavMeshAgent>().SetDestination(destination);

        // set speed to run 
        monster.GetComponent<NavMeshAgent>().speed = monster.GetComponent<sightMonsterController>().runSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(monster.transform.position, monster.GetComponent<NavMeshAgent>().destination) <= 2f)
        {
            animator.SetTrigger("locating");
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
