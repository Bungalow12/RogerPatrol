using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GameMenuItem : PhysicalObject, ISelectHandler, IPointerEnterHandler
{
    [SerializeField]
    protected AudioClip selectSound;
    
    public void OnSelect(BaseEventData eventData) 
    {
        this.PlaySound(selectSound);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.PlaySound(selectSound);
    }
}
