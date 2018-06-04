using UnityEngine;
using System.Collections;

public class TutorialTarget : ActiveObject 
{
	[SerializeField]
	protected string[] weaknessTags;

	/// <summary>
    /// Handles the collision with other game objects.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    protected override void HandleCollision(Collision2D collision)
    {
		foreach (string weakness in this.weaknessTags)
		{
			if(collision.collider.tag == weakness)
			{
				Destroy(this.gameObject);
			}
		}
	}

	/// <summary>
    /// Handles the collision with triggers.
    /// </summary>
    /// <param name="collider">The detected trigger collision.</param>
    protected override void HandleTriggers(Collider2D collider)
    {
		foreach (string weakness in this.weaknessTags)
		{
			if(collider.tag == weakness)
			{
				Destroy(this.gameObject);
			}
		}
	}

	/// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collision">The detected collision.</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

	/// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collider">The detected collision.</param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        HandleTriggers(collider);
    }
}
