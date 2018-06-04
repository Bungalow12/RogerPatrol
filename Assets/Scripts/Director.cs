using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The class responsible for the spawning of enemies and asteroids for the game.
/// </summary>
public class Director : MonoBehaviour 
{
    /// <summary>
    /// The maximum number of asteroids that can be on screen at a time.
    /// </summary>
    [SerializeField]
    private ResponsiveInt asteroidSpawnMax = new ResponsiveInt(20, 5, ModificationStyle.ADDITIVE);

    /// <summary>
    /// The asteroid spawn rate.
    /// </summary>
    [SerializeField]
    private ResponsiveFloat asteroidSpawnDelay = new ResponsiveFloat(5.0f, -0.3f, ModificationStyle.ADDITIVE);

    /// <summary>
    /// The asteroid prefabs for creation.
    /// </summary>
    [SerializeField]
    private Asteroid[] asteroids;

    [SerializeField]
    private int absoluteMaxAsteroidCount = 50;

    /// <summary>
    /// The time since last asteroid spawn.
    /// </summary>
    private float lastAsteroidSpawnTime = 0.0f;

    /// <summary>
    /// The maximum number of enemies that can be on screen at a time.
    /// </summary>
    [SerializeField]
    private ResponsiveInt enemySpawnMax = new ResponsiveInt(20, 0, ModificationStyle.ADDITIVE);

    /// <summary>
    /// The enemy spawn rate.
    /// </summary>
    [SerializeField]
    private ResponsiveFloat enemySpawnDelay = new ResponsiveFloat(5.0f, -0.3f, ModificationStyle.ADDITIVE);

    /// <summary>
    /// The time since last spawn.
    /// </summary>
    private float lastEnemySpawnTime = 0.0f;
    
    /// <summary>
    /// The enemy prefabs for creation.
    /// </summary>
    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();
    
    /// <summary>
    /// The power up prefab.
    /// </summary>
    [SerializeField]
    private PowerUp powerUp;

    /// <summary>
    /// Gets the Asteroid Prefabs.
    /// </summary>
    /// <value>The asteroid prefabs.</value>
    public Asteroid[] AsteroidPrefabs
    {
        get
        {
            return this.asteroids;
        }
    }

    public void createPowerup()
    {
        PowerUp newPowerUp = (PowerUp)Instantiate(this.powerUp, GetRandomLocation(), Quaternion.identity);
        newPowerUp.Scale = newPowerUp.transform.localScale;
    }

    /// <summary>
    /// Called once on scene start.
    /// </summary>
    void Start()
    {
        createPowerup();
    }

    /// <summary>
    /// The update occuring at fixed intervals rather than frame rate.
    /// </summary>
    void Update()
    {
        // Handle asteroid spawning.
        int currentAsteroidCount = GameObject.FindGameObjectsWithTag ("Asteroid").Length;
        if (currentAsteroidCount < System.Math.Min( this.asteroidSpawnMax.Value, this.absoluteMaxAsteroidCount ))
        {
            if (Time.time - this.lastAsteroidSpawnTime >= this.asteroidSpawnDelay.Value) 
            {
                this.lastAsteroidSpawnTime = Time.time;
                SpawnAsteroid();
            }
        }

        var totalEnemiesInScene = GameObject.FindGameObjectsWithTag("Enemy").Length;
        // Handle enemy spawning.
        if (totalEnemiesInScene < this.enemySpawnMax.Value) 
        {
            if (Time.time - this.lastEnemySpawnTime >= this.enemySpawnDelay.Value) 
            {
                this.lastEnemySpawnTime = Time.time;
                SpawnEnemy();
            }
        }
    }

    /// <summary>
    /// The side enumeration for use with the world boundaries
    /// </summary>
    private enum Side
    {
        Left,
        Top,
        Right,
        Bottom
    }

