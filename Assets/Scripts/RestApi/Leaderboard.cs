using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

/// <summary>
/// Implements communication with the Leaderboard
/// Inherits MonoBehaviour for the Unity StartCoroutine.
/// </summary>
public class Leaderboard : MonoBehaviour 
{
    [SerializeField]
    /// <summary>
    /// The URL to post your score to.
    /// </summary>
    private string baseUrl;

    [SerializeField]
    /// <summary>
    /// The prefix for the endpoint.
    /// </summary>
    private string urlPrefix;

    /// <summary>
    /// The authentication username to use to allow access to the scoreboard server.
    /// </summary>
    [SerializeField]
    private string authenticationUsername;

    /// <summary>
    /// The authentication password to use to allow access to the scoreboard server.
    /// </summary>
    [SerializeField]
    private string authenticationPassword;

    private Dictionary<String, String> BuildAuthenticationHeaders()
    {
        var secretKey = authenticationUsername + ":" + authenticationPassword;
        
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(secretKey));

        return headers;
    }

    /// <summary>
    /// Sends the score to the leaderboard async.
    /// </summary>
    /// <param name="score">The player's high score.</param>
    /// <param name="stats">The player's round statistics</param> 
    /// <param name="callback">The callback accepting the web response.</param>
    public void SendScore(int score, UsageStats stats, Action<UserScoresResponse> callback)
    {
        // This Method referenced Moz internal code and so areas have been redacted
        string cookie = ""; // Redacted
        if (!string.IsNullOrEmpty(cookie))
        {
            UserScoreSubmission scoreSubmission = new UserScoreSubmission();
            scoreSubmission.id = 0; // Redacted
            scoreSubmission.username = "Default User"; // Redacted
            scoreSubmission.score = score;
            scoreSubmission.admin = false; // Redacted
            scoreSubmission.usage = stats;

            var jsonSubmission = JsonUtility.ToJson(scoreSubmission);
            byte[] postBody = Encoding.ASCII.GetBytes(jsonSubmission);

            var authenticationHeaders = BuildAuthenticationHeaders();
            WWW request = new WWW(this.baseUrl + "/" + urlPrefix + "/score", postBody, authenticationHeaders);
       
            StartCoroutine(WaitTillDone(request, callback));
        }
    }

    /// <summary>
    /// Sends the score to the leaderboard async.
    /// </summary>
    /// <param name="score">The player's high score.</param>
    /// <param name="stats">The player's round statistics</param> 
    /// <param name="callback">The callback accepting the web response.</param>
    public void SendStats(int score, UsageStats stats, Action<string> callback)
    {
        UserStatSubmission scoreSubmission = new UserStatSubmission();
        scoreSubmission.score = score;
        scoreSubmission.usage = stats;

        var jsonSubmission = JsonUtility.ToJson(scoreSubmission);
        byte[] postBody = Encoding.ASCII.GetBytes(jsonSubmission);

        var authenticationHeaders = BuildAuthenticationHeaders();
        WWW request = new WWW(this.baseUrl + "/" + urlPrefix + "/arcade/stats", postBody, authenticationHeaders);

        StartCoroutine(WaitTillDoneString(request, callback));
    }

    /// <summary>
    /// Gets the scores from the leaderboard async.
    /// </summary>
    /// <param name="filter">The score filter.</param>
    /// <param name="limit">The total to receive.</param>
    /// <param name="callback">The callback accepting the web response.</param>
    public void GetScores(Filter filter, int limit, Action<UserScoresResponse> callback)
    {                  
        string query = "?limit=" + limit;

        string category;
        switch (filter)
        {
            case Filter.All:
                category = "all";
                break;
            case Filter.Mozzer:
                category = "admin";
                break;
            default:
                category = "community";
                break;
        }

        var authenticationHeaders = BuildAuthenticationHeaders();
        WWW request = new WWW(string.Format("{0}/{1}/scores/{2}{3}", this.baseUrl, urlPrefix, category, query), null, authenticationHeaders);

        StartCoroutine(WaitTillDone(request, callback));
    }

    /// <summary>
    /// Gets the scores from the leaderboard async.
    /// </summary>
    /// <param name="callback">The callback accepting the web response.</param>
    public void GetMyScore(Action<MyScore> callback)
    {
        // This Method referenced Moz internal code and so areas have been redacted
        MyScoreRequest scoreRequest = new MyScoreRequest();
        scoreRequest.id = 0; // Redacted
        scoreRequest.admin = false; // Redacted

        var jsonSubmission = JsonUtility.ToJson(scoreRequest);
        byte[] postBody = Encoding.ASCII.GetBytes(jsonSubmission);

        string cookie = ""; // Redacted
        if (!string.IsNullOrEmpty(cookie))
        {           
            var authenticationHeaders = BuildAuthenticationHeaders();
            WWW request = new WWW(this.baseUrl + "/" + urlPrefix + "/my-score", postBody, authenticationHeaders);

            StartCoroutine(WaitTillDone(request, callback));
        }
    }

    /// <summary>
    /// Gets the credits from the server async.
    /// </summary>
    /// <param name="callback">The callback accepting the web response.</param>
    public void GetCredits(Action<Credits> callback)
    {    
        var authenticationHeaders = BuildAuthenticationHeaders();
        WWW request = new WWW(string.Format("{0}/{1}/credits", this.baseUrl, urlPrefix), null, authenticationHeaders);
        StartCoroutine(WaitTillDone(request, callback));
    }

   
    /// Gets the redeem code from the leaderboard async.
    /// </summary>
    /// <param name="callback">The callback accepting the web response.</param>
    public void GetRedeemCode(int score, Action<string> callback)
    {
        RedeemCodeRequest redeemRequest = new  RedeemCodeRequest {score=score};

        var jsonSubmission = JsonUtility.ToJson(redeemRequest);
        byte[] postBody = Encoding.ASCII.GetBytes(jsonSubmission);
         
        var authenticationHeaders = BuildAuthenticationHeaders();
        WWW request = new WWW(this.baseUrl + "/" + urlPrefix + "/arcade/record", postBody, authenticationHeaders);
        StartCoroutine(WaitTillDone(request, callback));
    }

    /// <summary>
    /// Checks if the Session Paused flag is set.
    /// </summary>
    /// <param name="callback">The callback accepting the web response.</param>
    public void CheckIfPaused(Action<bool> callback)
    {
        var authenticationHeaders = BuildAuthenticationHeaders();
        WWW request = new WWW(this.baseUrl + "/" + urlPrefix + "/paused", null, authenticationHeaders);
        StartCoroutine(WaitTillDone(request, callback));
    }

    /// <summary>
    /// Checks if the leaderboard is up.
    /// </summary>
    /// <param name="callback">The callback accepting the web response.</param>
    public void PingLeaderboard(Action<string> callback)
    {
        var authenticationHeaders = BuildAuthenticationHeaders();
        WWW request = new WWW(this.baseUrl + "/ping", null, authenticationHeaders);
        StartCoroutine(WaitTillDoneString(request, callback));
    }

    /// <summary>
    /// Waits for the request to finish.
    /// </summary>
    /// <param name="request">The WWW request object to wait for.</param>
    /// <param name="callback">The callback to call upon completion.</param>
    /// <returns>The Enumerator for the coroutine.</returns>
    public IEnumerator WaitTillDone(WWW request, Action<UserScoresResponse> callback)
    {
        yield return new WaitUntil(() => request.isDone);
        if (callback != null)
        {
            if(!String.IsNullOrEmpty(request.text))
            {
                var scores = JsonUtility.FromJson<UserScoresResponse>(request.text);
                callback(scores);
            }
            else
            {
                callback(null);
            }
        }
    }

    /// <summary>
    /// Waits for the request to finish.
    /// </summary>
    /// <param name="request">The WWW request object to wait for.</param>
    /// <param name="callback">The callback to call upon completion.</param>
    /// <returns>The Enumerator for the coroutine.</returns>
    public IEnumerator WaitTillDone(WWW request, Action<MyScore> callback)
    {
        yield return new WaitUntil(() => request.isDone);
        if (callback != null)
        {
            var score = JsonUtility.FromJson<MyScoreResponse>(request.text).content;
            callback(score);
        }
    }

    /// <summary>
    /// Waits for the request to finish.
    /// </summary>
    /// <param name="request">The WWW request object to wait for.</param>
    /// <param name="callback">The callback to call upon completion.</param>
    /// <returns>The Enumerator for the coroutine.</returns>
    public IEnumerator WaitTillDone(WWW request, Action<Credits> callback)
    {
        yield return new WaitUntil(() => request.isDone);

        if (callback != null)
        {
            try
            {
                var credits = JsonUtility.FromJson<CreditsResponse>(request.text).content;
                callback(credits);
            }
            catch
            {
                Debug.Log("GetCredits resulted in an error.");
                Debug.Log(request.error);
                callback(null);
            }
        }
    }

    /// <summary>
    /// Waits for the request to finish.
    /// </summary>
    /// <param name="request">The WWW request object to wait for.</param>
    /// <param name="callback">The callback to call upon completion.</param>
    /// <returns>The Enumerator for the coroutine.</returns>
    public IEnumerator WaitTillDone(WWW request, Action<string> callback)
    {
        yield return new WaitUntil(() => request.isDone);
        if (callback != null)
        {
            var code = JsonUtility.FromJson<RedeemCodeResponse>(request.text).content;
            callback(code);
        }
    }

    /// <summary>
    /// Waits for the request to finish.
    /// </summary>
    /// <param name="request">The WWW request object to wait for.</param>
    /// <param name="callback">The callback to call upon completion.</param>
    /// <returns>The Enumerator for the coroutine.</returns>
    public IEnumerator WaitTillDone(WWW request, Action<bool> callback)
    {
        yield return new WaitUntil(() => request.isDone);
        if (callback != null)
        {
            var isPaused = JsonUtility.FromJson<SessionPausedResponse>(request.text).content;
            callback(isPaused);
        }
    }

    /// <summary>
    /// Waits for the request to finish.
    /// </summary>
    /// <param name="request">The WWW request object to wait for.</param>
    /// <param name="callback">The callback to call upon completion.</param>
    /// <returns>The Enumerator for the coroutine.</returns>
    public IEnumerator WaitTillDoneString(WWW request, Action<string> callback)
    {
        yield return new WaitUntil(() => request.isDone);
        if (callback != null)
        {
            callback(request.text);
        }
    }

    public void CancelAllRequests()
    {
        StopAllCoroutines();
    }
}

