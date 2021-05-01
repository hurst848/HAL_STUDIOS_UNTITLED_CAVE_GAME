using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sound_Walk_state : StateMachineBehaviour
{
    private GameObject monster;

    private Vector3 prevPos;
    private int breakState = 0;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("entered WALK state");
        animator.ResetTrigger("walk");
        monster = animator.gameObject;
        monster.GetComponent<NavMeshAgent>().enabled = true;

        // get a random position to walk to
        GameObject[] walkinglocs = GameObject.FindGameObjectsWithTag("Ground");
        monster.GetComponent<NavMeshAgent>().SetDestination(walkinglocs[Random.Range(0, walkinglocs.Length  - 1)].transform.position);

        // set walking speed
        monster.GetComponent<NavMeshAgent>().speed = monster.GetComponent<soundMonsterController>().walkSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        soundMonsterController.attacktarget possTarget = monster.GetComponent<soundMonsterController>().fetchSound();


        // if the monster detected the sound, check which state to switch to
        if (possTarget.target != null)
        {
            // if bellow the chase threshold, enter the locating state, else enter the running state
            if (possTarget.relativeSound >= monster.GetComponent<soundMonsterController>().chaseThreshold)
            {
                animator.SetTrigger("run");
            }
            else if (possTarget.relativeSound >= monster.GetComponent<soundMonsterController>().listenThreshold)
            {
                animator.SetTrigger("locating");
            }
        }
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

        if (Vector3.Distance(prevPos, monster.transform.position) <= 0.5f) { breakState++; }
        else { breakState = 0; }

        if (breakState > 1000)
        {
            animator.SetTrigger("idle");
        }


    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

   
}
