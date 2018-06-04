using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the laser projectile.
/// </summary>
public class Laser : Projectile 
{	
    private Vector3 initialVelocity;
    /// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        base.HandleCollision(collision);
    }

    protected override void OnStart()
    {
        base.OnStart();
        this.initialVelocity = this.Velocity;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(this.Velocity != this.initialVelocity)
        {
            Kill();
        }
    }
}
