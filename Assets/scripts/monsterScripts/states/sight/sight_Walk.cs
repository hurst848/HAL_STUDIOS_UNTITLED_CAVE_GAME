using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sight_Walk : StateMachineBehaviour
{
    private NavMeshAgent navigation;
    private GameObject agent;
    private GameObject player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = animator.gameObject.transform.parent.gameObject;
        navigation = agent.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // no logic for moving here as that is dealt with the nav mesh agent destination
        if (agent.transform.position  == navigation.destination)
        {
            int choise = Random.Range(0, 1);
            if (choise == 0)
            {
                animator.SetTrigger("idle");
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
        animator.ResetTrigger("idle");
        animator.ResetTrigger("locating");
    }

  
}
