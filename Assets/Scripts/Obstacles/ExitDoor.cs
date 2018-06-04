using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls components making the door.
/// </summary>
public class ExitDoor : PhysicalObject 
{	
    [SerializeField]
    private List<GameObject> doorBlocks;

    public List<GameObject> DoorBlocks
    {
        get
        {
            return this.doorBlocks;
        }
    }
}