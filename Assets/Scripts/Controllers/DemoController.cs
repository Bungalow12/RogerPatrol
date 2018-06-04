using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityStandardAssets.CrossPlatformInput;

public class DemoController : BaseController 
{
    [SerializeField]
    private Text scoreUI;

    [SerializeField]
    /// <summary>
    /// The player.
    /// </summary>
    private Player player;

    /// <summary>
    /// Reference to the game controller's audio source.
    /// </summary>
    [SerializeField]
    protected AudioSource audioSource;

    /// <summary>
    /// How many points per level ( used with pointsMultiplier as level increases, 500, 1000, 2000, etc. )
    /// </summary>
    [SerializeField]
    protected float pointsOffset;

    /// <summary>
    /// How big is the jump in required points between levels ( 2.0 means 2x the points )
    /// </summary>
    [SerializeField]
    protected float pointsMultiplier;

    /// <summary>
    /// Quit game flag.
    /// </summary>
    private bool endGame = false;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="GameController"/> end game.
    /// </summary>
    /// <value><c>true</c> if end game; otherwise, <c>false</c>.</value>
    public bool EndGame
    {
        get
        {
            return this.endGame;
        }
        set
        {
            this.endGame = value;
        }
    }

    // Use this for initialization
    public override void OnStart () 
    {
        base.OnStart();

        // Update the level calc values.
        Globals.pointsMultiplier = pointsMultiplier;
        Globals.pointsOffset = pointsOffset;

        Globals.LockScore = false;
        Globals.Score = 0;

        #if !UNITY_WEBGL
        Cursor.visible = false;
        #endif
    }

    // Update is called once per frame
    public override void OnUpdate () 
    {
        this.scoreUI.text = "Score: " + Mathf.FloorToInt(Globals.Score).ToString();
        
        // Check if the player has died.
        if(this.endGame)
        {
            this.endGame = false;
            Invoke("GotoSplash", 2.0f);
        }

        // Go back to main menu on any button.
        if (AnyButtonPressed())
        {
            GotoMenu();
        }
    }

    /// <summary>
    /// Goes to the menu scene.
    /// </summary>
    public void GotoMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Goes to the splash scene.
    /// </summary>
    public void GotoSplash()
    {
        SceneManager.LoadScene("SplashScreen");
    }
}
