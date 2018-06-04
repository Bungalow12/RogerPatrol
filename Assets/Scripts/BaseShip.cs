using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Type of ships.
/// </summary>
public enum ShipType
{
    Normal,
    Special1,
    Special2,
    Special3
}

/// <summary>
/// The type of shot.
/// </summary>
public enum ShotType
{
    Laser,
    Rainbow,
    Missile,
    Bomb
}

/// <summary>
/// The level of damage of the ship.
/// </summary>
public enum DamageLevel
{
    None,
    Low,
    Medium,
    High ,
    Danger,
    Critical
}

[System.Serializable]
public class ShipCannons
{
    public Transform[] cannons;
}

public class BaseShip : ActiveObject 
{

    /// <summary>
    /// The Max index to display on Damage Level.
    /// </summary>
    private static readonly int[] DamageRanges = {100, 80, 60, 40, 20, 10};

    [SerializeField]
    protected ResponsiveInt maxHealth = new ResponsiveInt(100, 0, ModificationStyle.ADDITIVE);

    /// <summary>
    /// Health
    /// </summary>
    [SerializeField]
    protected int health = 100;

    /// <summary>
    /// The speed modifier of the ship
    /// </summary>
    [SerializeField]
    protected ResponsiveFloat speed = new ResponsiveFloat(7.5f, 0.2f, ModificationStyle.ADDITIVE);

    [SerializeField]
    /// <summary>
    /// The rate of fire.
    /// </summary>
    protected ResponsiveFloat[] rateOfFire = new ResponsiveFloat[4];

    /// <summary>
    /// The last shot time.
    /// </summary>
    protected float lastShotTime = 0.0f;

    [SerializeField]
    /// <summary>
    /// The rate of specialfire.
    /// </summary>
    protected ResponsiveFloat rainbowShotCooldown = new ResponsiveFloat(10.0f, 0.0f, ModificationStyle.ADDITIVE);

    [SerializeField]
    /// <summary>
    /// Rotation speed while shooting your special laser.
    /// </summary>
    protected ResponsiveFloat rainbowShotRotationSpeedModifier = new ResponsiveFloat(0.1f, 0.0f, ModificationStyle.ADDITIVE);

    [SerializeField]
    /// <summary>
    /// The rate of specialfire.
    /// </summary>
    protected ResponsiveFloat missileShotCooldown = new ResponsiveFloat(5.0f, 0.0f, ModificationStyle.ADDITIVE);

    [SerializeField]
    /// <summary>
    /// The rate of specialfire.
    /// </summary>
    protected ResponsiveFloat bombShotCooldown = new ResponsiveFloat(10.0f, 0.0f, ModificationStyle.ADDITIVE);

    /// <summary>
    /// The last shot time.
    /// </summary>
    protected float lastSpecialShotTime = 0.0f;
    
    [SerializeField]
    /// <summary>
    /// Reference to the child laser cannon emitter game objects.
    /// </summary>
    protected ShipCannons[] laserCannons = new ShipCannons[1];
    
    [SerializeField]
    protected Transform specialCannon;
    
    [SerializeField]
    /// <summary>
    /// Reference to the laser prefab
    /// </summary>
    protected Laser laser;

    [SerializeField]
    /// <summary>
    /// Reference to the missile prefab
    /// </summary>
    protected Missile missile;
    
    [SerializeField]
    /// <summary>
    /// Reference to the bomb prefab
    /// </summary>
    protected Bomb bomb;
    
    [SerializeField]
    /// <summary>
    /// Reference to the rainbow laser prefab
    /// </summary>
    protected RainbowLaser rainbowLaser;
    
    /// <summary>
    /// The ship type currently in use. (For players and powerups mostly)
    /// </summary>
    [SerializeField]
    protected ShipType shipType;
    
    /// <summary>
    /// Reference to all the views on the ship to enable or disable them.
    /// </summary>
    [SerializeField]
    protected GameObject[] availableViews;
    
    /// <summary>
    /// Explosion prefab reference.
    /// </summary>
    [SerializeField]
    protected Explosion explosion;
    
    /// <summary>
    /// Reference to power up prefab.
    /// </summary>
    [SerializeField]
    protected PowerUp powerUpPrefab;

    [SerializeField]
    protected ResponsiveFloat maxSpeedMultiplier = new ResponsiveFloat(2.0f, 0.2f, ModificationStyle.ADDITIVE);

    [SerializeField]
    protected GameObject[] SmokeEmitters;
    
    protected float maxSpeed;

    public bool IsSpecialActive
    {
        get;
        set;
    }

