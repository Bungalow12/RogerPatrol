using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

using System;

/// <summary>
/// Handles main menu input.
/// </summary>
public class MenuController : BaseController 
{
    [SerializeField]
    private MainMenuCanvas MainMenu;

    [SerializeField]
    private Canvas LoginMenu;

    [SerializeField]
    private Text LoginMessage;

    [SerializeField]
    private InputField EmailField;

    [SerializeField]
    private InputField PasswordField;

    [SerializeField]
    private Button LoginButton;
    
    [SerializeField]
    private Text WelcomeText;

    [SerializeField]
    private Text SessionsPausedText;

    [SerializeField]
    private Text connectionUpdateText;
    
    [SerializeField]
    private Text versionText;
    
    private AudioSource audioSource;

    [SerializeField]
    private float DemoStartTimeout = 30.0f;

    private float lastInputTime = 0.0f;

    [SerializeField]
    private float checkIfPausedTimeout = 30.0f;

    private float lastCheckIfPausedTime = 0.0f;

    [SerializeField]
    private CalloutManager calloutManager;

    public override void OnStart()
    {
        base.OnStart();

        this.lastInputTime = Time.time;
        this.audioSource = this.GetComponent<AudioSource>();

        #if !OFFLINE_MODE
        CanPingLeaderboard();
        #endif
        
        #if !MOZAUTH_DISABLED
        SetCookie();
        #else
        this.MainMenu.LogoutButton.gameObject.SetActive(false);
        #endif

        #if CABINET_MODE || UNITY_WEBGL
        PlayerPrefs.SetInt("Finished Tutorial", 1);
        PlayerPrefs.Save();
        this.MainMenu.QuitButton.gameObject.SetActive(false);
        this.MainMenu.TwitterButton.gameObject.SetActive(false);
        this.MainMenu.StatsButton.gameObject.SetActive(true);
        var statsTransform = this.MainMenu.StatsButton.GetComponent<RectTransform>();
        statsTransform.anchoredPosition = new Vector2(0.0f, statsTransform.anchoredPosition.y);
        this.MainMenu.PlayButton.Select();
        #else
        this.MainMenu.StatsButton.gameObject.SetActive(true);
        this.MainMenu.PlayButton.Select();
        #endif

        bool finishedTutorial = PlayerPrefs.GetInt("Finished Tutorial", 0) == 1;
        if(!finishedTutorial)
        {
            this.MainMenu.TutorialButton.gameObject.SetActive(false);
        }
    }

    public void SetCookie()
    {
        // This Method referenced Moz internal code and so areas have been redacted
        bool loggedIn = false; // Redacted
        if(!loggedIn)
        {
            //Show Please login message.
            this.MainMenu.gameObject.SetActive(false);
            this.LoginMenu.gameObject.SetActive(true);
        }
        else
        {
            string displayName = "player1"; // Redacted
            this.WelcomeText.text = "Player " + displayName;
        }
    }

    /// <summary>
    /// Login to Moz Account.
    /// </summary>
    public void Login()
    {
        this.audioSource.Play();
        this.LoginButton.interactable = false;
        this.disableInput = true;
        Invoke("DoLogin", 0.5f);
    }

    public void DoLogin()
    {
        // This Method referenced Moz internal code and so areas have been redacted
        if(string.IsNullOrEmpty(this.EmailField.text) || string.IsNullOrEmpty(this.PasswordField.text))
        {
            return;
        }

        string errorMessage = null;
        bool isLoggedIn = false; // Called Moz Login here. Redacted

        if(!isLoggedIn)
        {
            this.LoginMessage.text = errorMessage;
        }
        else
        {
            this.LoginMessage.text = "Please Login to your Moz Account";
            this.LoginMenu.gameObject.SetActive(false);
            this.MainMenu.gameObject.SetActive(true);

            bool finishedTutorial = PlayerPrefs.GetInt("Finished Tutorial", 0) == 1;
            if(!finishedTutorial)
            {
                this.MainMenu.TutorialButton.gameObject.SetActive(false);
            }

            DeselectAllItems();
            // Saved Session here Redacted
            SetWelcomeText();
        }
        
        this.disableInput = false;
    }

