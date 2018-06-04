using System;
using UnityEngine;

public class ActiveObject : PhysicalObject
{
    /// <summary>
    /// The speed modifier for rotation.
    /// </summary>
    [SerializeField]
    protected ResponsiveFloat rotationSpeed = new ResponsiveFloat(2.0f, 0.0f, ModificationStyle.ADDITIVE);

    protected float RotationSpeed
    {
        get
        {
            return this.rotationSpeed.Value;
        }
        set
        {
            this.rotationSpeed.Value = value;
        }
    }

    /// <summary>
    /// Handles the collision.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    protected virtual void HandleCollision(Collision2D collision)
    {
        //Do Nothing
    }
    
    /// <summary>
    /// Handles the collision with triggers.
    /// </summary>
    /// <param name="collider">The detected trigger collision.</param>
    protected virtual void HandleTriggers(Collider2D collider)
    {
        //Do Nothing
    }

    protected virtual void OnStart()
    {
        //Do nothing
    }

    protected virtual void OnUpdate()
    {
        //Handle LookAt
        if(this.isTurning)
        {
            this.transform.rotation = Quaternion.Slerp(this.startingRotation, this.targetRotation, Time.deltaTime * this.RotationSpeed);
            if(this.transform.rotation == this.targetRotation)
            {
                this.isTurning = false;
            }
        }
    }
}

