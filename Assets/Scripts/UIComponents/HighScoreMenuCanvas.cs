using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreMenuCanvas : MonoBehaviour 
{
    [SerializeField]
    private Button mozzerButton;

    [SerializeField]
    private Button communityButton;

    [SerializeField]
    private Button allButton;

    [SerializeField]
    private Button backButton;

    public Button MozzerButton
    {
        get
        {
            return this.mozzerButton;
        }
    }

    public Button CommunityButton
    {
        get
        {
            return this.communityButton;
        }
    }

    public Button AllButton
    {
        get
        {
            return this.allButton;
        }
    }

    public Button BackButton
    {
        get
        {
            return this.backButton;
        }
    }
}
