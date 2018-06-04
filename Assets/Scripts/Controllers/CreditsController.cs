using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class CreditsController : BaseController
{
    [SerializeField]
    private Text title;

    [SerializeField]
    private Text members;

    private Credits credits;

    [SerializeField]
    private CreditsText creditsText;

    private int currentSection = 0;

    private Coroutine exitCoroutine;

    public bool IsOver
    {
        get
        {
            return this.currentSection == this.credits.sections.Length - 1;
        }
    }

    public override void OnStart()
    {
        #if !UNITY_WEBGL
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        #endif

        #if OFFLINE_MODE
        TextAsset credits = Resources.Load("Credits") as TextAsset;
        this.credits = JsonUtility.FromJson<Credits>(credits.text);
        this.title.text = this.credits.sections[this.currentSection].title;
        this.members.text = string.Join(Environment.NewLine, this.credits.sections[this.currentSection].members);
        this.creditsText.AnimationSpeed = this.creditsText.AnimationLength / this.credits.sections[this.currentSection].display_time;
        #else
        leaderboard.GetCredits((creditsData) =>{
            this.credits = creditsData;
            this.title.text = this.credits.sections[this.currentSection].title;
            this.members.text = string.Join(Environment.NewLine, this.credits.sections[this.currentSection].members);
            this.creditsText.AnimationSpeed = this.creditsText.AnimationLength / this.credits.sections[this.currentSection].display_time;
        });
        #endif
    }
    
    // Update is called once per frame
    public override void OnUpdate () 
    {
        if (AnyButtonPressed())
        {
            if(this.exitCoroutine != null)
            {
                StopCoroutine(this.exitCoroutine);
            }

            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ShowNextSection()
    {
        this.currentSection = Mathf.Min(this.currentSection + 1, this.credits.sections.Length - 1);
        this.title.text = this.credits.sections[this.currentSection].title;
        this.members.text = string.Join(Environment.NewLine, this.credits.sections[this.currentSection].members);

        float animationSpeed = 1.0f;
        if (this.currentSection < this.credits.sections.Length - 1)
        {
            animationSpeed = this.creditsText.AnimationLength / this.credits.sections[this.currentSection].display_time;
        }
        else
        {
            this.exitCoroutine = StartCoroutine(DoAutoExit(this.credits.sections[this.currentSection].display_time));
        }

        this.creditsText.AnimationSpeed = animationSpeed;
    }

    private IEnumerator DoAutoExit(float timeTillExit)
    {
        yield return new WaitForSeconds(timeTillExit);
        SceneManager.LoadScene("MainMenu");
    }
}
