using UnityEngine;
using System.Collections;

/// <summary>
/// The base Enemy game object.
/// </summary>
public class Enemy : BaseShip 
{		
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

    [SerializeField]
    /// <summary>
    /// The point multiplier value.
    /// </summary>
    protected ResponsiveFloat pointMultiplierValue = new ResponsiveFloat(1.0f, 0.25f, ModificationStyle.ADDITIVE);

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

    /// <summary>
    /// Gets or sets the point multiplier value.
    /// </summary>
    /// <value>The point multiplier value.</value>
    public float PointMultiplierValue
    {
        get
        {
            return this.pointMultiplierValue.Value;
        }
        set
        {
            this.pointMultiplierValue.Value = value;
        }
    }

    /// <summary>
    /// Update method called once per frame.
    /// </summary>
    void Update()
    {
        OnUpdate();
    }
    
    /// <summary>
    /// Handles the collision with other game objects.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);

        if (collision.collider.tag == "Laser" || 
            collision.collider.tag == "Rainbow" ||
            collision.collider.tag == "Bomb" ||
            collision.collider.tag == "Missile")
        {
            var laser = collision.collider.gameObject.GetComponent<Projectile>();
            ApplyDamage(laser.Damage);
            if (this.health <= 0)
            {
                laser.Owner.OnKillAI(this);
            }
        }
    }

    /// <summary>
    /// Handles the collision with triggers.
    /// </summary>
    /// <param name="collider">The detected trigger collision.</param>
    protected override void HandleTriggers(Collider2D collider)
    {
        base.HandleTriggers(collider);
        
        if(collider.tag == "Explosion")
        {
            var explosion = collider.gameObject.GetComponent<Explosion>();
            ApplyDamage(explosion.Damage);
            if(this.health <= 0)
            {
                if(explosion.Owner != null)
                {
                    explosion.Owner.OnKillAI(this);
                }
            }
        }
    }
    
    protected override void Detonate()
    {
        //Create explosion
        var explosion = (Explosion)Instantiate(this.explosion, this.transform.position, Quaternion.identity);
        explosion.gameObject.layer = LayerMask.NameToLayer("Depth1");
        
        Destroy(this.gameObject);
    }
}
