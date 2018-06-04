using UnityEngine;

public class PowerUp : PhysicalObject 
{
    /// <summary>
    /// The ship type given by the power up.
    /// </summary>
    [SerializeField]
    private ShipType shipType = ShipType.Normal;

    /// <summary>
    /// Sets whether the ship selection is random or preset.
    /// </summary>
    [SerializeField]
    protected bool isRandom = true;
    
    /// <summary>
    /// Gets and sets the ship type given by the power up.
    /// </summary>
    /// <returns>The ship type.</returns>
    public ShipType ShipType
    {
        get
        {
            return this.shipType;	
        }
        set
        {
            this.shipType = value;
        }
    }

    /// <summary>
    /// Initializer for the Powerup, to preset the ship to a random ship type.
    /// </summary>
    protected void Start()
    {
        if(this.isRandom)
        {
            this.setShipTypeRandom();
        }
    }

    /// <summary>
    /// Function to handle the act of setting the current shipType randomly to one of the elements of the ShipType enumeration.
    /// </summary>
    public void setShipTypeRandom()
    {
        System.Array shipTypes = System.Enum.GetValues(typeof(ShipType));
        ShipType selected = (ShipType)shipTypes.GetValue(UnityEngine.Random.Range(1, shipTypes.Length));

        this.ShipType = selected;
    }

    [SerializeField]
    private AudioClip collisionSound;
    
    /// <summary>
    /// Handles Trigger Enter event.
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter2D(Collider2D collider)
    {	
        if(collider.tag == "Player")
        {
            this.PlaySound(this.collisionSound);

            this.GetComponent<CircleCollider2D>().enabled = false;
            this.GetComponent<SpriteRenderer>().enabled = false;

            Destroy(this.gameObject, this.collisionSound.length);
        }	
    }
}
