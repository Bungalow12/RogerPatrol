using UnityEngine;
using System.Collections;

/// <summary>
/// Child Object that checks for asteroids in the vicinity.
/// </summary>
public class TargetDetector : PhysicalObject 
{	
    private Projectile parent;

    private GameObject target = null;

    private bool targetIsEnemy = false;

    public void Start()
    {
        this.parent = this.transform.GetComponentInParent<Projectile>();
    }

    public void Update()
    {
        if (target != null)
        {
            this.parent.LookAt(target);
        }
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
            target = collider.gameObject;
            if(target != null && this.parent != null)
            {
                parent.LookAt(target);
            }
        }
        else if(target == null && !this.targetIsEnemy && collider.tag == "Asteroid")
        {
            target = collider.gameObject;
            if(target != null && this.parent != null)
            {
                parent.LookAt(target);
            }
        }
        else if(target == null && !this.targetIsEnemy && collider.tag == "Target")
        {
            target = collider.gameObject;
            if(target != null && this.parent != null)
            {
                parent.LookAt(target);
            }
        }
    }
}
