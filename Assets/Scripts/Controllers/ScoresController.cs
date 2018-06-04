using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum Filter
{
    Community,
    Mozzer,
    All,
    Local
}

/// <summary>
/// Handles high scores input.
/// </summary>
public class ScoresController : BaseController 
{
    [SerializeField]
    private Text LoadingText;

    [SerializeField]
    private Button[] filterButtons;

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private RankItem[] rankFields;

    [SerializeField]
    private RankItem myRank;

    [SerializeField]
    private GameObject ellipsis;

    private Filter currentFilter;
    
    private AudioSource audioSource;

    /// <summary>
    /// Executes on object start.
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();
        this.audioSource = this.GetComponent<AudioSource>();

        #if CABINET_MODE
        this.filterButtons[(int)Filter.All].gameObject.SetActive(false);
        this.filterButtons[(int)Filter.Local].gameObject.SetActive(true);
        #endif

        #if OFFLINE_MODE
        this.filterButtons[(int)Filter.Community].gameObject.SetActive(false);
        this.filterButtons[(int)Filter.Mozzer].gameObject.SetActive(false);
        this.filterButtons[(int)Filter.All].gameObject.SetActive(false);
        this.filterButtons[(int)Filter.Local].gameObject.SetActive(true);
        var localTransform = this.filterButtons[(int)Filter.Local].GetComponent<RectTransform>();
        localTransform.anchoredPosition = new Vector2(-50.0f, localTransform.anchoredPosition.y); //Center from anchored position
        #endif

        #if CABINET_MODE || OFFLINE_MODE
        ChangeFilter(Filter.Local);
        #else
        ChangeFilter(Filter.Community);
        #endif

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

    public void LockButtons(bool locked)
    {
        for (int i = 0; i < this.filterButtons.Length; ++i)
        {
            if((Filter)i != this.currentFilter)
            {
                this.filterButtons[i].interactable = !locked;
            }
            this.quitButton.interactable = !locked;
        }
    }

    /// <summary>
    /// Sets the filter for the leaderboards.
    /// </summary>
    /// <param name="filterValue">The filter for the leaderboards.</param>
    public void ChangeFilter(string filterValue)
    {		
        Filter filter = (Filter)Enum.Parse(typeof(Filter), filterValue);
        ChangeFilter(filter);
    }

    /// <summary>
    /// Changes the filter for scores.
    /// </summary>
    /// <param name="filter">Filter for the leaderboard.</param>
    public void ChangeFilter(Filter filter)
    {
        this.currentFilter = filter;
        LockButtons(true);
        this.leaderboard.CancelAllRequests();
        this.audioSource.Play();
        HideScores();
        this.LoadingText.gameObject.SetActive(true);

        this.filterButtons[(int)filter].Select();

        if(filter == Filter.Local)
        {
            UpdateLocalScores();
        }
        else
        {
            this.leaderboard.GetScores(filter, 10, UpdateScores);
        }
    }

    public void UpdateLocalScores()
    {
        LockButtons(false);
        this.LoadingText.gameObject.SetActive(false);
        for (int i = 0; i < rankFields.Length; ++i)
        {
            var score = Globals.HighScores[i];
            this.rankFields[i].gameObject.SetActive(true);
            this.rankFields[i].PlayerName = score.username;
            this.rankFields[i].Score = score.score.ToString();
        }
    }
    
    /// <summary>
    /// Updates the displayed scores.
    /// </summary>
    /// <param name="userScores">The User scores object received.</param>
    public void UpdateScores(UserScoresResponse userScores)
    {
        this.LoadingText.gameObject.SetActive(false);
        var inTop10 = false;	

        for (int i = 0; i < rankFields.Length; ++i)
        {			
            if(i < userScores.content.Length)
            {
                var score = userScores.content[i];
                this.rankFields[i].gameObject.SetActive(true);
                this.rankFields[i].PlayerName = score.username;
                this.rankFields[i].Score = score.score.ToString();

                #if !MOZAUTH_DISABLED
                if(score.id.ToString() == Mozauth.HeaderInfo.id)
                {
                    this.rankFields[i].IsMine = true;
                    inTop10 = true;
                    LockButtons(false);
                }
                #else
                LockButtons(false);
                #endif

            }
            else
            {
                this.rankFields[i].gameObject.SetActive(true);
                this.rankFields[i].PlayerName = "Roger Mozbot";
                this.rankFields[i].Score = "0";
                this.rankFields[i].IsMine = false;
                this.rankFields[i].Color = Color.white;
                LockButtons(false);
            }					
        }

        #if !MOZAUTH_DISABLED
        if(!inTop10)
        {
            this.leaderboard.GetMyScore((myScore) => 
            {
                var shouldShowMe = (Mozauth.HeaderInfo.admin == (( this.currentFilter == Filter.Mozzer ) ? "1" : "0"));
                if(shouldShowMe && myScore.score != 0)
                {
                    this.ellipsis.SetActive(true);
                    this.myRank.gameObject.SetActive(true);
                    this.myRank.Rank = (myScore.rank + 1).ToString();
                    this.myRank.PlayerName = Mozauth.HeaderInfo.display_name ?? Mozauth.HeaderInfo.full_name;
                    this.myRank.Score = myScore.score.ToString();
                    this.myRank.IsMine = true;
                }

                LockButtons(false);
            });
        }
        #endif
    }

    /// <summary>
    /// Hide all scores.
    /// </summary>
    private void HideScores()
    {
        this.ellipsis.SetActive(false);
        this.myRank.gameObject.SetActive(false);
        
        foreach(var rankItem in this.rankFields)
        {
            rankItem.gameObject.SetActive(false);
        }
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
}
