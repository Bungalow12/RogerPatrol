using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInitial : MonoBehaviour 
{
	[SerializeField]
    private bool isActive;

    private Color targetColor;

    [SerializeField]
    private Color[] colorSteps = {Color.red, new Color(1.0f, 0.55f, 0), Color.yellow, Color.green, Color.blue, new Color(0.56f, 0.0f, 1.0f)};

    private int currentColorIndex = 0;
    private float lastTValue = 0.0f;

    private bool tWasDeclining = false;

    private bool isReady = false;

	private Text textComponent;

	private Text TextComponent
	{
		get
		{
			return this.textComponent ?? GetComponent<Text>();
		}
	}
	
	/// <summary>
    /// Flag as to whether or not the item is the players.
    /// </summary>
    /// <returns>True if it is the players row.</returns>
    public bool IsCurrent
    {
        get
        {
            return this.isActive;
        }
        set
        {
            this.isActive = value;
        }
    }

	public string Text
	{
		get
		{
			return this.TextComponent.text;
		}
		set
		{
			this.TextComponent.text = value;
		}
	}

	// Use this for initialization
	void Start () 
	{
		this.textComponent = GetComponent<Text>();
		this.targetColor = this.colorSteps[this.currentColorIndex];
        this.isReady = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(this.isReady)
        {
			if(this.IsCurrent)
			{
				float t =  Mathf.PingPong(Time.time, 1);
				Color currentColor = Color.Lerp(Color.white, this.targetColor, t);

				//Not perfect but it looks random and always picks a new color and provides a good look.
				if(this.tWasDeclining && t >= this.lastTValue) // Detect going from down to up
				{
					//Pick new color
					this.tWasDeclining = false;
					this.currentColorIndex = (this.currentColorIndex + 1) >= this.colorSteps.Length ? 0 : this.currentColorIndex + 1;
					this.targetColor = this.colorSteps[this.currentColorIndex];
				}

				this.tWasDeclining = t < this.lastTValue;
				this.lastTValue = t;
				this.TextComponent.color = currentColor;
			}
			else
			{
				this.TextComponent.color = Color.white;
			}
        }
	}
    /// <summary>
    /// Dostuff the specified jonny, russel and ben.
    /// </summary>
    /// <returns>The dostuff.</returns>
    /// <param name="jonny">Jonny.</param>
    /// <param name="russel">Russel.</param>
    /// <param name="ben">If set to <c>true</c> ben.</param>
    public GameObject DoStuff(int jonny, float russel, bool ben)
    {
        return null;
    }
}
