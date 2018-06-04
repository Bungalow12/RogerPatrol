using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base physical game object providing easier access to some attributes.
/// </summary>
public class PhysicalObject : MonoBehaviour 
{
    protected Collider2D physicalCollider;
    
    private Rigidbody2D body;
    
    private Renderer objectRenderer;

    // Used to avoid division.
    private float? halfWidth;
    private float? halfHeight;

    protected Quaternion startingRotation;
    protected Quaternion targetRotation;
    protected bool isTurning = false;

    /// <summary>
    /// Gets the physics collider once.
    /// </summary>
    /// <value>The physics collider.</value>
    public virtual Collider2D PhysicsCollider
    {
        get
        {
            this.physicalCollider = this.physicalCollider ?? GetComponent<Collider2D>();
            return this.physicalCollider;
        }
    }

    /// <summary>
    /// Most Objects need their own audio source, move this logic to a common place.
    /// </summary>
    protected AudioSource audioSource;

    /// <summary>
    /// MEMOIZED Retrieval of this objects AudioSource.
    /// </summary>
    public AudioSource AudioSource
    {
        get
        {
            this.audioSource = this.audioSource ?? GetComponent<AudioSource>();
            return this.audioSource;
        }
    }

    /// <summary>
    /// Play a sound clip using this objects AudioSource.
    /// </summary>
    /// <param name="soundToPlay"></param>
    public void PlaySound(AudioClip soundToPlay)
    {
        this.AudioSource.clip = soundToPlay;
        this.AudioSource.Play();
    }

    /// <summary>
    /// Play a random member of a list of sound clips using this objects AudioSource.
    /// </summary>
    /// <param name="soundsToPlayOneOf"></param>
    public int PlaySound(List<AudioClip> soundsToPlayOneOf)
    {
        if((soundsToPlayOneOf == null) ||
           (soundsToPlayOneOf.Count == 0))
        {
            return -1;
        }

        var soundIndex = Random.Range(0, soundsToPlayOneOf.Count);
        this.PlaySound(soundsToPlayOneOf[soundIndex]);

        return soundIndex;
    }

    /// <summary>
    /// Gets the body once.
    /// </summary>
    /// <value>The body.</value>
    public Rigidbody2D Body
    {
        get
        {
            this.body = this.body ?? GetComponent<Rigidbody2D>();
            return this.body;
        }
    }
    
    /// <summary>
    /// Gets the renderer once.
    /// </summary>
    /// <value>The renderer.</value>
    public Renderer Renderer
    {
        get
        {
            this.objectRenderer = this.objectRenderer ?? GetComponent<Renderer>();
            return this.objectRenderer;
        }
    }

    /// <summary>
    /// Gets the left of the object.
    /// </summary>
    /// <value>The left.</value>
    public float Left 
    {
        get 
        {
            return this.PhysicsCollider.bounds.min.x;
        }
    }
    
    /// <summary>
    /// Gets the right of the object.
    /// </summary>
    /// <value>The right.</value>
    public float Right 
    {
        get 
        {
            return this.PhysicsCollider.bounds.max.x;
        }
    }
    
    /// <summary>
    /// Gets the center of the object.
    /// </summary>
    /// <value>The center.</value>
    public Vector3 Center 
    {
        get 
        {
            return this.PhysicsCollider.bounds.center;
        }
    }
    
    /// <summary>
    /// Gets the bottom of the object.
    /// </summary>
    /// <value>The bottom.</value>
    public float Bottom 
    {
        get 
        {
            return this.PhysicsCollider.bounds.min.y;
        }
    }
    
    /// <summary>
    /// Gets the top of the object.
    /// </summary>
    /// <value>The top.</value>
    public float Top 
    {
        get 
        {
            return this.PhysicsCollider.bounds.max.y;
        }
    }
    
    /// <summary>
    /// Gets the width of the object.
    /// </summary>
    /// <value>The width.</value>
    public float Width 
    {
        get 
        {
            return this.PhysicsCollider.bounds.size.x;
        }
    }
    
    /// <summary>
    /// Gets the height of the object.
    /// </summary>
    /// <value>The height.</value>
    public float Height 
    {
        get 
        {
            return this.PhysicsCollider.bounds.size.y;
        }
    }

    /// <summary>
    /// Gets the width of the object.
    /// </summary>
    /// <value>The width.</value>
    public float HalfWidth 
    {
        get 
        {
            if(!this.halfWidth.HasValue)
            {
                this.halfWidth = this.Width / 2;
            }

            return this.halfWidth.Value;
        }
    }
    
    /// <summary>
    /// Gets the height of the object.
    /// </summary>
    /// <value>The height.</value>
    public float HalfHeight 
    {
        get 
        {
            if(!this.halfHeight.HasValue)
            {
                this.halfHeight = this.Height / 2;
            }
            
            return this.halfHeight.Value;
        }
    }

    /// <summary>
    /// Gets or sets the position of the object.
    /// </summary>
    /// <value>The position.</value>
    public Vector3 Position
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    /// <summary>
    /// Gets or sets the velocity of the object.
    /// </summary>
    /// <value>The velocity.</value>
    public Vector3 Velocity
    {
        get
        {
            return this.Body.velocity;
        }
        set
        {
            this.Body.velocity = value;
        }
    }

    /// <summary>
    /// Gets or sets the velocity of the object.
    /// </summary>
    /// <value>The velocity.</value>
    public Vector3 Scale
    {
        get
        {
            return this.transform.localScale;
        }
        set
        {
            this.transform.localScale = value;
            this.halfWidth = null;
            this.halfHeight = null;
        }
    }

    /// <summary>
    /// Raises the change scale event.
    /// </summary>
    /// <param name="scale">Scale.</param>
    public virtual void OnChangeScale(Vector3 scale)
    {
        this.Scale = scale;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is out of world.
    /// </summary>
    /// <value><c>true</c> if this instance is out of world; otherwise, <c>false</c>.</value>
    public bool IsOutOfWorld
    {
        get
        {
            bool result = false;
            if(this.Left < Globals.WorldBoundaries.xMin - 40 || 
               this.Right > Globals.WorldBoundaries.xMax + 40 ||
               this.Top > Globals.WorldBoundaries.yMax + 40 ||
               this.Bottom < Globals.WorldBoundaries.yMin - 40)
            {
                result = true;
            }
            return result;
        }
    }

    /// <summary>
    /// Rotates the object to look at the target point on the screen.
    /// </summary>
    /// <param name="target">Target.</param>
    public void LookAtScreen(Vector3 screenTarget)
    {
        Vector3 me = Camera.main.WorldToScreenPoint(this.transform.localPosition);

        Vector2 lookAt = screenTarget - me;

        float angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg - 90.0f;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Looks at a specific world location immediately.
    /// </summary>
    /// <param name="target"></param>
    public void LookAtImmediate(Vector3 target)
    {
        Vector2 lookAt = target - this.Position;
        
        float angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg - 90.0f;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Looks at a specific world location.
    /// </summary>
    /// <param name="obj">Game Object.</param>
    public void LookAt(Vector3 target)
    {
        this.isTurning = true;
        this.startingRotation = this.transform.rotation;
        Vector2 lookAt = target - this.Position;
        
        float angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg - 90.0f;
        this.targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Looks at a specific game object.
    /// </summary>
    /// <param name="obj">Game Object.</param>
    public void LookAt(GameObject obj)
    {
        LookAt(obj.transform.position);
    }
}
