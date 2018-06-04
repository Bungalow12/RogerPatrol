using UnityEngine;
using System.Collections;

/// <summary>
/// Child Object that checks for asteroids in the vicinity.
/// </summary>
public class CheckForAsteroids : PhysicalObject 
{	
    /// <summary>
    /// The parent state machine
    /// </summary>
    private Animator parentAnimator;

    /// <summary>
    /// Gets the parent state machine.
    /// </summary>
    /// <value>The parent animator.</value>
    public Animator ParentAnimator
    {
        get
        {
            this.parentAnimator = this.parentAnimator ?? GetComponentInParent<Animator>();
            return this.parentAnimator;
        }
    }

    /// <summary>
    /// Method triggered when a collidable item enters the trigger space.
    /// </summary>
    /// <param name="collider">The colliding object.</param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Asteroid")
        {
            // Moves to the Dodge state.
            this.ParentAnimator.SetBool("InDanger", true);
        }
    }
}
