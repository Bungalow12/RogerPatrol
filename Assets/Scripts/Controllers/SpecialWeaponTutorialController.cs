using UnityEngine;
using System.Collections;

public class SpecialWeaponTutorialController : TutorialController
{
	[SerializeField]
	protected GameObject[] advanceTextItems;

	protected bool messageChanged = false;

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (!messageChanged && ReadyToAdvance())
		{
			this.messageChanged = true;
			this.tutorialTextBox.TypeText(this.tutorialText[1]);
		}
	}

	private bool ReadyToAdvance()
	{
		foreach(var gameObject in this.advanceTextItems)
		{
			if(gameObject != null)
			{
				return false;
			}
		}
		return true;
	}
}
