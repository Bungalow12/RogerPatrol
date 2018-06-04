using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StatsController : BaseController 
{
    [SerializeField]
    private Button backButton;

    [SerializeField]
    private Button totalButton;

    [SerializeField]
    private Button lastButton;
    
    [SerializeField]
    private Button bestButton;

    [SerializeField]
    private Text statsText;

    [SerializeField]
    private AudioSource audioSource;

	// Use this for initialization
	public override void OnStart () 
    {
        this.totalButton.Select();
        var stats = Globals.PlayerOverallStats.ToString();
        this.statsText.text = String.Format("Total games played: {0}\n{1}", Globals.TotalGamesPlayed, stats);
	}

    /// <summary>
    /// Adds tab as a form of selection.
    /// </summary>
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
    }

    private IEnumerator LoadScene(string sceneName)
    {
        this.disableInput = true;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Backs to main.
    /// </summary>
    public void BackToMain()
    {
        this.audioSource.Play();
        StartCoroutine(LoadScene("MainMenu"));
    }

    public void ShowLast()
    {
        this.audioSource.Play();
        this.lastButton.Select();
        this.statsText.text = Globals.LastPlayStats.ToString();
    }

    public void ShowTotal()
    {
        this.audioSource.Play();
        this.totalButton.Select();
        this.statsText.text = String.Format("Total games played: {0}\n{1}", Globals.TotalGamesPlayed, Globals.PlayerOverallStats.ToString());
    }

    public void ShowBest()
    {
        this.audioSource.Play();
        this.bestButton.Select();
        this.statsText.text = Globals.BestPlayStats.ToString();
    }
}