/// <summary>
/// Individual User Score serialized from a JSON string.
/// </summary>
[Serializable]
public class UserScore
{
    public int rank;
    public int id;
    public string username;
    public int score;
}

/// <summary>
/// Individual User Score Submission for serialization to a JSON string.
/// </summary>
[Serializable]
public class UserScoreSubmission
{
    public bool admin;
    public int id;
    public string username;
    public int score;
    public UsageStats usage;
}

/// <summary>
/// My Score serialized from a JSON string.
/// </summary>
[Serializable]
public class MyScore
{
    public int rank;
    public int score;
}

/// <summary>
/// My score response serialized from a JSON string.
/// </summary>
[Serializable]
public class MyScoreResponse
{
    public MyScore content;
    public string status;
}

/// <summary>
/// My Score Request for serialization to a JSON string.
/// </summary>
[Serializable]
public class MyScoreRequest
{
    public bool admin;
    public int id;
}

/// <summary>
/// List of User Score response objects serialized from a JSON string.
/// </summary>
[Serializable]
public class UserScoresResponse
{
    public UserScore[] content;
    public string status;
}

/// <summary>
/// Object representing the JSON for 1 section of the credits.
/// </summary>
[Serializable]
public class CreditSection
{
    public float display_time;
    public string title;
    public string[] members;
}

