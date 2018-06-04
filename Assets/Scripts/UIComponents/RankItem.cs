using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankItem : MonoBehaviour 
{
    [SerializeField]
    private Text rank;

    [SerializeField]
    private Text playerName;

    [SerializeField]
    private Text score;

    [SerializeField]
    private bool isMine;

    private Color targetColor;

    [SerializeField]
    private Color[] colorSteps = {Color.red, new Color(1.0f, 0.55f, 0), Color.yellow, Color.green, Color.blue, new Color(0.56f, 0.0f, 1.0f)};

    private int currentColorIndex = 0;
    private float lastTValue = 0.0f;

    private bool tWasDeclining = false;

    private bool isReady = false;

    /// <summary>
    /// Gets and sets the rank.
    /// </summary>
    /// <returns></returns>
    public string Rank
    {
        get
        {
            return this.rank.text;
        }
        set
        {
            this.rank.text = value;
        }
    }

    /// <summary>
    /// Gets and sets the player's name.
    /// </summary>
    /// <returns></returns>
    public string PlayerName
    {
        get
        {
            return this.playerName.text;
        }
        set
        {
            this.playerName.text = value;
        }
    }

    /// <summary>
    /// Gets and sets the player's score.
    /// </summary>
    /// <returns></returns>
    public string Score
    {
        get
        {
            return this.score.text;
        }
        set
        {
            this.score.text = value;
        }
    }

    /// <summary>
    /// Flag as to whether or not the item is the players.
    /// </summary>
    /// <returns>True if it is the players row.</returns>
    public bool IsMine
    {
        get
        {
            return this.isMine;
        }
        set
        {
            this.isMine = value;
        }
    }

    /// <summary>
    /// Gets and Sets the collor of the row.
    /// </summary>
    /// <returns>The color of the row.</returns>
    public Color Color
    {
        get
        {
            return this.rank.color;
        }
        set
        {
            this.rank.color = value;
            this.playerName.color = value;
            this.score.color = value;
        }
    }

    public void Start()
    {
        this.targetColor = this.colorSteps[this.currentColorIndex];
        this.isReady = true;
    }

    /// <summary>
    /// Updates the Color if the value is mine.
    /// </summary>
    public void Update()
    {
        if(this.isReady && this.IsMine)
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
            this.rank.color = currentColor;
            this.playerName.color = currentColor;
            this.score.color = currentColor;
        }
    }

    private bool Approximately(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b);
    }
}
