using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityStandardAssets.CrossPlatformInput;

public class GameController : BaseController 
{
    [SerializeField]
    private Text scoreUI;

    [SerializeField]
    /// <summary>
    /// Reference to the game message UI.
    /// </summary>
    private Text gameMessage;

    [SerializeField]
    /// <summary>
    /// Reference to the sub message UI.
    /// </summary>
    private Text subMessage;

    [SerializeField]
    /// <summary>
    /// Reference to the score message UI.
    /// </summary>
    private Text scoreMessage;

    [SerializeField]
    /// <summary>
    /// Reference to the redeem code message UI.
    /// </summary>
    private Text redeemMessage;

    [SerializeField]
    /// <summary>
    /// Reference to the score elements.
    /// </summary>
    private RankItem[] scoreList;

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
    /// Reference to the game over audio.
    /// </summary>
    [SerializeField]
    protected AudioClip gameOverMusic;

    /// <summary>
    /// Reference to the sound emitter for entering initials.
    /// </summary>
    [SerializeField]
    protected AudioSource initialsEnteringSound;

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

    [SerializeField]
    protected EnterInitialsCanvas initialsCanvas;

    /// <summary>
    /// Quit game flag.
    /// </summary>
    private bool endGame = false;

    /// <summary>
    /// Quit flag.
    /// </summary>
    private bool isQuittable = false;

    [SerializeField]
    private float initialsInputDelay = 0.25f;

    [SerializeField]
    private GameObject postToTwitterMenu;

    [SerializeField]
    private Button yesPostButton;

    /// <summary>
    /// Flag for whether or not the input check for gatheing initials should happen.
    /// </summary>
    private bool isEnteringInitials = false;

    private int newScoreIndex = -1;

    private int currentLetter = 0;

    private const string initialOptions = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";

    private float initialsLastInputTime = 0.0f;



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
        
