using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Base Projectile game object.
/// </summary>
public abstract class Projectile : ActiveObject 
{
    /// <summary>
    /// The damage dealt by the projectile.
    /// </summary>
    [SerializeField]
    protected ResponsiveInt damage = new ResponsiveInt(10, 2.5f, ModificationStyle.ADDITIVE);

    /// <summary>
    /// The owner game object.
    /// </summary>
    protected BaseShip owner;
    
    /// <summary>
    /// The speed of the projectile
    /// </summary>
    [SerializeField]
    protected ResponsiveFloat speed = new ResponsiveFloat(60.0f, 5.0f, ModificationStyle.ADDITIVE);

    /// <summary>
    /// List of sounds to use when projectile fired.
    /// </summary>
    [SerializeField]
    public List<AudioClip> sounds = new List<AudioClip>();
    
    /// <summary>
    /// The time to live.
    /// </summary>
    [SerializeField]
    protected ResponsiveFloat timeToLive = new ResponsiveFloat(2.0f, -0.02f, ModificationStyle.ADDITIVE);
    
    /// <summary>
    /// Time of creation.
    /// </summary>
    protected float startTime;
    
    /// <summary>
    /// Prevent more than one kill.
    /// </summary>
    protected bool isDead = false;
    
    /// <summary>
    /// Explosion prefab reference.
    /// </summary>
    [SerializeField]
    protected Explosion explosion;
    
    /// <summary>
    /// Gets the Damage dealt by the projectile.
    /// </summary>
    /// <returns>The damage dealt.</returns>
    public int Damage
    {
        get
        {
            return this.damage.Value;
        }
    }

    public float TimeToLive
    {
        get
        {
            return this.timeToLive.Value;
        }
    }

    void Start()
    {
        this.startTime = Time.time;
        OnStart();
    }

    /// <summary>
    /// Gets or sets the owner.
    /// </summary>
    /// <value>The owner game object.</value>
    public BaseShip Owner
    {
        get
        {
            return this.owner;
        }

        set
        {
            this.owner = value;
        }
    }
    
    /// <summary>
    /// Gets and sets the speed.
    /// </summary>
    /// <returns>The speed.</returns>
    public float Speed
    {
        get
        {
            return this.speed.Value;
        }
        set
        {
            this.speed.Value = value;
        }
    }

    /// <summary>
    /// Sets the owner.
    /// Invokable without via messaging.
    /// </summary>
    /// <param name="owner">Owner ship.</param>
    public void SetOwner(BaseShip owner)
    {
        this.owner = owner;
    }

    protected override void OnStart()
    {		
        base.OnStart();
        this.PlaySound(this.sounds);
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () 
    {
        OnUpdate();	
    }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(!isDead && Time.time - this.startTime >= this.TimeToLive)
        {
            Kill();
        }
        
        if (IsOutOfWorld)
        {
            Kill();
        }
    }
    
    /// <summary>
    /// Handles the collision.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    protected override void HandleCollision (Collision2D collision)
    {
        base.HandleCollision(collision);
        
        //Create explosion
        var explosion = (Explosion)Instantiate(this.explosion, this.transform.position, Quaternion.identity);
        explosion.Scale = new Vector3(0.25f, 0.25f, 1.0f);
        explosion.Owner = this.owner;
        // These are not to hurt anything.
        explosion.gameObject.layer = LayerMask.NameToLayer("Depth1");
        
        Kill();
    }
    
    /// <summary>
    /// Tells the server to detonate the missile.
    /// </summary>
    //[Command]
    protected virtual void Kill()
    {	
        this.isDead = true;
        Destroy(this.gameObject);
    }
}
