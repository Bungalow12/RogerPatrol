using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Main menu canvas.
/// </summary>
public class MainMenuCanvas : MonoBehaviour 
{
    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Text browserMessageText;

    [SerializeField]
    private Button tutorialButton;

    [SerializeField]
    private Button highScoresButton;

    [SerializeField]
    private Button creditsButton;

    [SerializeField]
    private Button logoutButton;

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private Button twitterButton;

    [SerializeField]
    private Button statsButton;

    /// <summary>
    /// Gets the play button.
    /// </summary>
    /// <value>The play button.</value>
    public Button PlayButton
    {
        get
        {
            return this.playButton;
        }
    }

    /// <summary>
    /// Gets the browser message text.
    /// </summary>
    /// <value>The browser message text.</value>
    public Text BrowserMessageText
    {
        get
        {
            return this.browserMessageText;
        }
    }

    /// <summary>
    /// Gets the tutorial button.
    /// </summary>
    /// <value>The tutorial button.</value>
    public Button TutorialButton
    {
        get
        {
            return this.tutorialButton;
        }
    }

    /// <summary>
    /// Gets the High Scores button.
    /// </summary>
    /// <value>The High Scores button.</value>
    public Button HighScoresButton
    {
        get
        {
            return this.highScoresButton;
        }
    }

    /// <summary>
    /// Gets the credits button.
    /// </summary>
    /// <value>The credits button.</value>
    public Button CreditsButton
    {
        get
        {
            return this.creditsButton;
        }
    }

    public Button LogoutButton
    {
        get
        {
            return this.logoutButton;
        }
    }

    public Button QuitButton
    {
        get
        {
            return this.quitButton;
        }
    }

    // <summary>
    /// Gets the Twitter Connect button.
    /// </summary>
    /// <value>The Twitter Connect button.</value>
    public Button TwitterButton
    {
        get
        {
            return this.twitterButton;
        }
    }

    public Button StatsButton
    {
        get
        {
            return this.statsButton;
        }
    }
}
