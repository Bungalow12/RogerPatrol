using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Explosion
/// </summary>
public class Explosion : ActiveObject 
{
    /// <summary>
    /// The damage dealt by the explosion.
    /// </summary>
    [SerializeField]
    protected int damage = 50;


    /// <summary>
    /// The time to live.
    /// </summary>
    [SerializeField]
    protected float timeToLive = 0.5f;
    
    /// <summary>
    /// The time of creation.
    /// </summary>
    protected float startTime;
    

    [SerializeField]
    /// <summary>
    /// The list of possible laser sounds.
    /// </summary>
    protected List<AudioClip> soundClips = new List<AudioClip>();

    /// <summary>
    /// The owner game object.
    /// </summary>
    protected BaseShip owner;

    /// <summary>
    /// Gets the Damage dealt by the projectile.
    /// </summary>
    /// <returns>The damage dealt.</returns>
    public int Damage
    {
        get
        {
            return this.damage;
        }
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
    
    // Use this for initialization
    void Start () 
    {
        OnStart();
    }
    
    protected override void OnStart()
    {
        this.startTime = Time.time;
        this.PlaySound(this.soundClips);
    }
    
    // Update is called once per frame
    void Update () 
    {
        OnUpdate();  
    }
    
    protected override void OnUpdate()
    {
        var elapsed = Time.time - this.startTime;

        if (elapsed >= this.timeToLive)
        {
            this.PhysicsCollider.enabled = false;
            if(this.Renderer != null)
            {
                this.Renderer.enabled = false;
            }
            
            if (elapsed >= this.AudioSource.clip.length)
            {
                Destroy(this.gameObject);
            }
        }  
    }
}