    /// <summary>
    /// Spawns a new asteroid at one of the world boundaries with a random starting point, velocity, spin and size.
    /// </summary>
    void SpawnAsteroid()
    {
        //Pick side
        var side = (Side)Random.Range (0, 4);

        //Select position and velocity
        Vector2 position = Vector2.zero;
        Vector2 velocity = Vector2.zero;

        switch (side) {
        case Side.Left:
            position = new Vector2 (Globals.WorldBoundaries.xMin - 20f, Random.Range (Globals.WorldBoundaries.yMin, Globals.WorldBoundaries.yMax));
            velocity = new Vector2 (Random.Range (0.0f, 11.0f), Random.Range (-10.0f, 11.0f));
            break;
        case Side.Top:
            position = new Vector2 (Random.Range (Globals.WorldBoundaries.xMin, Globals.WorldBoundaries.xMax), Globals.WorldBoundaries.yMax + 20f);
            velocity = new Vector2 (Random.Range (-10.0f, 11.0f), Random.Range (-10.0f, 1.0f));
            break;
        case Side.Right:
            position = new Vector2 (Globals.WorldBoundaries.xMax + 20f, Random.Range (Globals.WorldBoundaries.yMin, Globals.WorldBoundaries.yMax));
            velocity = new Vector2 (Random.Range (-10.0f, 1.0f), Random.Range (-10.0f, 11.0f));
            break;
        case Side.Bottom:
            position = new Vector2 (Random.Range (Globals.WorldBoundaries.xMin, Globals.WorldBoundaries.xMax), Globals.WorldBoundaries.yMin - 20f);
            velocity = new Vector2 (Random.Range (-10.0f, 11.0f), Random.Range (0.0f, 11.0f));
            break;
        }

        //Create asteroid
        Asteroid newAsteroid = Instantiate (this.asteroids[Random.Range(0, this.asteroids.Length)], position, Quaternion.identity) as Asteroid;

        //Add Spin
        newAsteroid.Spin = Random.Range (0.0f, 6.0f);
        newAsteroid.Velocity = velocity;

        // // Add Choose Size
        // int scaleX = Random.Range(1, 4);
        // int scaleY = Random.Range(1, 4);
        // newAsteroid.Scale = new Vector3(scaleX, scaleY, 1);
        //
        // //Get mass
        // int mass = (int)newAsteroid.Body.mass;
        // mass = mass * Mathf.Max (scaleX, scaleY);
        // if (scaleX != scaleY)
        // {
        //     mass = mass >> 1; //Faster version of mass = mass / 2
        // }
    }

    /// <summary>
    /// Spawns a new enemy at one of the world boundaries with a random starting point
    /// </summary>
    void SpawnEnemy()
    {
        //Pick side
        var side = (Side)Random.Range (0, 4);
        
        //Select position and velocity
        Vector2 position = Vector2.zero;
        
        switch (side) {
        case Side.Left:
            position = new Vector2 (Globals.WorldBoundaries.xMin - 20f, Random.Range (Globals.WorldBoundaries.yMin, Globals.WorldBoundaries.yMax));
            break;
        case Side.Top:
            position = new Vector2 (Random.Range (Globals.WorldBoundaries.xMin, Globals.WorldBoundaries.xMax), Globals.WorldBoundaries.yMax + 20f);
            break;
        case Side.Right:
            position = new Vector2 (Globals.WorldBoundaries.xMax + 20f, Random.Range (Globals.WorldBoundaries.yMin, Globals.WorldBoundaries.yMax));
            break;
        case Side.Bottom:
            position = new Vector2 (Random.Range (Globals.WorldBoundaries.xMin, Globals.WorldBoundaries.xMax), Globals.WorldBoundaries.yMin - 20f);
            break;
        }
        
        //Create enemy
        var enemyType = Random.Range(0, this.enemies.Count);
        Instantiate(this.enemies[enemyType], position, Quaternion.identity);
    }
    
    /// <summary>
    /// Gets a random location in the world.
    /// </summary>
    /// <returns>A random location in the world.</returns>
    Vector3 GetRandomLocation()
    {
        return new Vector3 (Random.Range (Globals.WorldBoundaries.xMin + 10, Globals.WorldBoundaries.xMax - 10), 
                            Random.Range (Globals.WorldBoundaries.yMin + 10, Globals.WorldBoundaries.yMax - 10),
                            0.0f);
    }
}