    /// <summary>
    /// Navigate to create a moz account.
    /// </summary>
    public void CreateAccount()
    {
        this.audioSource.Play();
        this.disableInput = true;
        Invoke("DoCreateAccount", 0.5f);
    }

    public void DoCreateAccount()
    {
        PlayerPrefs.SetInt("ClickedCreateAccount", 1);
        PlayerPrefs.Save();
        Application.OpenURL("https://moz.com/community/join");
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Moves to the game scene.
    /// </summary>
    public void PlayGame()
    {
        this.audioSource.Play();

        this.disableInput = true;
        bool finishedTutorial = PlayerPrefs.GetInt("Finished Tutorial", 0) == 1;
        if(finishedTutorial)
        {
            StartCoroutine(LoadScene("LoadingGame"));
        }
        else
        {
            StartCoroutine(LoadScene("LoadingTutorial"));
        }
    }

    /// <summary>
    /// Moves to the tutorial scene.
    /// </summary>
    public void PlayTutorial()
    {
        this.audioSource.Play();
        this.disableInput = true;
        StartCoroutine(LoadScene("LoadingTutorial"));
    }
    
    /// <summary>
    /// Move to credits scene.
    /// </summary>
    public void HighScores()
    {
        this.audioSource.Play();
        this.disableInput = true;
        StartCoroutine(LoadScene("HighScores"));
    }

    /// <summary>
    /// Move to credits scene.
    /// </summary>
    public void ShowCredits()
    {
        this.audioSource.Play();
        this.disableInput = true;
        StartCoroutine(LoadScene("Credits"));
    }

    /// <summary>
    /// Move to credits scene.
    /// </summary>
    public void ShowStats()
    {
        this.audioSource.Play();
        this.disableInput = true;
        StartCoroutine(LoadScene("Stats"));
    }

    /// <summary>
    /// Backs to main.
    /// </summary>
    public void BackToMain()
    {
        //TODO: Add scene stuff for Tutorial or play?
        this.audioSource.Play();
        this.MainMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        this.audioSource.Play();
        this.disableInput = true;
        Invoke("DoQuit", 0.5f);
    }
    
    private void DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Used to logout the current player.
    /// </summary>
    public void Logout()
    {
        this.audioSource.Play();
        this.disableInput = true;
        Invoke("DoLogout", 0.5f);
    }

    private void DoLogout()
    {
        // This Method referenced Moz internal code and so areas have been redacted
        // Redacted: Moz Logout
        this.MainMenu.gameObject.SetActive(false);
        this.LoginMenu.gameObject.SetActive(true);
        this.WelcomeText.text = "";
        this.disableInput = false;
        PlayerPrefs.DeleteKey("Cookie");
        PlayerPrefs.DeleteKey("Finished Tutorial");
        PlayerPrefs.DeleteKey(Globals.PLAYER_PREFS_TWITTER_USER_ID);
        PlayerPrefs.DeleteKey(Globals.PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
        PlayerPrefs.DeleteKey(Globals.PLAYER_PREFS_TWITTER_USER_TOKEN);
        PlayerPrefs.DeleteKey(Globals.PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);
    }    

    /// <summary>
    /// Moves to the Twitter connect scene.
    /// </summary>
    public void ConnectTwitter()
    {
        this.audioSource.Play();
        this.disableInput = true;
        Globals.TwitterBackTargetScene = "MainMenu";
        StartCoroutine(LoadScene("ConnectTwitter"));
    }

    /// <summary>
    /// Adds tab as a form of selection.
    /// </summary>
    public override void OnUpdate()
    {
        this.versionText.text = Application.version;
        EventSystem.current.sendNavigationEvents = !disableInput;
        UpdateCursorLocking();
        if (disableInput)
        {
            return;
        }

        if(EventSystem.current.currentSelectedGameObject)
        {
            var selected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            
            if (CrossPlatformInputManager.GetButtonDown("MenuSelect"))
            {
                selected = selected.FindSelectableOnDown();
                if(selected != null)
                {
                    selected.Select();
                }
            }
        }
        else
        {
            if (CrossPlatformInputManager.GetButtonDown("Vertical") ||
                CrossPlatformInputManager.GetButtonDown("MenuSelect"))
                {
                    if (this.LoginMenu.isActiveAndEnabled)
                    {
                        this.EmailField.Select();
                    }
                    else
                    {
                        this.MainMenu.PlayButton.Select();
                    }
                }
        }

        if (MouseWasMoved())
        {
            ResetDemoTimer();
            DeselectAllItems();
        }

        if (AnyButtonPressed())
        {
            ResetDemoTimer();
        }
        
        if(!String.IsNullOrEmpty(this.EmailField.text) &&
            !String.IsNullOrEmpty(this.PasswordField.text))
        {
            this.LoginButton.interactable = true;
        }
        else
        {
            this.LoginButton.interactable = false;
        }

        if (Time.time - this.lastInputTime >= this.DemoStartTimeout)
        {
            SceneManager.LoadScene("Demo");
        }
    }

    private void CanPingLeaderboard()
    {        
        this.leaderboard.PingLeaderboard((pong) => {
            if(pong != "OK")
            {
                //Display error message.
                this.connectionUpdateText.gameObject.SetActive(true);
                this.MainMenu.PlayButton.gameObject.SetActive(false);
                this.MainMenu.TutorialButton.gameObject.SetActive(false);
                this.MainMenu.HighScoresButton.gameObject.SetActive(false);
                this.MainMenu.CreditsButton.gameObject.SetActive(false);
                this.MainMenu.LogoutButton.gameObject.SetActive(false);
            }
        });
    }

    private void CheckIfSessionsPaused()
    {                
        this.leaderboard.CheckIfPaused((isPaused) => {
            this.MainMenu.PlayButton.gameObject.SetActive(!isPaused);
            this.MainMenu.TutorialButton.gameObject.SetActive(!isPaused);
            this.MainMenu.HighScoresButton.gameObject.SetActive(!isPaused);
            this.MainMenu.CreditsButton.gameObject.SetActive(!isPaused);
            this.MainMenu.LogoutButton.interactable = !isPaused;
            this.SessionsPausedText.gameObject.SetActive(isPaused);

            if(isPaused)
            {
                this.MainMenu.QuitButton.Select();
            }

            AudioListener.pause = isPaused;
        });
    }

    private void ResetDemoTimer()
    {
        this.lastInputTime = Time.time;
    }

    private void DeselectAllItems()
    {
        GameObject activeEventSystem = GameObject.Find("EventSystem");
        activeEventSystem .GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    
    /// <summary>
    /// Selects the top most selectable item in the scene.
    /// </summary>
    private void SelectTopItem()
    {
        var selectables = Selectable.allSelectables;
        selectables.Sort((item1, item2) => {
            if (item1.transform.position.y > item2.transform.position.y)
            {
                return -1;
            } 
            else if (item1.transform.position.y < item2.transform.position.y)
            {
                return 1;
            } 
            
            return 0;
        });
        selectables[0].Select();
    }
    
    /// <summary>
    /// Sets the logged in text.
    /// </summary>
    private void SetWelcomeText()
    {
        // This Method referenced Moz internal code and so areas have been redacted
        string displayName = ""; // Redacted
        if(string.IsNullOrEmpty(displayName))
        {
            this.WelcomeText.text = "Player";
        }
        else
        {
            this.WelcomeText.text = "Player " + displayName;
        }	
    }

    public void EasteEggCallout()
    {
        if(this.calloutManager != null)
        {
            if (!this.calloutManager.IsCalloutPlaying)
            {
                this.calloutManager.PerformCallout(1, "rogerlovesyou", "spaceballs", "mathematical", "helpusroger", "fiveisalive",
                    "doabarrelroll", "droid", "bladerunner");
            }
        }
    }

    public void MoveFocusPassword()
    {
        this.PasswordField.Select();
    }
}
