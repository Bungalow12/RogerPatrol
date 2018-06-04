using UnityEngine;
using System.Collections;

/// <summary>
/// The default attack state for a demo player.
/// </summary>
public class DemoPlayerAttack : BasePlayerBehavior 
{
    private float preferredDistance;
    private float maxDistance;

    /// <summary>
    /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    /// <summary>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {

        if (self.Target == null || 
            self.Target.transform == null)
        {
            self.Target = null;

            // Moves out of the attack state.
            animator.SetBool("PickedTarget", false);
            return;
        }
       
        //Look at the target constantly
        self.LookAt(self.Target);

		//Fire at firing rate.
        self.Shoot();
    }

    /// <summary>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        
    }
}
