using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnterInitialsCanvas : MonoBehaviour 
{
	protected int index = 0;

	[SerializeField]
	protected PlayerInitial firstInitial;

	[SerializeField]
	protected PlayerInitial middleInitial;

	[SerializeField]
	protected PlayerInitial lastInitial;

	[SerializeField]
	protected Button submitButton;

	public PlayerInitial FirstInitial
	{
		get
		{
			return this.firstInitial;
		}
	}

	public PlayerInitial MiddleInitial
	{
		get
		{
			return this.middleInitial;
		}
	}

	public PlayerInitial LastInitial
	{
		get
		{
			return this.lastInitial;
		}
	}

	public Button SubmitButton
	{
		get
		{
			return this.submitButton;
		}
	}

	public PlayerInitial GetCurrentInitial()
	{
		PlayerInitial[] initialsByIndex = new PlayerInitial[] {
			this.firstInitial,
			this.middleInitial,
			this.lastInitial
		};
		
		foreach(var initial in initialsByIndex)
		{
			initial.IsCurrent = false;
		}

		if(index > 2)
		{
			this.submitButton.gameObject.SetActive(true);
			this.submitButton.Select();
			return null;
		}

		this.submitButton.gameObject.SetActive(false);		

		initialsByIndex[this.index].IsCurrent = true;

		return initialsByIndex[this.index];
	}

	/// <summary>
    /// Increments the index and returns true if we are done.
    /// </summary>
    /// <returns>True if we have finished entering intials.</returns>
	public PlayerInitial NextInitial()
	{
		++this.index;		
		return GetCurrentInitial();
	}

	/// <summary>
    /// Decrements the index. 
    /// </summary>
	public PlayerInitial PreviousInitial()
	{
		this.index = Mathf.Max(0, this.index - 1);
		return GetCurrentInitial();
	}
}
