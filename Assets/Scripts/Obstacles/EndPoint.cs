using UnityEngine;
using System.Collections;

public class EndPoint : MonoBehaviour 
{
	[SerializeField]
	private TutorialController sceneController;

	/// <summary>
    /// Handles Trigger Enter event.
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.tag == "Player")
		{
			sceneController.GotoNextScene();
		}
	}
}
