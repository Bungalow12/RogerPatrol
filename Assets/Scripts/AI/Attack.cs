using UnityEngine;
using System.Collections;

/// <summary>
/// The default attack state for an enemy.
/// </summary>
public class Attack : BaseEnemyBehavior 
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
        this.preferredDistance = animator.GetFloat("PreferredDistance");
        this.maxDistance = animator.GetFloat("MaxAttackDistance");

        if (self.Target == null           || 
            self.Target.transform == null ||
            Mathf.Abs(Vector3.Distance(self.Position, self.Target.transform.position)) > this.maxDistance)
        {
            self.Target = null;

            // Moves out of the attack state.
            animator.SetBool("PickedTarget", false);
            return;
        }

        //Keep a distance from the target
        float distance = Mathf.Abs(Vector2.Distance(self.Position, self.Target.transform.position));

        if ((distance - preferredDistance) > 2.0f)
        {
            //Move 
            Vector2 direction = self.Target.transform.position - self.Position;
            direction.Normalize();
            self.Body.AddForce(direction * self.Speed, ForceMode2D.Impulse);
        }
        else
        {
            self.Velocity = Vector3.zero;
        }

        //Look at the target constantly
        self.LookAt(self.Target);

        // If within the play area, shoot!
        if (!self.IsOutOfWorld)
        {
            //Fire at firing rate.
            self.Shoot();
        }
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
