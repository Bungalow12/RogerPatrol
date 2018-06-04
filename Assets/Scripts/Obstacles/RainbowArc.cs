using UnityEngine;
using System.Collections;

public class RainbowArc : Projectile 
{		
    protected override void OnUpdate()
    {
        Vector3 forward = new Vector2(0.0f, 1.0f); //Up in the local space.
        this.transform.Translate(forward * (this.Speed * Time.deltaTime));
        
        base.OnUpdate();
    }
}
