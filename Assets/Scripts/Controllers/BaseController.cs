using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class BaseController : MonoBehaviour 
{
	[SerializeField]
    /// <summary>
    /// The leaderboard communication object.
    /// </summary>
    protected Leaderboard leaderboard;
	
	protected GameObject[] nebulas;

	[SerializeField]
	protected GameObject[] spacedust;

	[SerializeField]
	protected int totalSpaceDust = 0;

	protected bool disableInput = false;
	
	public virtual bool AnyButtonPressed()
	{
		foreach (var key in Enum.GetValues(typeof(KeyCode))) 
		{
			if (Input.GetKey((KeyCode)key))
			{
				return true;
			}
		}

		return false;
	}

	public virtual bool MouseWasMoved()
	{
		 return Input.GetAxis("Mouse X") != 0.0f ||
		   		Input.GetAxis("Mouse Y") != 0.0f;
	}

	public void Start()
	{
		OnStart();
	}

	public virtual void OnStart()
	{
		#if !CABINET_MODE
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		#else
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		#endif
		PlaceSpaceStuff();
	}

	public void Update()
	{
		OnUpdate();
	}

	public virtual void OnUpdate()
	{
		//Do Nothing
	}

	protected virtual void UpdateCursorLocking()
    {
		#if !CABINET_MODE
        if(disableInput)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
		#else
		//TODO: Disable cursor for Cabinet mode. Doing this makes quitting nearly impossible
		Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
		#endif
    }

	/// <summary>
    /// Gets a random location in the world.
    /// </summary>
    /// <returns>A random location in the world.</returns>
    protected Vector3 GetRandomLocation()
    {
        return new Vector3 (UnityEngine.Random.Range (Globals.WorldBoundaries.xMin + 10, Globals.WorldBoundaries.xMax - 10), 
                            UnityEngine.Random.Range (Globals.WorldBoundaries.yMin + 10, Globals.WorldBoundaries.yMax - 10),
                            0.0f);
    }

	protected virtual void PlaceSpaceStuff()
	{
		if(this.nebulas != null)
		{
			foreach(var nebula in this.nebulas)
			{
				Instantiate(nebula, GetRandomLocation(), Quaternion.identity);
			}
		}

		for (int i = 0; i < totalSpaceDust; ++i)
		{
			var spacedust = this.spacedust[UnityEngine.Random.Range(0, this.spacedust.Length)];
			Instantiate(spacedust, GetRandomLocation(), Quaternion.identity);
		}
	}
}
