using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loader : BaseController 
{
	[SerializeField]
	protected string sceneToLoad;
	private AsyncOperation loadingOperation;

	public override void OnStart()
	{
		base.OnStart();
		loadingOperation = SceneManager.LoadSceneAsync(sceneToLoad);
	}
}
