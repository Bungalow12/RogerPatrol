using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Asteroid game object.
/// </summary>
public class Asteroid : ActiveObject 
{

    /// <summary>
    /// The number of points this asteroid is worth (initial destruction
    /// and debris destruction).
    /// </summary>
    [SerializeField]
    private float points = 5.0f;

    /// <summary>
    /// Explosion to use when this asteroid is 'sploded.
    /// </summary>
    [SerializeField]
    private Explosion explosion;

    /// <summary>
    /// What is the minimum quantity of children to spawn when this asteroid is shot?
    /// </summary>
    [SerializeField]
    private int minChildren = 1;

    /// <summary>
    /// What is the maximum quantity of children to spawn when this asteroid is shot?
    /// </summary>
    [SerializeField]
    private int maxChildren = 3;

    /// <summary>
    /// This is the max speed/rotation multiplier for the smaller chunks that come from this asteroid breaking up.
    /// </summary>
    [SerializeField]
    private float breakupMagnification = 1.5f;

    /// <summary>
    /// The maximum variation in scale during break up.
    /// </summary>
    [SerializeField]
    private float scaleVariation = 0.1f;

    /// <summary>
    /// The spin speed of the object.
    /// </summary>
    [SerializeField]
    private float spin = 0.0f;

    /// <summary>
    /// children asteroids, one size down from this. The smallest asteroids will have no children.
    /// </summary>
    [SerializeField]
    private Asteroid[] childAsteroids;

    /// <summary>
    /// Gets or sets the spin speed of the object in degrees.
    /// </summary>
    /// <value>The spin.</value>
    public float Spin
    {
        get
        {
            return this.spin;
        }

        set
        {
            this.spin = value;
        }
    }

    /// <summary>
    /// Gets or sets the total points this asteroid is worth (initial
    /// destruction and debris destruction).
    /// </summary>
    /// <value>The points value.</value>
    public float Points
    {
        get
        {
            return this.points;
        }

        set
        {
            this.points = value;
        }
    }
        
    /// <summary>
    /// Update is called once per fixed framerate frame
    /// </summary>
    void Update () 
    {
        this.Body.AddTorque(spin * Time.deltaTime, ForceMode2D.Impulse);

        if (IsOutOfWorld)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Handles the collision with other game objects.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    protected override void HandleCollision(Collision2D collision)
    {
        if (collision.collider.tag == "Laser" || 
            collision.collider.tag == "Rainbow" ||
            collision.collider.tag == "Bomb" ||
            collision.collider.tag == "Missile" ||
            collision.collider.tag == "Player")
        {
            // If there are no child classes, then we can't make children.
            if (childAsteroids.Length == 0)
            {
                minChildren = 0;
                maxChildren = 0;
            }
            var numToMake = (int)(Random.Range(minChildren, maxChildren));

            // Precalc limits 1 time.
            var newMaxXVelocity = this.Velocity.x * this.breakupMagnification;
            var newMaxYVelocity = this.Velocity.y * this.breakupMagnification;

            var newMinXPosition = this.transform.position.x - this.HalfHeight;
            var newMaxXPosition = this.transform.position.x + this.HalfHeight;

            var newMinYPosition = this.transform.position.y - this.HalfWidth;
            var newMaxYPosition = this.transform.position.y + this.HalfWidth;

            var newMaxRotation = this.Spin * this.breakupMagnification;

            for (int i = 0; i < numToMake; i++)
            {
                // VELOCITY //////////////////////////////////////////////////////////////////////////

                var newVelocityX = Random.Range(-newMaxXVelocity, newMaxXVelocity);
                var newVelocityY = Random.Range(-newMaxYVelocity, newMaxYVelocity);

                var mixedVelocityX = (this.Velocity.x * 0.75f);
                mixedVelocityX += (newVelocityX * 0.25f);

                var mixedVelocityY = (this.Velocity.y * 0.75f);
                mixedVelocityY += (newVelocityY * 0.25f);

                var mixedVelocity = new Vector2(newVelocityX, newVelocityY);

                var flipX = Random.Range(0, 1) >= 0.5f;
                var flipY = Random.Range(0, 1) >= 0.5f;

                // POSITION //////////////////////////////////////////////////////////////////////////

                var newXPosition = Random.Range(newMinXPosition, newMaxXPosition);
                var newYPosition = Random.Range(newMinYPosition, newMaxYPosition);

                var newPosition = new Vector2( newXPosition, newYPosition );

                // ROTATION //////////////////////////////////////////////////////////////////////////

                var newRotation = Random.Range(0, newMaxRotation);

                var newPoints = Points / numToMake;

                var chosenPrefabIndex = (int)(Random.Range(0, childAsteroids.Length));
                Asteroid chosenPrefab = childAsteroids[chosenPrefabIndex];

                Asteroid newAsteroid = (Asteroid) Instantiate(chosenPrefab, newPosition, Quaternion.identity);
                newAsteroid.Velocity = mixedVelocity;
                newAsteroid.Spin = newRotation;
                newAsteroid.Points = newPoints;
                // newAsteroid.Scale = new Vector2(newAsteroid.Points, newAsteroid.Points);
                newAsteroid.gameObject.layer = LayerMask.NameToLayer("Asteroid");

                // ASSUMPTION: that the base scale is square, x == y.
                var scale = newAsteroid.Scale.x;
                var scaleDelta = scale * scaleVariation;

                var scaleX = Random.Range(scale - scaleDelta, scale + scaleDelta);
                if (flipX)
                {
                    scaleX *= -1.0f;
                }

                var scaleY = Random.Range(scale - scaleDelta, scale + scaleDelta);
                if (flipY)
                {
                    scaleY *= -1.0f;
                }

                newAsteroid.Scale = new Vector2(scaleX, scaleY);

            }

            var explosion = (Explosion)Instantiate(this.explosion, this.transform.position, Quaternion.identity);
            explosion.Owner = null;
            explosion.gameObject.layer = LayerMask.NameToLayer("Asteroid Explosion");
            explosion.tag = "Asteroid Explosion";
            explosion.Scale = this.Scale;
            explosion.AudioSource.volume = 0.25f;

            var renderer = ((SpriteRenderer)explosion.Renderer);
            renderer.color = new Color(0.0f, 0.0f, 0.0f, 0.25f);

            if (collision.collider.tag != "Player")
            {
                var projectile = collision.collider.gameObject.GetComponent<Projectile>();
                projectile.Owner.OnDestroyAsteroid(this);
            }
            
            Destroy();
        }
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
    /// Handles the collision with triggers.
    /// </summary>
    /// <param name="collider">The detected trigger collision.</param>
    protected override void HandleTriggers(Collider2D collider)
    {
        base.HandleTriggers(collider);
        
        if(collider.tag == "Explosion")
        {
            var explosion = collider.gameObject.GetComponent<Explosion>();
            if(explosion.Owner != null)
            {
                explosion.Owner.OnDestroyAsteroid(this);
            }
            Destroy();
        }
    }

    /// <summary>
    /// Raises the trigger enter event.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        HandleTriggers(collider);
    }

    /// <summary>
    /// Tells the Server to destroy it.
    /// </summary>
    protected void Destroy()
    {       
        Destroy(this.gameObject);
    }
}
