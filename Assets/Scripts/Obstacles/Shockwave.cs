using UnityEngine;
using System.Collections;

public class Shockwave : Explosion 
{	
    [SerializeField]
    protected CircleCollider2D circleCollider;
    
    [SerializeField]
    private float maxDetonationRadius = 20.0f;
    
    [SerializeField]
    private float radialExpansion = 5.0f;
        
    protected override void OnUpdate()
    {
        if(this.circleCollider != null)
        {
            this.circleCollider.radius = Mathf.Min(this.circleCollider.radius + (Time.time - this.startTime) * this.radialExpansion, this.maxDetonationRadius);
        }
        
        base.OnUpdate();
    }
}
