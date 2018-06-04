using UnityEngine;
using System.Collections;

public class BGScroller : MonoBehaviour {

    [SerializeField]
    private float scrollSpeed;

    private Vector3 startPostion;

    void Start() 
    {
        startPostion = transform.position;
    }

    // Update is called once per frame
    void Update () 
    {
        float newPostion = Mathf.Repeat (Time.time * scrollSpeed, transform.localScale.y);
        transform.position = startPostion + Vector3.down * newPostion;
    }
}
