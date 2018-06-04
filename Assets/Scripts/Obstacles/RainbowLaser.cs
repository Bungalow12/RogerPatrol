using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainbowLaser : Projectile 
{
    [SerializeField]
    protected GameObject rainbow;
    
    [SerializeField]
    protected GameObject beam;
    
    [SerializeField]
    protected GameObject lights;

    [SerializeField]
    protected GameObject orb;
    
    [SerializeField]
    protected SpriteRenderer lightStart;
    
    [SerializeField]
    protected SpriteRenderer lightMiddle;
    
    [SerializeField]
    protected AudioSource soundEmitter;

    /// <summary>
    /// List of sounds to use when projectile fired.
    /// </summary>
    [SerializeField]
    public List<AudioClip> shutdownSounds = new List<AudioClip>();
    
    [SerializeField]
    protected Animator laserAnimator;
    
    [SerializeField]
    protected RainbowArc[] rainbowOverlayPrefabs;
    
    [SerializeField]
    protected float pushBackForce = 200.0f;

    [SerializeField]
    protected float overlaySpawnDelay = 0.05f;

    protected float overlaySpawnTime = 0.0f;
    
    [SerializeField]
    protected Transform overlaySpawn;
    
    private bool spawned = false;

    private int usedSoundIndex;
    
    protected override void OnStart()
    {		
        this.usedSoundIndex = this.PlaySound(this.sounds);
    }

    public void Spawned()
    {
        this.Owner.Body.AddForce(this.Owner.transform.TransformDirection(Vector3.down) * pushBackForce, ForceMode2D.Impulse);
        this.rainbow.SetActive(true);
        this.lights.SetActive(true);
        this.spawned = true;
        laserAnimator.SetBool("spawned", true);
    }
    
    // Update is called once per frame
    protected override void OnUpdate() 
    {
        if(this.spawned)
        {
            float randomOpacity = Random.Range (0.2f, 0.3f);
            lightStart.color = new Color(1f, 1f, 1f, randomOpacity);
            lightMiddle.color = new Color(1f, 1f, 1f, randomOpacity);
            
            Vector3 newScale = beam.transform.localScale;
            newScale.x = Random.Range (0.9f, 1.1f);
            beam.transform.localScale = newScale;
            
            if (Time.time - this.overlaySpawnTime >= this.overlaySpawnDelay)
            {
                this.overlaySpawnTime = Time.time;
                RainbowArc newOverlay = Instantiate(rainbowOverlayPrefabs[Random.Range(0, rainbowOverlayPrefabs.Length)], 
                overlaySpawn.position, Quaternion.identity) as RainbowArc;
                newOverlay.Owner = this.Owner;
                newOverlay.transform.rotation = this.transform.rotation;
                
                newOverlay.transform.SetParent(gameObject.transform);
            }
        }
        
        if(!isDead && Time.time - this.startTime >= this.TimeToLive)
        {
            Cancel();            
        }
    }

    public void FinishBlast()
    {
        this.isDead = true;
        if(this.usedSoundIndex >= 0)
        {
            this.PlaySound(this.shutdownSounds[this.usedSoundIndex]);
        }

        this.owner.IsSpecialActive = false;
        // Make the blast appear dead.
        this.spawned = false;
        this.rainbow.SetActive(false);
        this.lights.SetActive(false);
        this.orb.SetActive(false);
        
        // Destroy all left over arcs.
        var allArcs = GetComponentsInChildren<RainbowArc>();
        foreach (var arc in allArcs)
        {
            Destroy(arc.gameObject);
        }

        this.Owner.LockMovement = false;
    }

    public void Cancel()
    {
        FinishBlast();
        //Kill when the sound completes.
        Invoke("TimedKill", this.shutdownSounds[this.usedSoundIndex].length);
    }

    public void TimedKill()
    {
        Kill();
    }
}
