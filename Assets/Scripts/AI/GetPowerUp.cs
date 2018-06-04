using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// The Demo Player default State.
/// </summary>
public class GetPowerUp : BasePlayerBehavior 
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
        // Do we need to find a new location to move towards?
        if (this.targetLocation == Vector3.zero ||
            Vector3.Distance(this.targetLocation, this.self.Position) < 2.0f)
        {
            ChooseLocation();
        }
        
        MoveToLocation();
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
        var powerUp = GameObject.FindGameObjectsWithTag("PowerUp")[0];
        //Fly to random onscreen point.
        this.targetLocation = new Vector3(powerUp.transform.position.x, 
                                          powerUp.transform.position.y, 
                                          powerUp.transform.position.z);
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