/// <summary>
/// Object representing the full credits.
/// </summary>
[Serializable]
public class Credits
{
    public CreditSection[] sections;
}

/// <summary>
/// The Credits request response object.
/// </summary>
[Serializable]
public class CreditsResponse
{
    public Credits content;
    public string status;
}

/// <summary>
/// Redeem Code request from current score for serialization to a JSON string.
/// </summary>
[Serializable]
public class RedeemCodeRequest
{
    public int score;
}

/// <summary>
/// Redeem Code response for serialization from a JSON string
/// </summary>
[Serializable]
public class RedeemCodeResponse
{
    public string content;
    public string status;
}

/// <summary>
/// Session Paused response for serialization from a JSON string.
/// </summary>
[Serializable]
public class SessionPausedResponse
{
    public bool content;
    public string status;
}

[Serializable]
public class UsageStats
{
    #if CABINET_MODE
    public string platform = "Cabinet";
    #elif UNITY_STANDALONE_OSX
    public string platform = "OSX";
    #elif UNITY_STANDALONE_WIN
    public string platform = "Windows";
    #endif

    public float playTime;
    public int shotsFired;
    public int enemiesKilled;
    public int asteroidsDestroyed;
    public int powerUpsCollected;
    public int tagfeeCannonShots;
    public int missileShots;
    public int bombsDropped;
    public int uturnsMade;
    public bool userClickedSignup;

