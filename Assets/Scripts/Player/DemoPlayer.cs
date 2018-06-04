using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets._2D;

/// <summary>
/// The AI demo player object.
/// </summary>
public class DemoPlayer : Player 
{	    
    private GetPowerUp getPowerUpBehavior;

    private DemoPlayerAttack attackBehavior;

    [SerializeField]
    /// <summary>
    /// Reference to the Danger detection collision circle.
    /// </summary>
    protected CircleCollider2D detector;

    [SerializeField]
    /// <summary>
    /// The enemy's target object.
    /// </summary>
    protected GameObject target;

    /// <summary>
    /// Reference to the State Machine.
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// Gets the danger detector.
    /// </summary>
    /// <value>The detector.</value>
    public CircleCollider2D Detector
    {
        get
        {
            this.detector = this.detector ?? GetComponentInChildren<CircleCollider2D>();
            return this.detector;
        }
    }

    /// <summary>
    /// Gets or sets the target.
    /// </summary>
    /// <value>The target.</value>
    public GameObject Target
    {
        get
        {
            return this.target;
        }
        set
        {
            this.target = value;
        }
    }

    /// <summary>
    /// Gets the State Machine.
    /// </summary>
    /// <value>The state machine animator.</value>
    public Animator Animator
    {
        get
        {
            this.animator = this.animator ?? GetComponent<Animator>();
            return this.animator;
        }
    }

    protected override void OnStart()
    {
        GameObject.Find("Main Camera").GetComponent<CameraFollow>().SetPlayer(this.transform);
        this.getPowerUpBehavior = this.Animator.GetBehaviour<GetPowerUp>();
        this.getPowerUpBehavior.Self = this;

        this.attackBehavior = this.Animator.GetBehaviour<DemoPlayerAttack>();
        this.attackBehavior.Self = this;
        
        base.OnStart();
    }

    protected override void HandleThrust()
    {
        if (Mathf.Approximately(this.Velocity.x, 0.0f) || Mathf.Approximately(this.Velocity.y, 0.0f))
        {
            if( this.AudioSource.isPlaying )
            {
                this.AudioSource.Pause();
            }
        }
        else
        {
            if( this.AudioSource.isPlaying == false )
            {
                this.AudioSource.UnPause();
            }
        }
    }

    /// <summary>
    /// Updates the score UI and handles input.
    /// </summary>
    protected override void OnUpdate()
    {	
        base.OnUpdate();        

        //Maintain Boundaries & handle reflection.
        HandleBoundaryBounce();

        // Handle Acceleration.
        HandleThrust();


        // Handle powerup time-limit.
        if (this.ShipType != ShipType.Normal)
        {
            var timePoweredUp = Time.time - this.powerTime;
            if(timePoweredUp >= this.powerTimeLimit)
            {
                // Powerup time expired, return to normal.
                this.SetView(ShipType.Normal);
            }
        }

        // If the player isn't moving, they aren't scoring.
        if ( this.Velocity.magnitude < 0.2f )
        {
            return;
        }

        float multiplier = 1.0f + this.killsMultiplier;
        float points = this.survivalPointsRate * multiplier * Time.deltaTime;

        this.UpdateScore(points);
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

    public override void KillShip()
    {
        var position = this.gameObject.transform.position;

        // Switch to the camera listener.
        this.gameObject.GetComponent<AudioListener>().enabled = false;
        GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true;

        Destroy(this.gameObject);		
        Detonate();

        GameObject.Find("DemoController").GetComponent<DemoController>().EndGame = true;
    }
}
