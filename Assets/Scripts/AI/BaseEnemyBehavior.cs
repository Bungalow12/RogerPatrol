using UnityEngine;
using System.Collections;

/// <summary>
/// Base enemy behavior.
/// </summary>
public class BaseEnemyBehavior : StateMachineBehaviour
{
    /// <summary>
    /// The enemy the State is attached to.
    /// </summary>
    protected Enemy self;

    /// <summary>
    /// Gets or sets the enemy the State is attached to.
    /// </summary>
    /// <value>The self.</value>
    public Enemy Self
    {
        get
        {
            return this.self;
        }
        set
        {
            this.self = value;
        }
    }

    /// <summary>
    /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state.
    /// In the case of the base class it sets up the enemy owner of the state.
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        this.self = animator.gameObject.GetComponent<Enemy>();
    }
}
