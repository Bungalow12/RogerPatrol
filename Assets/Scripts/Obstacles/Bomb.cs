using UnityEngine;

/// <summary>
/// Bomb weapon.
/// </summary>
public class Bomb : Projectile 
{	
    [SerializeField]
    protected AudioClip normalTick;
    
    [SerializeField]
    protected AudioClip finalTick;
    
    [SerializeField]
    protected AudioSource countdownEmitter;
    
    [SerializeField]
    protected Shockwave shockwave;

    [SerializeField]
    protected float tickDelay = 1.0f;
    
    protected float lastTickTime = 0.0f;
    
    protected override void HandleCollision(Collision2D collision)
    {
        Kill();
    }
    
    /// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    void OnCollisionEnter2D(Collision2D collision)
    {		
        HandleCollision(collision);
    }
    
    /// <summary>
    /// Tells the server to detonate the missile.
    /// </summary>
    //[Command]
    protected override void Kill()
    {
        this.owner.IsSpecialActive = false;
        this.PhysicsCollider.enabled = false;
        
        //Create explosion
        var explosion = (Explosion)Instantiate(this.explosion, this.transform.position, Quaternion.identity);
        explosion.Scale = new Vector3(3.0f, 3.0f, 3.0f);
        explosion.Owner = this.owner;

        var shockwave = (Shockwave)Instantiate(this.shockwave, this.transform.position, Quaternion.identity);
        shockwave.Owner = this.owner;

        // The explosions from Bombs should not hurt dropper.
        explosion.gameObject.layer = LayerMask.NameToLayer("Bomb Explosion");
        shockwave.gameObject.layer = LayerMask.NameToLayer("Bomb Explosion");
        
        base.Kill();
    }
    
    
    protected override void OnStart()
    {
        this.lastTickTime = Time.time;
        base.OnStart();
    }
    
    protected override void OnUpdate()
    {
        var elapsed = Time.time - this.startTime;
        
        if(!isDead && Time.time - this.lastTickTime >= this.tickDelay)
        {
            this.lastTickTime = Time.time;
            
            if(elapsed < this.TimeToLive - this.tickDelay)
            {
                this.countdownEmitter.clip = this.normalTick;
            }
            else
            {
                this.countdownEmitter.clip = this.finalTick;
                Invoke("TimedKill", this.finalTick.length);
            }
            
            this.countdownEmitter.Play();
        }
        
        if (IsOutOfWorld)
        {
            Kill();
        }
    }
    
    public void Detonate()
    {
        Kill();
    }

    public void TimedKill()
    {
        Kill();
    }
}
