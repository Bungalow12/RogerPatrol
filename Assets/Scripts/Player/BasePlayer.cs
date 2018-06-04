using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Base Player game object.
/// </summary>
public class BasePlayer : BaseShip 
{	

    [SerializeField]
    protected CalloutManager calloutManager;

    /// <summary>
    /// Time limit ( in seconds ) for powerups.
    /// </summary>
    [SerializeField]
    protected float powerTimeLimit = 30.0f;

    /// <summary>
    /// The start time for the current powerup cycle.
    /// </summary>
    protected float powerTime = 0.0f;

    [SerializeField]
    /// <summary>
    /// Length of time to cycle through available ship views
    /// </summary>
    protected float rouletteDuration = 1.0f;
    
    /// <summary>
    /// Time between cycling.
    /// </summary>
    [SerializeField]
    protected float rouletteStep = 0.1f;

    /// <summary>
    /// Number of AI kills.
    /// </summary>
    protected int killsAI = 0;

    /// <summary>
    /// Point multiplier from ship kills.
    /// </summary>
    protected float killsMultiplier = 0.0f;

    [SerializeField]
    /// <summary>
    /// Number of points earned for survival every second.
    /// </summary>
    protected float survivalPointsRate = 2.5f;

    [SerializeField]
    protected Explosion shockwave;
    
    [SerializeField]
    private bool isInvulnerable = false;
    
    protected bool choosingShip = false;

    [SerializeField]
    protected bool respawnPowerups = true;

    [SerializeField]
    protected bool canHavePowerUpForever = false;

    [SerializeField]
    protected ResponsiveFloat asteroidDamageResistanceMultiplier = new ResponsiveFloat(0.2f, 0.1f, ModificationStyle.ADDITIVE);

    [SerializeField]
    protected int minimumAsteroidDamage = 10;

    protected UsageStats playStatistics = new UsageStats();

    private float gameStartTime;

    public UsageStats PlayStatistics
    {
        get
        {
            return this.playStatistics;
        }
    }
    
    /// <summary>
    /// Overriden Physical Collider property to find the component in the child.
    /// </summary>
    /// <returns></returns>
    public override Collider2D PhysicsCollider
    {
        get
        {
            this.physicalCollider = this.physicalCollider ?? GetComponentInChildren<Collider2D>();
            return this.physicalCollider;
        }
    }
    
    /// <summary>
    /// Called once on object start.
    /// </summary>
    public void Start()
    {
        OnStart();
    }
    
    protected override void OnStart()
    {
        this.gameStartTime = Time.time;
        base.OnStart();

        this.isInvulnerable = true;
        //Make transparent
        var renderer = this.GetComponentInChildren<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.5f);
        Invoke("MakeVulnerable", 3.0f);