    #if OFFLINE_MODE
    public static UsageStats operator+(UsageStats left, UsageStats right)
    {
        UsageStats combined = new UsageStats {
            playTime = left.playTime + right.playTime,
            shotsFired = left.shotsFired + right.shotsFired,
            enemiesKilled = left.enemiesKilled + right.enemiesKilled,
            asteroidsDestroyed = left.asteroidsDestroyed + right.asteroidsDestroyed,
            powerUpsCollected = left.powerUpsCollected + right.powerUpsCollected,
            tagfeeCannonShots = left.tagfeeCannonShots + right.tagfeeCannonShots,
            missileShots = left.missileShots + right.missileShots,
            bombsDropped = left.bombsDropped + right.bombsDropped,
            uturnsMade = left.uturnsMade + right.uturnsMade,
            userClickedSignup = left.userClickedSignup || right.userClickedSignup
        };

        return combined;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(playTime);
        writer.Write(shotsFired);
        writer.Write(enemiesKilled);
        writer.Write(asteroidsDestroyed);
        writer.Write(powerUpsCollected);
        writer.Write(tagfeeCannonShots);
        writer.Write(missileShots);
        writer.Write(bombsDropped);
        writer.Write(uturnsMade);
        writer.Write(userClickedSignup);
    }

    public static UsageStats Read(BinaryReader reader)
    {
        try
        {
            UsageStats stats = new UsageStats {
                playTime = reader.ReadSingle(),
                shotsFired = reader.ReadInt32(),
                enemiesKilled = reader.ReadInt32(),
                asteroidsDestroyed = reader.ReadInt32(),
                powerUpsCollected = reader.ReadInt32(),
                tagfeeCannonShots = reader.ReadInt32(),
                missileShots = reader.ReadInt32(),
                bombsDropped = reader.ReadInt32(),
                uturnsMade = reader.ReadInt32(),
                userClickedSignup = reader.ReadBoolean()
            };

            return stats;
        }
        catch
        {
            return new UsageStats();
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        TimeSpan totalTime = TimeSpan.FromSeconds(playTime);
        builder.AppendLine(String.Format("Time played: {0:D2}:{1:D2}:{2:D2}:{3:D3}", 
            totalTime.Hours, 
            totalTime.Minutes, 
            totalTime.Seconds, 
            totalTime.Milliseconds));
        builder.AppendLine("Shots fired: " + shotsFired);
        builder.AppendLine("Enemies defeated: " + enemiesKilled);
        builder.AppendLine("Asteroids destroyed: " + asteroidsDestroyed);
        builder.AppendLine("Powerups collected: " + powerUpsCollected);
        builder.AppendLine("TAGFEE Cannon blasts unleashed: " + tagfeeCannonShots);
        builder.AppendLine("Missiles fired: " + missileShots);
        builder.AppendLine("Bombs dropped: " + bombsDropped);
        builder.AppendLine("U-Turns performed: " + uturnsMade);

        return builder.ToString();
    }
    #endif
}

/// <summary>
/// Individual User Score Submission for serialization to a JSON string.
/// </summary>
[Serializable]
public class UserStatSubmission
{
    public int score;
    public UsageStats usage;
}
