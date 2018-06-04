using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// The Enemy Seek State.
/// </summary>
public class Seek : BaseEnemyBehavior 
{
    /// <summary>
    /// The target location.
    /// </summary>
    private Vector3 targetLocation = Vector3.zero;

    /// <summary>
    /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state.
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // Choose an initial fly to location.
        ChooseLocation();
    }

    /// <summary>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        // if at anypoint you encounter a player select them as the target
        var colliders = (from player in Physics2D.OverlapCircleAll(self.Position, self.Detector.radius, LayerMask.GetMask("Player"))
                         where player.tag == "Player"
                         select player).ToArray();
        if (colliders.Length > 0)
        {
            self.Target = colliders[Random.Range(0, colliders.Length)].gameObject;

            //Move to attack state.
            animator.SetBool("PickedTarget", true);
        }
        else
        {
            // Do we need to find a new location to move towards?
            if (this.targetLocation == Vector3.zero ||
                Vector3.Distance(this.targetLocation, this.self.Position) < 2.0f)
            {
                ChooseLocation();
            }

            MoveToLocation();
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

    /// <summary>
    /// Chooses a random location within the world boundaries.
    /// </summary>
    private void ChooseLocation()
    {
        //Fly to random onscreen point.
        this.targetLocation = new Vector3(Random.Range(Globals.WorldBoundaries.xMin, Globals.WorldBoundaries.xMax),
                                          Random.Range(Globals.WorldBoundaries.yMin, Globals.WorldBoundaries.yMax),
                                          0.0f);
    }

    /// <summary>
    /// Flies towards most recently chosen location.
    /// </summary>
    private void MoveToLocation()
    {
        // Always look towards targetLocation
        this.self.LookAt(this.targetLocation);

        Vector3 targetDirection = (this.targetLocation - this.self.Position);
        targetDirection.Normalize();
        self.Body.AddForce(targetDirection * this.self.Speed, ForceMode2D.Impulse);
    }
}