    public Bomb DroppedBomb
    {
        get;
        set;
    }	
    
    /// <summary>
    /// Gets and sets the ship's health.
    /// </summary>
    /// <returns>The ships current health.</returns>
    public int Health
    {
        get
        {
            return this.health;
        }
        set
        {
            this.health = value;
            
            var damageLevel = GetDamageLevel();
            if(damageLevel == DamageLevel.None)
            {
                foreach(var emitter in this.SmokeEmitters)
                {
                    emitter.SetActive(false);
                }
            }
            else
            {				
                for(int i = 0; i < (int)damageLevel; ++i)
                {
                    this.SmokeEmitters[i].SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the lock on the user's movement.
    /// </summary>
    /// <returns>True if movement is locked.</returns>
    public bool LockMovement
    {
        get;
        set;
    }
    
    /// <summary>
    /// Gets and sets the Ship Type.
    /// </summary>
    /// <returns>The Ship Type.</returns>
    public ShipType ShipType
    {
        get
        {
            return this.shipType;
        }
        set
        {
            this.shipType = value;
            this.physicalCollider = null;
            this.IsSpecialActive = false;
        }
    }

    /// <summary>
    /// Gets or sets the speed modifier.
    /// </summary>
    /// <value>The speed.</value>
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

    protected virtual DamageLevel GetDamageLevel()
    {
        for(int i = DamageRanges.Length - 1; i > 0; --i)
        {
            if (this.health <= DamageRanges[i])
            {
                return (DamageLevel)i;
            }
        }

        return DamageLevel.None;
    }

    /// <summary>
    /// Overridable OnUpdate event.
    /// </summary>
    protected override void OnUpdate()
    {
        base.OnUpdate();
        this.maxSpeed = this.Speed * this.maxSpeedMultiplier.Value;
        float xSpeed = Mathf.Min(Mathf.Abs(this.Velocity.x), this.maxSpeed);
        float ySpeed = Mathf.Min(Mathf.Abs(this.Velocity.y), this.maxSpeed);
        
        if (this.Velocity.x < 0.0f)
        {
            xSpeed *= -1.0f;
        }
        
        if (this.Velocity.y < 0.0f)
        {
            ySpeed *= -1.0f;
        }
        
        this.Velocity = new Vector2(xSpeed, ySpeed);
    }

    /// <summary>
    /// Overridable OnFiuxedUpdate event.
    /// </summary>
    protected virtual void OnFixedUpdate()
    {
        //Nothing at the base.
    }

    /// <summary>
    /// Handles the collision.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);
    }

    /// <summary>
    /// Changes the ship type and sets the correct view.
    /// </summary>
    /// <param name="shipType"></param>
    protected void SetView(ShipType shipType)
    {
        this.availableViews[(int)this.ShipType].gameObject.SetActive(false);
        this.availableViews[(int)shipType].gameObject.SetActive(true);
        this.ShipType = shipType;
    }

    /// <summary>
    /// Drop a randomized powerup.
    /// </summary>
    /// <param name="location">Location.</param>
    protected void DropPowerUp(Vector3 location)
    {
        DropPowerUp(ShipType.Normal, location);
    }

    /// <summary>
    /// Dropping a specific power up ( OR, if you use ShipType.Normal ... the drop will be randomized )
    /// </summary>
    /// <param name="shipType"></param>
    /// <param name="location"></param>
    protected void DropPowerUp(ShipType shipType, Vector3 location)
    {
        var powerUp = (PowerUp)Instantiate(this.powerUpPrefab, location, Quaternion.identity);
        powerUp.Scale = powerUp.transform.localScale;
        if (shipType != ShipType.Normal)
        {
            powerUp.ShipType = shipType;
        }
    }
    
    public virtual void ApplyDamage(int damageTaken)
    {
        this.Health = Mathf.Max(0, this.Health - damageTaken);
        if (this.Health == 0)
        {
            KillShip();
        }
    }

    public virtual void KillShip()
    {
        Detonate();
    }
    
    protected virtual void Detonate()
    {
        //Create explosion
        var explosion = (Explosion)Instantiate(this.explosion, this.transform.position, Quaternion.identity);
        explosion.gameObject.layer = LayerMask.NameToLayer("Player");
        // explosion.gameObject.layer = this.gameObject.layer;
    }
    
    /// <summary>
    /// Gets the appropriate cannon to fire from.
    /// </summary>
    /// <returns>The cannons for firing.</returns>
    protected Transform[] GetCannonsFromShipType()
    {
        Transform[] cannons = this.laserCannons[(int)this.shipType].cannons;
        
        return cannons;
    }
    
    /// <summary>
    /// Handles event of killing an enemy AI.
    /// </summary>
    public virtual void OnKillAI(Enemy enemy)
    {
        
    }
    
    /// <summary>
    /// Handles event of destroying an asteroid.
    /// </summary>
    public virtual void OnDestroyAsteroid(Asteroid asteroid)
    {
    }
    
    /// <summary>
    /// Shoots weapon.
    /// </summary>
    public virtual bool Shoot(ShotType shotType = ShotType.Laser)
    {
        bool shotFired = false;
        switch(shotType)
        {			
            case ShotType.Rainbow:
                if(Time.time - this.lastSpecialShotTime >= this.rainbowShotCooldown.Value)
                {
                    shotFired = true;
                    this.IsSpecialActive = true;
                    this.lastSpecialShotTime = Time.time;
                    ShootRainbow();
                }
                break;
            case ShotType.Missile:
                if(Time.time - this.lastSpecialShotTime >= this.missileShotCooldown.Value)
                {
                    shotFired = true;
                    this.IsSpecialActive = true;
                    this.lastSpecialShotTime = Time.time;
                    ShootMissile();
                }
                break;
            case ShotType.Bomb:
                if(Time.time - this.lastSpecialShotTime >= this.bombShotCooldown.Value)
                {
                    shotFired = true;
                    this.IsSpecialActive = true;
                    this.lastSpecialShotTime = Time.time;
                    DropBomb();
                }
                break;
            case ShotType.Laser:
                if(Time.time - this.lastShotTime >= this.rateOfFire[(int)this.shipType].Value)
                {
                    shotFired = true;
                    this.lastShotTime = Time.time;
                    ShootLaser();
                }
                break;
        }	
        return shotFired;
    }
    
    /// <summary>
    /// Shoots lasers from each cannon.
    /// </summary>
    protected virtual void ShootLaser()
    {
        //Create shot
        var cannons = GetCannonsFromShipType();

        var perShotVolume = 0.35f;
        switch (cannons.Length)
        {
            case 5:
                perShotVolume = 0.2f;        
                break;
        }

        foreach(var cannon in cannons)
        {
            var laser = (Instantiate(this.laser, cannon.position, cannon.rotation) as Laser);
            laser.AudioSource.volume = perShotVolume;
            laser.Owner = this;
            laser.gameObject.layer = this.gameObject.layer;
            Vector3 forward = laser.transform.TransformDirection(Vector3.up); 
            laser.Velocity = forward * laser.Speed;
        }
    }
    
    /// <summary>
    /// Shoots a massive rainbow laser.
    /// </summary>
    protected virtual void ShootRainbow()
    {
        var blast = (Instantiate(this.rainbowLaser, this.specialCannon.position, Quaternion.identity) as RainbowLaser);
        blast.Owner = this;
        blast.transform.rotation = this.transform.rotation;

        blast.gameObject.layer = this.gameObject.layer;	
        blast.transform.SetParent(this.gameObject.transform);
        
        //Stop ship.
        this.Velocity = Vector3.zero;
        this.LockMovement = true;
    }
    
    /// <summary>
    /// Shoots missiles from each cannon.
    /// </summary>
    protected virtual void ShootMissile()
    {
        //Create missiles
        var cannons = GetCannonsFromShipType();

        foreach(var cannon in cannons)
        {
            var newMissile = (Instantiate(this.missile, cannon.position, cannon.rotation) as Missile);
            newMissile.Owner = this;
            newMissile.gameObject.layer = this.gameObject.layer;
        }
    }
    
    /// <summary>
    /// Drops a bomb
    /// </summary>
    protected virtual void DropBomb()
    {		
        var bomb = (Instantiate(this.bomb, this.Position - (this.transform.up * (this.Position.y - this.Bottom)),
                    Quaternion.identity) as Bomb);
        bomb.Owner = this;
        //bomb.transform.rotation = this.transform.rotation;
        this.DroppedBomb = bomb;

        bomb.gameObject.layer = this.gameObject.layer;	
    }
    
    /// <summary>
    /// Gets a random location in the world.
    /// </summary>
    /// <returns>A random location in the world.</returns>
    protected Vector3 GetRandomLocation()
    {
        return new Vector3 (Random.Range (Globals.WorldBoundaries.xMin + 10, Globals.WorldBoundaries.xMax - 10), 
                            Random.Range (Globals.WorldBoundaries.yMin + 10, Globals.WorldBoundaries.yMax - 10),
                            0.0f);
    }
}
