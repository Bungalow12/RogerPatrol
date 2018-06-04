using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class TwitterController : BaseController 
{
	[SerializeField]
	protected Text ConnectButtonText;

	[SerializeField]
	protected GameObject connectTwitterMenu;

	[SerializeField]
	protected GameObject enterPinMenu;

	[SerializeField]
	protected InputField pinField;

	[SerializeField]
	protected string appKey;

	[SerializeField]
	protected string appSecret;

	[SerializeField]
	private AudioSource audioSource;

    [SerializeField]
    private Text reportBackField;

	Twitter.RequestTokenResponse requestTokenResponse;
    Twitter.AccessTokenResponse accessTokenResponse;

	public override void OnStart()
	{
		LoadTwitterUserInfo();
	}

	public override void OnUpdate()
	{
		if (!string.IsNullOrEmpty(accessTokenResponse.ScreenName))
		{
			this.ConnectButtonText.text = "Reconnect";
		}

		else
		{
			this.ConnectButtonText.text = "Connect";
		}

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
                    if (this.enterPinMenu.activeSelf)
                    {
                        this.pinField.Select();
                    }
                    else
                    {
					    this.ConnectButtonText.GetComponentInParent<Button>().Select();
                    }
                }
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
	
	public void ConnectTwitter()
	{	
		StartCoroutine(Twitter.API.GetRequestToken(this.appKey, this.appSecret,
													new Twitter.RequestTokenCallback(this.OnRequestTokenCallback)));
	}

	public void UsePin()
	{
        if(!string.IsNullOrEmpty(this.pinField.text))
        {            
            StartCoroutine(Twitter.API.GetAccessToken(this.appKey, this.appSecret, this.requestTokenResponse.Token, this.pinField.text,
                        new Twitter.AccessTokenCallback(this.OnAccessTokenCallback)));
        }
	}

    public void GoBack()
    {
        this.audioSource.Play();
        this.disableInput = true;
        StartCoroutine(LoadScene(Globals.TwitterBackTargetScene));
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

    void OnRequestTokenCallback(bool success, Twitter.RequestTokenResponse response)
    {
        if (success)
        {
            string log = "OnRequestTokenCallback - succeeded";
            log += "\n    Token : " + response.Token;
            log += "\n    TokenSecret : " + response.TokenSecret;
            Debug.Log(log);

            requestTokenResponse = response;

            Twitter.API.OpenAuthorizationPage(response.Token);
			this.connectTwitterMenu.SetActive(false);
			this.enterPinMenu.SetActive(true);
        }
        else
        {
            Debug.Log("OnRequestTokenCallback - failed.");
        }
    }

    void OnAccessTokenCallback(bool success, Twitter.AccessTokenResponse response)
    {
        this.enterPinMenu.SetActive(false);
        this.reportBackField.gameObject.SetActive(true);
        this.reportBackField.text = success ? "Twitter Connected!" : "Please try again...";
        Invoke("ClearAndReset", 1.0f);

        if (success)
        {
            string log = "OnAccessTokenCallback - succeeded";
            log += "\n    UserId : " + response.UserId;
            log += "\n    ScreenName : " + response.ScreenName;
            log += "\n    Token : " + response.Token;
            log += "\n    TokenSecret : " + response.TokenSecret;
            Debug.Log(log);
  
            accessTokenResponse = response;

            PlayerPrefs.SetString(Globals.PLAYER_PREFS_TWITTER_USER_ID, response.UserId);
            PlayerPrefs.SetString(Globals.PLAYER_PREFS_TWITTER_USER_SCREEN_NAME, response.ScreenName);
            PlayerPrefs.SetString(Globals.PLAYER_PREFS_TWITTER_USER_TOKEN, response.Token);
            PlayerPrefs.SetString(Globals.PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET, response.TokenSecret);

			this.enterPinMenu.SetActive(false);
            GoBack();
        }
        else
        {
            Debug.Log("OnAccessTokenCallback - failed.");
        }
    }
    
    void ClearAndReset()
    {
        this.reportBackField.text = "";
        this.reportBackField.gameObject.SetActive(false);
        this.enterPinMenu.SetActive(true);
    }
}
