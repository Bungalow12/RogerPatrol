using UnityEngine;
using System.Collections;

public class CreditsText : MonoBehaviour 
{
	[SerializeField]
	private CreditsController controller;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AnimationClip animationClip;

	public float AnimationLength
	{
		get
		{
			return this.animationClip.length;
		}
	}

	public float AnimationSpeed
	{
		get
		{
			return this.animator.speed;
		}
		set
		{
			this.animator.speed = value;
		}
	}

	public void ShowNextSection()
	{
		this.controller.ShowNextSection();
	}

	public void CheckForStop()
	{
		if (this.controller.IsOver)
		{
			this.animator.StopPlayback();
		}
	}
}
