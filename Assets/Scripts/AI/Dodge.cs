using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The Basic Dodge State for an enemy.
/// </summary>
public class Dodge : BaseEnemyBehavior 
{
    /// <summary>
    /// The asteroids in the danger zone.
    /// </summary>
    private List<Asteroid> asteroids = new List<Asteroid>();

    /// <summary>
    /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state.
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //Get surrounding obstacles
        var collisions = Physics2D.OverlapCircleAll(self.transform.position, self.Detector.radius, LayerMask.GetMask("Asteroid"));

        // Collects only the game objects.
        foreach (var collision in collisions)
        {
            this.asteroids.Add(collision.gameObject.GetComponent<Asteroid>());
        }

        if (this.asteroids.Count > 0)
        {
            //Avoid 
            try
            {
                Asteroid mostDangerous = FindMostDangerous();
                if(mostDangerous != null)
                { 
                    float x = mostDangerous.Velocity.x;
                    float y = mostDangerous.Velocity.y;
                    
                    Vector2 dodge = x > y ? new Vector2(-x, y) : new Vector2(x, -y);
                    dodge.Normalize();
                    self.Body.AddForce(dodge * self.Speed);
                }
            }
            catch(MissingReferenceException)
            {
                //Do nothing to self.
            }
        }

        // Leave the Dodge state.
        animator.SetBool("InDanger", false);
    }

    /// <summary>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        // Leave the Dodge state. This needed to be here it seems the state change in OnStateEnter sometimes was missed...
        animator.SetBool("InDanger", false);
    }

    /// <summary>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </summary>
    /// <param name="animator">Animator.</param>
    /// <param name="stateInfo">State info.</param>
    /// <param name="layerIndex">Layer index.</param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {

    }

    /// <summary>
    /// Finds the most dangerous asteroid.
    /// </summary>
    /// <returns>The most dangerous asteroid.</returns>
    private Asteroid FindMostDangerous()
    {
        float closest = 200.0f;
        float smallestAngle = 360.0f;
        Asteroid mostDangerous = null;

        foreach (var asteroid in this.asteroids)
        {
            if (asteroid != null && asteroid.transform != null)
            {
                // Get the distance and the asteroids angle of movement.
                float distance = Vector3.Distance(asteroid.Position, self.Position);
                float angleOfMovement = Mathf.Atan2(asteroid.Velocity.y, asteroid.Velocity.x) * Mathf.Rad2Deg - 90.0f;

                // Gets the angle of movement between ourself and the asteroid.
                Vector3 vectorBetween = self.Position - asteroid.Position;
                float angleBetween = Mathf.Atan2(vectorBetween.y, vectorBetween.x) * Mathf.Rad2Deg - 90.0f;

                //The "most dangerous" is the closest on a near collision course with us.
                float angleDifference = Mathf.Abs(angleBetween - angleOfMovement);
                if(closest > distance && smallestAngle > angleDifference)
                {
                    closest = distance;
                    smallestAngle = angleDifference;
                    mostDangerous = asteroid;
                }
            }
        }

        return mostDangerous;
    }
}
