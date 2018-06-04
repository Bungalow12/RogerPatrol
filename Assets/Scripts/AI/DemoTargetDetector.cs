using UnityEngine;
using System.Collections;

/// <summary>
/// Child Object that checks for asteroids in the vicinity.
/// </summary>
public class DemoTargetDetector : PhysicalObject 
{	
    private DemoPlayer parent;
    
    private GameObject target = null;

    private bool targetIsEnemy = false;

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

    public void Start()
    {
        this.parent = this.transform.GetComponentInParent<DemoPlayer>();
    }

    /// <summary>
    /// Method triggered when a collidable item enters the trigger space.
    /// </summary>
    /// <param name="collider">The colliding object.</param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        //Let's try first come
        if((target == null || !this.targetIsEnemy) && collider.tag == "Enemy")
        {
            this.targetIsEnemy = true;
            this.parent.Target = collider.gameObject;

            //Move to attack state.
            this.ParentAnimator.SetBool("PickedTarget", true);
        }
        else if(target == null && !this.targetIsEnemy && collider.tag == "Asteroid")
        {
            this.parent.Target = collider.gameObject;

            //Move to attack state.
            this.ParentAnimator.SetBool("PickedTarget", true);
        }
    }
}
