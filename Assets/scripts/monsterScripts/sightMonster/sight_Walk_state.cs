using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sight_Walk_state : StateMachineBehaviour
{
    private GameObject monster;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("walk");
        monster = animator.gameObject;
        monster.GetComponent<NavMeshAgent>().enabled = true;

        // get a random position to walk to
        GameObject[] walkinglocs = GameObject.FindGameObjectsWithTag("Ground");
        monster.GetComponent<NavMeshAgent>().SetDestination(walkinglocs[Random.Range(0, walkinglocs.Length - 1)].transform.position);

        // set walking speed
        monster.GetComponent<NavMeshAgent>().speed = monster.GetComponent<sightMonsterController>().walkSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (monster.GetComponent<sightMonsterController>().target != null)
        {
            animator.SetTrigger("run");
        }
        else
        {
            if (Vector3.Distance(monster.transform.position, monster.GetComponent<NavMeshAgent>().destination) <= 2f)
            {
                int dec = Random.Range(0, 9) % 2;
                if (dec == 0)
                {
                    animator.SetTrigger("locating");
                }
                else
                {
                    animator.SetTrigger("idle");
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
