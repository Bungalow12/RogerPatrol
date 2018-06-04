using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashController : BaseController 
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private Animator animator;

	IEnumerator DoPlayWaitMoveOn()
	{
		this.audioSource.Play();
		yield return new WaitWhile( () => this.audioSource.isPlaying );
		this.animator.SetTrigger("FadeOut");
	}

	public void PlaySound()
	{
		StartCoroutine(DoPlayWaitMoveOn());
	}

	public void GotoMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