        this.gameMessage.text = "Ready Player One";
        this.gameMessage.gameObject.SetActive(true);
        Invoke("ClearMessage", 1.0f);
    }
    
    void ClearMessage()
    {
        this.gameMessage.text = "";
        this.gameMessage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public override void OnUpdate () 
    {
        this.scoreUI.text = "Score: " + Mathf.FloorToInt(Globals.Score).ToString();
        
        // Check if the player has died.
        if(this.endGame)
        {
            Globals.LockScore = true;
            this.endGame = false;

            this.DoEndGame();

            this.gameMessage.text = "Game Over";
            this.gameMessage.gameObject.SetActive(true);
            this.audioSource.clip = this.gameOverMusic;
            this.audioSource.Play();
        }

        if(isEnteringInitials)
        {
            GetInitials();         
        }

        // Go back to main menu on button press.
        if ((this.isQuittable && AnyButtonPressed()))
        {
            GotoMenu();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GotoMenu();
        }
    }

    private IEnumerator AutoQuit()
    {
        yield return new WaitForSeconds(30.0f);
        GotoMenu();
    }

    private void AllowQuit()
    {
        this.subMessage.gameObject.SetActive(true);            
        this.isQuittable = true;
        StartCoroutine(AutoQuit());
    }

    private void AskForInitials()
    {
        this.initialsCanvas.gameObject.SetActive(true);
        this.initialsCanvas.GetCurrentInitial().Text = initialOptions[this.currentLetter].ToString();
        this.isEnteringInitials = true;
    }

    private void GetInitials()
    {
        //Handle Initials entering.
        var currentInitial = this.initialsCanvas.GetCurrentInitial();

        if(Time.time - this.initialsLastInputTime > this.initialsInputDelay)
        {
            float verticalAxis = CrossPlatformInputManager.GetAxis("Vertical");
            float horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal");
            this.initialsLastInputTime = Time.time;
            if(currentInitial != null && !Mathf.Approximately(verticalAxis, 0.0f))
            {                
                if(verticalAxis < 0.0f)
                {
                    this.currentLetter = this.currentLetter < initialOptions.Length - 1 ? this.currentLetter + 1 : 0;
                }
                else
                {
                    this.currentLetter = this.currentLetter > 0 ? this.currentLetter - 1 : initialOptions.Length - 1;                      
                }
                this.initialsEnteringSound.Play();
                currentInitial.Text = initialOptions[this.currentLetter].ToString();
            }
            // Done to prevent accidental switch with the joystick. Giving letter choice preference
            else if(!Mathf.Approximately(horizontalAxis, 0.0f))  
            {
                if (horizontalAxis > 0.0f)
                {
                    var nextInitial = this.initialsCanvas.NextInitial();
                    if(nextInitial != null)
                    {
                        nextInitial.Text = initialOptions[this.currentLetter].ToString();
                    }
                }
                else
                {
                    this.isEnteringInitials = true;
                    if(currentInitial != null && this.initialsCanvas.FirstInitial != currentInitial)
                    {
                        currentInitial.Text = "";
                    }
                    var previousInitial = this.initialsCanvas.PreviousInitial();
                    this.currentLetter = initialOptions.IndexOf(previousInitial.Text);
                }
                this.initialsEnteringSound.Play();
            }
        }   
    }

    public void SubmitHighScore()
    {
        this.initialsCanvas.SubmitButton.gameObject.SetActive(false);
        this.isEnteringInitials = false;
    
        string name = this.initialsCanvas.FirstInitial.Text.Replace("_"," ") + 
                      this.initialsCanvas.MiddleInitial.Text.Replace("_", " ") + 
                      this.initialsCanvas.LastInitial.Text.Replace("_", " ");

        UserScore newHighScore = new UserScore {
            rank = this.newScoreIndex, 
            username = name, 
            score = (int)Globals.Score
        };

        Globals.HighScores.Insert(this.newScoreIndex, newHighScore);
        Globals.HighScores.RemoveAt(Globals.HighScores.Count - 1);

        #if PERSIST_SCORES
        Globals.SaveHighScores();
        #endif

        #if OFFLINE_MODE && !CABINET_MODE
        this.postToTwitterMenu.SetActive(true);
        this.yesPostButton.Select();
        #else
        Invoke("AllowQuit", 0.5f);     
        #endif
    }

#if MOZAUTH_DISABLED && !OFFLINE_MODE
    private void DoEndGame()
    {
        this.leaderboard.GetRedeemCode(Mathf.FloorToInt(Globals.Score), (code) => {
            this.redeemMessage.text += code;
            this.redeemMessage.gameObject.SetActive(true);
            for (int i = 0; i < Globals.HighScores.Count; ++i)
            {
                if (Globals.Score >= Globals.HighScores[i].score)
                {
                    this.scoreMessage.text = "New High Score!\nEnter your initials";
                    this.scoreMessage.gameObject.SetActive(true);
                    foreach(var rankMessage in this.scoreList)
                    {
                        rankMessage.gameObject.SetActive(false);
                    }
                    this.newScoreIndex = i;
                    
                    //Invoke("AskForInitials", 2.0f);
                    AskForInitials();
                    break;
                }
            }        
                
            if(this.newScoreIndex < 0)
            {
                Invoke("AllowQuit", 2.0f);
            }
        });  

        this.leaderboard.SendStats(Mathf.FloorToInt(Globals.Score), this.player.PlayStatistics, (message) => {
            print(message);
        });
    }
    #else
    private void DoEndGame()
    {       
        ++Globals.TotalGamesPlayed;
        Globals.PlayerOverallStats = Globals.PlayerOverallStats + this.player.PlayStatistics;
        Globals.LastPlayStats = this.player.PlayStatistics;

        if(Globals.PlayersHighestScore < Mathf.FloorToInt(Globals.Score))
        {
            Globals.PlayersHighestScore = Mathf.FloorToInt(Globals.Score);
            Globals.BestPlayStats = this.player.PlayStatistics;
        }

        Globals.SaveStats();

        for (int i = 0; i < Globals.HighScores.Count; ++i)
        {
            if (Globals.Score >= Globals.HighScores[i].score)
            {
                this.scoreMessage.text = "New High Score!\nEnter your initials";
                this.scoreMessage.gameObject.SetActive(true);
                foreach(var rankMessage in this.scoreList)
                {
                    rankMessage.gameObject.SetActive(false);
                }
                this.newScoreIndex = i;

                Invoke("AskForInitials", 2.0f);
                break;
            }
        }        

        if(this.newScoreIndex < 0)
        {
            Invoke("AllowQuit", 2.0f);
        }
    }
    #endif

    /// <summary>
    /// Goes to the menu scene.
    /// </summary>
    public void GotoMenu()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");
    }

    public void GoTweet()
    {
        SceneManager.LoadScene("TweetScore");
    }

    public void NoTweet()
    {
        this.postToTwitterMenu.SetActive(false);
        Invoke("AllowQuit", 2.0f);
    }
}
