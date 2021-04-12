using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sight_idle : StateMachineBehaviour
{

    private NavMeshAgent navigation;
    private GameObject agent;
    private GameObject player;

    private bool chooseNewLocation = false;
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
        if (chooseNewLocation == true)
        {
            GameObject[] possibleLoc = GameObject.FindGameObjectsWithTag("Ground");
            int roomChosen = Random.Range(0 , possibleLoc.Length  - 1);
            navigation.SetDestination(possibleLoc[roomChosen].transform.position);
            animator.SetTrigger("walk");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("walk");
    }

    IEnumerator waitSomeTime()
    {
        float timeToWait = Random.Range(1.0f, 5.0f);
        yield return new WaitForSeconds(timeToWait);
        chooseNewLocation = true;
        yield return null;
    }

}
