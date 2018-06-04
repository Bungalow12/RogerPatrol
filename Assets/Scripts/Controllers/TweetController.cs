using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class TweetController : BaseController 
{
    [SerializeField]
    protected GameObject postUI;

	[SerializeField]
	protected Button connectButton;
    
    [SerializeField]
    protected Button postButton;

	[SerializeField]
	protected InputField tweetField;

	[SerializeField]
	protected string appKey;

	[SerializeField]
	protected string appSecret;

	[SerializeField]
	private AudioSource audioSource;

    [SerializeField]
    private Text reportBackText;

	Twitter.RequestTokenResponse requestTokenResponse;
    Twitter.AccessTokenResponse accessTokenResponse;

	public override void OnStart()
	{
		LoadTwitterUserInfo();
        if (string.IsNullOrEmpty(accessTokenResponse.ScreenName))
		{
            this.postButton.gameObject.SetActive(false);
            this.tweetField.interactable = false;
			this.connectButton.gameObject.SetActive(true);
		}
        
        this.tweetField.text = string.Format(this.tweetField.text, Mathf.FloorToInt(Globals.Score));
	}

	public override void OnUpdate()
	{
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
					this.connectButton.Select();
                }
        }

        if(string.IsNullOrEmpty(this.tweetField.text) ||
           string.IsNullOrEmpty(accessTokenResponse.ScreenName))
        {
            this.postButton.interactable = false;
        }
        else
        {
            this.postButton.interactable = true;
        }

        if (MouseWasMoved())
        {
            DeselectAllItems();
        }
	}

	private void DeselectAllItems()
    {
        GameObject activeEventSystem = GameObject.Find("EventSystem");
        activeEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
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

	private IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
	
	public void PostTweet()
	{	
        if(!string.IsNullOrEmpty(this.tweetField.text))
        {
		    StartCoroutine(Twitter.API.PostTweet(this.tweetField.text, this.appKey, this.appSecret, this.accessTokenResponse,
                                                 new Twitter.PostTweetCallback(this.OnPostTweet)));
        }
	}

	public void ConnectTwitter()
	{
		this.audioSource.Play();
        this.disableInput = true;
        Globals.TwitterBackTargetScene = "TweetScore";
        StartCoroutine(LoadScene("ConnectTwitter"));
	}

    public void GoBack()
    {
        this.audioSource.Play();
        this.disableInput = true;
        StartCoroutine(LoadScene("MainMenu"));
    }

	void LoadTwitterUserInfo()
    {
        accessTokenResponse = new Twitter.AccessTokenResponse();

        accessTokenResponse.UserId = PlayerPrefs.GetString(Globals.PLAYER_PREFS_TWITTER_USER_ID);
        accessTokenResponse.ScreenName = PlayerPrefs.GetString(Globals.PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
        accessTokenResponse.Token = PlayerPrefs.GetString(Globals.PLAYER_PREFS_TWITTER_USER_TOKEN);
        accessTokenResponse.TokenSecret = PlayerPrefs.GetString(Globals.PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);

        if (!string.IsNullOrEmpty(accessTokenResponse.Token) &&
            !string.IsNullOrEmpty(accessTokenResponse.ScreenName) &&
            !string.IsNullOrEmpty(accessTokenResponse.Token) &&
            !string.IsNullOrEmpty(accessTokenResponse.TokenSecret))
        {
            string log = "LoadTwitterUserInfo - succeeded";
            log += "\n    UserId : " + accessTokenResponse.UserId;
            log += "\n    ScreenName : " + accessTokenResponse.ScreenName;
            log += "\n    Token : " + accessTokenResponse.Token;
            log += "\n    TokenSecret : " + accessTokenResponse.TokenSecret;
            Debug.Log(log);
        }
    }

    void OnPostTweet(bool success)
    {
        this.postUI.SetActive(false);
        this.reportBackText.gameObject.SetActive(true);
        this.reportBackText.text = success ? "Tweet Complete!" : "Please try again...";
        Invoke("ClearAndReset", 1.0f);
        if (success)
        {
            GoBack();
        }
    }

    void ClearAndReset()
    {
        this.reportBackText.text = "";
        this.reportBackText.gameObject.SetActive(false);
        this.postUI.SetActive(true);
    }
}
