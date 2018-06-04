using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityStandardAssets.CrossPlatformInput;

public class TutorialController : BaseController 
{
	[SerializeField]
    /// <summary>
    /// Reference to the game message UI.
    /// </summary>
    private Text gameMessage;

    [SerializeField]
    /// <summary>
    /// The player.
    /// </summary>
    private Player player;

    /// <summary>
    /// The reference to the BG music player.
    /// </summary>
    [SerializeField]
    protected AudioSource bgMusicAudioSource;

    /// <summary>
    /// The callout manager for this tutorial.
    /// </summary>
    [SerializeField]
    protected CalloutManager calloutManager;

    /// <summary>
    /// Reference to the audio source for the door opening sound.
    /// </summary>
    [SerializeField]
    protected AudioSource doorAudioSource;

    /// <summary>
    /// Reference to the audio source for the goal reached sound.
    /// </summary>
    [SerializeField]
    protected AudioSource goalAudioSource;

    [SerializeField]
    protected GameObject[] Objectives;

    [SerializeField]
    protected ExitDoor exitDoor;

    [SerializeField]
    protected float exitOpenRate = 0.1f;

    [SerializeField]
    protected string NextScene;

    [SerializeField]
    protected string[] tutorialText;

    [SerializeField]
    protected TypingText tutorialTextBox;

    [SerializeField]
    private bool doorOpening = false;

    // Use this for initialization
    public override void OnStart () 
    {      
        base.OnStart();

        #if !UNITY_WEBGL
        Cursor.visible = false;
        #endif
        
        Invoke("DisplayTutorialText", 1.0f);
        StartCoroutine("PlayTutorialVoiceover"); 

        if (this.bgMusicAudioSource != null)
        {
            DontDestroyOnLoad(this.bgMusicAudioSource);
        }
    }
    
    void ClearMessage()
    {
        this.gameMessage.text = "";
        this.gameMessage.gameObject.SetActive(false);
    }
    

    public override void OnUpdate()
    {
        // Go back to main menu on button press.
        if ((Input.GetKey(KeyCode.Escape)))
        {
            GotoMenu();
        }

        bool objectivesComplete = true;
        foreach (var objective in this.Objectives)
        {
            if(objective != null)
            {
                objectivesComplete = false;
            }
        }

        if (objectivesComplete)
        {
            if(!this.doorOpening)
            {
                this.doorAudioSource.Play();
                StartCoroutine(OpenDoor());
            }
        }
    }

    /// <summary>
    /// Goes to the menu scene.
    /// </summary>
    public void GotoMenu()
    {
        var bgMusic = GameObject.Find("BG Music");
        Destroy(bgMusic);
        PlayerPrefs.SetInt("Finished Tutorial", 1);
        PlayerPrefs.Save();

        if(this.doorAudioSource != null && this.doorAudioSource.isPlaying)
        {
            this.doorAudioSource.Stop();
        }
        StopAllCoroutines();
        this.tutorialTextBox.Stop();

        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeScene()
    {
        if(this.doorAudioSource != null && this.doorAudioSource.isPlaying)
        {
            this.doorAudioSource.Stop();
        }
        StopAllCoroutines();
        this.tutorialTextBox.Stop();
        SceneManager.LoadScene(this.NextScene);
    }

    public virtual void GotoNextScene()
    {
        if (this.NextScene == "MainMenu")
        {
            var bgMusic = GameObject.Find("BG Music");
            Destroy(bgMusic);
            PlayerPrefs.SetInt("Finished Tutorial", 1);
            PlayerPrefs.Save();
        }

        this.goalAudioSource.Play();
        this.gameMessage.gameObject.SetActive(true);
        this.gameMessage.text = "Good job!";
        Invoke("ChangeScene", 4.0f);
    }

    public void DisplayTutorialText()
    {
        if(this.tutorialText != null && this.tutorialText.Length > 0)
        {
            #if CABINET_MODE
            for (int i = 0; i < this.tutorialText.Length; ++i)
            {
                this.tutorialText[i] = this.tutorialText[i].Replace("[SPACEBAR]", "FIRE BUTTON");
                this.tutorialText[i] = this.tutorialText[i].Replace("[SHIFT]", "SPECIAL BUTTON");
            }
            #endif
            this.tutorialTextBox.Show(() => {
                this.tutorialTextBox.TypeText(this.tutorialText[0]);
            });            
        }
    }

    IEnumerator PlayTutorialVoiceover()
    {
        // Wait 1 second
        yield return new WaitForSeconds(1.0f);

        // Now iterate the clips we have.
        for( int i = 0; i < this.calloutManager.audioClips.Count; i++ )
        {
            this.calloutManager.PlayClipByIndex( i );

            // Wait until the current clip is finished, plus 1 second.
            yield return new WaitWhile( () => this.calloutManager.audioSource.isPlaying );
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator OpenDoor()
    {
        this.doorOpening = true;

        int halfCount = this.exitDoor.DoorBlocks.Count / 2;
        for (int i = 0; i < halfCount; ++i)
        {

            int leftIndex = (halfCount - 1) - i;
            int rightIndex = halfCount + i;
            this.exitDoor.DoorBlocks[leftIndex].SetActive(false);
            this.exitDoor.DoorBlocks[rightIndex].SetActive(false);
            yield return new WaitForSeconds(this.exitOpenRate);
        }

        this.doorAudioSource.Stop();
    }
}