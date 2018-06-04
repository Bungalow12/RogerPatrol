using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Scout Enemy game object.
/// </summary>
public class Scout : Enemy 
{
    private Seek seekBehavior;
    private Attack attackBehavior;
    private Dodge dodgeBehavior;

    /// <summary>
    /// The preferred distance from the target.
    /// </summary>
    [SerializeField]
    private ResponsiveFloat preferredDistance = new ResponsiveFloat(10.0f, 5.0f, ModificationStyle.ADDITIVE);

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () 
    {
        OnStart();
    }

    /// <summary>
    /// Collects references to the scout's game states.
    /// </summary>
    protected override void OnStart ()
    {
        this.health = this.maxHealth.Value;

        this.Animator.SetFloat("PreferredDistance", preferredDistance.Value);

        this.seekBehavior = this.animator.GetBehaviour<Seek>();
        this.seekBehavior.Self = this;

        this.attackBehavior = this.animator.GetBehaviour<Attack>();
        this.attackBehavior.Self = this;
        
        this.dodgeBehavior = this.animator.GetBehaviour<Dodge>();
        this.dodgeBehavior.Self = this;
    }

    /// <summary>
    /// Update method called once per frame.
    /// </summary>
    void Update () 
    {
        OnUpdate();
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
    /// <param name="collider">The detected collision.</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleTriggers(collider);
    }
}
