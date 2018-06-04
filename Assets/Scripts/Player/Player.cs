using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets._2D;

/// <summary>
/// The local player object.
/// </summary>
public class Player : BasePlayer 
{	

    /// <summary>
    /// Prevent multiple halfturn event overlap.
    /// </summary>
    [SerializeField]
    private float bounceDelay = 0.25f;

    private float bounceTime = 0.0f;
    
    protected override void OnStart()
    {
        this.playStatistics.userClickedSignup = PlayerPrefs.GetInt("ClickedCreateAccount", 0) == 1;
        GameObject.Find("Main Camera").GetComponent<CameraFollow>().SetPlayer(this.transform);
        base.OnStart();
    }

    protected void HandleRotation(float rotationSpeedModifier = 1.0f)
    {
        // Handle rotation.
        var rotation = CrossPlatformInputManager.GetAxis("Rotate");
        this.Body.rotation -= ((this.RotationSpeed * rotationSpeedModifier) * rotation);
    }

    protected void HandleUTurn()
    {
        // Handle hard 180.
        if (CrossPlatformInputManager.GetButtonDown("HalfTurn"))
        {
            if ((Time.time - this.bounceTime) >= this.bounceDelay)
            {
                ++this.playStatistics.uturnsMade;
                this.bounceTime = Time.time;
                var turnDirection = CrossPlatformInputManager.GetAxis("HalfTurn") > 0.0f ? 1.0f : -1.0f;
                this.Body.MoveRotation(this.Body.rotation + (180.0f * turnDirection));
            }
        }
    }

    protected virtual void HandleBoundaryBounce()
    {
        if (this.Position.x < Globals.WorldBoundaries.xMin + this.HalfWidth ||
            this.Position.x > Globals.WorldBoundaries.xMax - this.HalfWidth)
        {
            var reflectedVelocity = new Vector2(this.Velocity.x * -1.0f, this.Velocity.y);
            this.Velocity = reflectedVelocity;
        }

        if (this.Position.y < Globals.WorldBoundaries.yMin + this.HalfWidth ||
            this.Position.y > Globals.WorldBoundaries.yMax - this.HalfWidth)
        {
            var reflectedVelocity = new Vector2(this.Velocity.x, this.Velocity.y * -1.0f);
            this.Velocity = reflectedVelocity;
        }

        var targetX = Mathf.Clamp(this.Position.x, Globals.WorldBoundaries.xMin + this.HalfWidth, Globals.WorldBoundaries.xMax - this.HalfWidth);
        var targetY = Mathf.Clamp(this.Position.y, Globals.WorldBoundaries.yMin + this.HalfHeight, Globals.WorldBoundaries.yMax - this.HalfHeight);
        this.Position = new Vector3(targetX, targetY, this.transform.position.z);
    }

    protected virtual void HandleThrust()
    {
        float thrust = CrossPlatformInputManager.GetAxis("Vertical");
        if (Mathf.Approximately(thrust, 0.0f))
        {
            if( this.AudioSource.isPlaying )
            {
                this.AudioSource.Pause();
            }
        }
        else
        {
            Vector2 forward = this.transform.TransformDirection(Vector2.up);

            var reverseModifier = 1.0f;

            if (thrust < 0.0f)
            {
                reverseModifier = -0.25f;
            }

            var newVelocity = forward * this.Speed * (thrust + reverseModifier) * Time.deltaTime;
            newVelocity.x = Mathf.Min(newVelocity.x, this.maxSpeed);
            newVelocity.y = Mathf.Min(newVelocity.y, this.maxSpeed);

            this.Body.velocity += newVelocity;

            if( this.AudioSource.isPlaying == false )
            {
                this.AudioSource.UnPause();
            }
        }
    }

    protected void HandleShot()
    {
        if(CrossPlatformInputManager.GetButtonDown("Shoot"))
        {
            if(Shoot())
            {
                ++this.playStatistics.shotsFired;
            }
        }    
    }

    protected void HandleSpecialShot()
    {
        if(!this.choosingShip && CrossPlatformInputManager.GetButtonDown("SpecialShot"))
        {
            switch(this.shipType)
            {
                case ShipType.Special1:
                    //Rainbow cannon
                    if(Shoot(ShotType.Rainbow))
                    {
                        ++this.playStatistics.tagfeeCannonShots;
                    }
                    break;
                case ShipType.Special2:
                    //Missiles
                    if(Shoot(ShotType.Missile))
                    {
                        ++this.playStatistics.missileShots;
                    }
                    break;
                case ShipType.Special3:
                    //Bomb
                    if(Shoot(ShotType.Bomb))
                    {
                        ++this.playStatistics.bombsDropped;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    protected void HandleSpecialCancel()
    {
        if(this.IsSpecialActive && CrossPlatformInputManager.GetButtonDown("SpecialShot"))
        {
            if (this.shipType == ShipType.Special1)
            {
                this.GetComponentInChildren<RainbowLaser>().Cancel();
            }
            else if(this.shipType == ShipType.Special3)
            {
                this.DroppedBomb.Detonate();
            }			
        }
    }

    /// <summary>
    /// Updates the score UI and handles input.
    /// </summary>
    protected override void OnUpdate()
    {	
        base.OnUpdate();

        if(this.LockMovement)
        {
            HandleRotation(this.rainbowShotRotationSpeedModifier.Value);
            HandleSpecialCancel();
            return;
        }

        // Handle Rotation Actions BEFORE adding directional thrust.
        HandleRotation();
        HandleUTurn();

        //Maintain Boundaries & handle reflection.
        HandleBoundaryBounce();

        // Handle Acceleration.
        HandleThrust();

        // Movement related activities complete, now handle attack actions.
        HandleShot();
        HandleSpecialCancel();	
        HandleSpecialShot();	
    }

    /// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }
    
    /// <summary>
    /// Raises the trigger enter event.
    /// </summary>
    /// <param name="collider">The detected trigger.</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleTriggers(collider);
    }
}