        if(this.calloutManager != null)
        {
            this.calloutManager.PerformCallout(60, "ready", "readyplayerone", "engage", "hugsmode", "goodluck", "helpusroger", "reportingforduty");
        }
    }

    private void MakeVulnerable()
    {
        this.isInvulnerable = false;
        //Make opaque
        var renderer = this.GetComponentInChildren<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1.0f);
    }

    /// <summary>
    /// Updates the score.
    /// </summary>
    /// <param name="points">The change in points.</param>
    protected void UpdateScore(float points)
    {
        Globals.Score += points;
    }

    /// <summary>
    /// Handles event of killing an enemy AI.
    /// </summary>
    public override void OnKillAI(Enemy enemy)
    {
        base.OnKillAI(enemy);
        ++this.playStatistics.enemiesKilled;
        this.killsAI += 1;
        this.killsMultiplier += enemy.PointMultiplierValue;

        if(this.calloutManager != null)
        {
            this.calloutManager.PerformCallout(10, "destroymode", "enemyoptimized", "spamlinksterminated", "droid");
        }
    }

    /// <summary>
    /// Handles event of destroying an asteroid.
    /// </summary>
    public override void OnDestroyAsteroid(Asteroid asteroid)
    {
        base.OnDestroyAsteroid(asteroid);
        ++this.playStatistics.asteroidsDestroyed;
        UpdateScore(asteroid.Points);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () 
    {
        OnUpdate();
    }

    /// <summary>
    /// Update the score based on survival time and multiplier.
    /// </summary>
    protected override void OnUpdate()
    {
        base.OnUpdate();

        // Handle powerup time-limit.
        if (this.ShipType != ShipType.Normal)
        {
            var timePoweredUp = Time.time - this.powerTime;
            if(!this.canHavePowerUpForever && timePoweredUp >= this.powerTimeLimit)
            {
                // Powerup time expired, return to normal.
                this.SetView(ShipType.Normal);
            }
        }

        // If the player isn't thrusting, they aren't scoring.
        float thrust = CrossPlatformInputManager.GetAxis("Vertical");
        if ( Mathf.Approximately(thrust, 0.0f) )
        {
            return;
        }

        float multiplier = 1.0f + this.killsMultiplier;
        float points = survivalPointsRate * multiplier * Time.deltaTime;

        this.UpdateScore(points);
    }

    void FixedUpdate()
    {
        this.OnFixedUpdate();
    }
        
    public override void ApplyDamage(int damageTaken)
    {
        base.ApplyDamage(damageTaken);

        if ( this.calloutManager != null && ( ( this.Health / this.maxHealth.Value ) <= 0.4f ) && ( this.Health > 0.0f ) )
        {
            this.calloutManager.PerformCallout(30, "alert", "warning");
        }
    }

    /// <summary>
    /// Handles the collision.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);
        
        if(!isInvulnerable)
        {
            if(collision.collider.tag == "Enemy")
            {
                KillShip();
            }
            else if(collision.collider.tag == "Laser")
            {	
                var projectile = collision.collider.gameObject.GetComponent<Projectile>();
                ApplyDamage(projectile.Damage);
            }
            else if(collision.collider.tag == "Asteroid")
            {
                //TODO: Apply damage based on size.
                Asteroid asteroid = collision.collider.gameObject.GetComponent<Asteroid>();
                float asteroidMomentum = asteroid.Body.mass * collision.relativeVelocity.magnitude;
                float damage = asteroidMomentum * this.asteroidDamageResistanceMultiplier.Value;
                ApplyDamage(Mathf.Max(this.minimumAsteroidDamage, Mathf.RoundToInt(damage)));
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
        
        if (collider.tag == "PowerUp")
        {
            ++this.playStatistics.powerUpsCollected;
            this.lastSpecialShotTime = 0.0f; 
            this.powerTime = Time.time;

            var powerUp = collider.gameObject.GetComponent<PowerUp>();
            var collectedShip = powerUp.ShipType;

            this.choosingShip = true;

            if(this.respawnPowerups)
            {
                DropPowerUp(GetRandomLocation());
            }		
            
            StartCoroutine(CycleAvailableViews(collectedShip));

            if(this.calloutManager != null)
            {
                this.calloutManager.PerformCallout(60, "powerup", "mathematical", "fiveisalive", "hahaha", "customerjourney");
            }
        }
        else if(collider.tag == "Explosion")
        {
            if(!isInvulnerable)
            {
                ApplyDamage(collider.gameObject.GetComponent<Explosion>().Damage);
            }
        }
    }
    
    /// <summary>
    /// Animates through available ship view options.
    /// </summary>
    protected IEnumerator CycleAvailableViews(ShipType finalShip)
    {
        for(var f = 0.0f; f < this.rouletteDuration; f += this.rouletteStep)
        {			
            SetView((ShipType)Random.Range(0, this.availableViews.Length));
            yield return new WaitForSeconds(this.rouletteStep);
        }
        SetView(finalShip);
        this.choosingShip = false;
    }

    /// <summary>
    /// leave the game.
    /// </summary>
    public void ExitGame()
    {	
        GameObject.Find("GameController").GetComponent<GameController>().EndGame = true;
    }
    
    public void Respawn()
    {
        SetView(ShipType.Normal);
        this.Velocity = Vector3.zero;
        this.Body.angularVelocity = 0.0f;
        Globals.Score = 0.0f;
        this.killsAI = 0;
        this.killsMultiplier = 0.0f;
        
        this.isInvulnerable = true;
        //Make transparent
        var renderer = this.GetComponentInChildren<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.5f);
        Invoke("MakeVulnerable", 3.0f);
        
        var spawnPoints = GameObject.FindObjectsOfType<NetworkStartPosition>();
            
            // Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

        // Set the player’s position to the chosen spawn point
        transform.position = spawnPoint;
    }	
    
    public override void KillShip()
    {
        this.playStatistics.playTime = Time.time - this.gameStartTime;

        // We're done.
        if(this.calloutManager != null)
        {
            this.calloutManager.PerformCallout(50, "gameover", "bladerunner", "signalterminated");
        }

        // Switch to the camera listener.
        this.gameObject.GetComponent<AudioListener>().enabled = false;
        GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true;

        Destroy(this.gameObject);		
        base.KillShip();
        //Respawn();

        GameObject.Find("GameController").GetComponent<GameController>().EndGame = true;
    }

    protected override void Detonate()
    {
        base.Detonate();
        //Create explosion
        var explosion = (Shockwave)Instantiate(this.shockwave, this.transform.position, Quaternion.identity);
        explosion.gameObject.layer = LayerMask.NameToLayer("Explosion");
        // explosion.gameObject.layer = this.gameObject.layer;		
    }
}
