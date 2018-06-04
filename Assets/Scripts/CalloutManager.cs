using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CalloutManager : MonoBehaviour 
{
    /// <summary>
    /// The AudioSource used for the callouts. Prevents competition.
    /// </summary>
    [SerializeField]
    public AudioSource audioSource;

    /// <summary>
    /// The collection of audio clips to be made available.
    /// </summary>
    [SerializeField]
    public List<AudioClip> audioClips;

    /// <summary>
    ///  The internally indexed list of audio clips.
    /// </summary>
    private Dictionary<string, List<AudioClip>> audioClipsByName = new Dictionary<string, List<AudioClip>>();

	// Use this for initialization
	void Start () 
    {
        RebuildAudioClipNameIndex();	
	}

    /// <summary>
    /// Rebuilds the name-based grouping index of known audio clips.
    /// </summary>
    public void RebuildAudioClipNameIndex ()
    {
        // Empty the collection.
        this.audioClipsByName.Clear();

        // Process through the audio clips to build a useful collection of names->clips.
        foreach ( AudioClip calloutClip in this.audioClips )
        {
            var currName = calloutClip.name;
            var eventName = currName.Split( new char[] { '_' }, 2 )[0];

            if (this.audioClipsByName.ContainsKey(eventName) == false)
            {
                this.audioClipsByName.Add(eventName, new List<AudioClip>());
            }

            List<AudioClip> currClips = this.audioClipsByName[ eventName ];
            currClips.Add( calloutClip );
        }
    }

    /// <summary>
    /// Given a probability and a list of audio clips, first decide if ANY clip is to be played, then choose one of the items in the list randomly.
    /// </summary>
    /// <param name="chanceToPlay">Chance to play.</param>
    /// <param name="calloutNames">Callout group names.</param>
    public void PerformCallout( int chanceToPlay, params string[] calloutNames )
    {
        if ( (int)( Random.Range(0, 100) ) <= chanceToPlay)
        {
            List<AudioClip> acceptableClips = new List<AudioClip>();
            foreach (string calloutName in calloutNames)
            {
                acceptableClips.AddRange(this.audioClipsByName[calloutName]);
            }

            var countOfClips = acceptableClips.Count;
            audioSource.clip = acceptableClips[ (int) ( Random.Range(0, countOfClips) ) ];
            audioSource.Play();
        }
    }

    /// <summary>
    ///  Method to allow direct access to the list of clips, when you aren't trying to play a random sound amongst a set.
    /// </summary>
    /// <param name="index">Index in audioClips.</param>
    public void PlayClipByIndex( int index )
    {
        // Not sure this is great, but if somebody asks us to play beyond our collection, ignore it quietly.
        if (index >= this.audioClips.Count)
        {
            return;
        }
            

        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    public bool IsCalloutPlaying
    {
        get
        {
            return this.audioSource.isPlaying;
        }
    }
}
