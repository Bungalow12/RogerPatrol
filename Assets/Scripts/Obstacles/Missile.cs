using UnityEngine;
using System.Collections;

public class Missile : Projectile 
{	

    protected override void OnUpdate()
    {
        this.RotationSpeed = 12.0f;
        this.Speed = 50.0f;
        
        Vector3 forward = new Vector2(0.0f, 1.0f); //Up in the local space.
        this.transform.Translate(forward * (this.Speed * Time.deltaTime));
        
        base.OnUpdate();
    }
    
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
        explosion.Owner = this.owner;
        ((SpriteRenderer)explosion.Renderer).color = new Color(0.88f, 0.56f, 0.0f, 0.85f);
        // Only the explosions from Missles should hurt people.
        explosion.gameObject.layer = LayerMask.NameToLayer("Explosion");
        
        base.Kill();
    }
}
