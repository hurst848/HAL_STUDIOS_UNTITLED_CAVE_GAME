using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sound_Attack_state : StateMachineBehaviour
{
    private GameObject monster;
    private bool targetType; // true = player, false = nearest sound source
    private bool attackSuccsesful;
    private bool attackFinished= false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("attack");
        monster.GetComponent<NavMeshAgent>().enabled = false;
        monster = animator.gameObject;
        // if there is no sound target, assume it is the player that is being attacked
        if (monster.GetComponent<soundMonsterController>().attackTarget == null)
        {
            targetType = true;
        }
        else
        {
            targetType = false;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackFinished)
        {
            if (attackSuccsesful)
            {
                if (targetType)
                {
                    // deduct health
                }
                else
                {
                    Destroy(monster.GetComponent<soundMonsterController>().attackTarget);
                }
              
            }
            animator.SetTrigger("locating");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monster.GetComponent<soundMonsterController>().attackTarget = null;
        monster.GetComponent<soundMonsterController>().isAttacking = false;
    }

   public void checkIfAttackIsSuccsesful()
   {
        if(targetType)
        {
            if (Vector3.Distance(monster.transform.position, GameObject.FindGameObjectWithTag("player").transform.position) <= 
                monster.GetComponent<soundMonsterController>().attackProximityThreshold)
            {
                attackSuccsesful = true;
            }
        }
        else
        {
            attackSuccsesful = true;
        }
   }

    public void animFinished()
    {
        attackFinished = true;
    }
}
