using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Globals.
/// </summary>
public static class Globals
{
    public const string PLAYER_PREFS_TWITTER_USER_ID = "TwitterUserID";
    public const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME = "TwitterUserScreenName";
    public const string PLAYER_PREFS_TWITTER_USER_TOKEN = "TwitterUserToken";
    public const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";
    
    /// <summary>
    /// The world boundaries.
    /// </summary>
    private static Rect worldBoundaries = new Rect (-100, -100, 200, 200);

    /// <summary>
    /// The player's score.
    /// </summary>
    private static float score = 0.0f;

    private static UsageStats playerOverallStats;
    private static UsageStats lastPlayStats;
    private static UsageStats bestPlayStats;

    public static int TotalGamesPlayed
    {
        get;
        set;
    }

    public static int PlayersHighestScore
    {
        get;
        set;
    }

    /// <summary>
    /// The players current level.
    /// </summary>
    private static float level = 0.0f;

    private static List<UserScore> highScores;

    /// <summary>
    /// The number of points that constitute a level of experience. Used by ResponsiveValue.
    /// </summary>
    public static float pointsOffset = 1000;
    public static float pointsMultiplier = 1.5f;

    public static string TwitterBackTargetScene = "MainMenu";

    public static bool LockScore
    {
        get;
        set;
    }

    #if OFFLINE_MODE
    public static UsageStats PlayerOverallStats
    {
        get
        {
            if(playerOverallStats == null)
            {    
                LoadStats();
            }
            return playerOverallStats;
        }
        set
        {
            playerOverallStats = value;
        }
    }

    public static UsageStats LastPlayStats
    {
        get
        {
            if(lastPlayStats == null)
            {    
                LoadStats();
            }
            return lastPlayStats;
        }
        set
        {
            lastPlayStats = value;
        }
    }

    public static UsageStats BestPlayStats
    {
        get
        {
            if(bestPlayStats == null)
            {    
                LoadStats();
            }
            return bestPlayStats;
        }
        set
        {
            bestPlayStats = value;
        }
    }
    #endif

    /// <summary>
    /// Gets the world boundaries.
    /// </summary>
    /// <value>The world boundaries.</value>
    public static Rect WorldBoundaries
    {
        get
        {
            return worldBoundaries;
        }
    }

    /// <summary>
    /// Gets and sets the player's score.
    /// </summary>
    public static float Score
    {
        get
        {
            return score;
        }

        set
        {
            if (LockScore)
            {
                return;
            }

            // Recalculate level when score is updated.
            var oper1 = (value / pointsOffset) + 1;
            level = Mathf.Log(oper1, pointsMultiplier);

            score = value;
        }
    }

    public static float Level
    {
        get
        {
            return level;
        }
    }

    public static List<UserScore> HighScores
    {
        get
        {
            if (highScores == null)
            {
                bool loaded = false;
                #if PERSIST_SCORES
                loaded = LoadHighScores();
                #endif

                if(!loaded)
                {
                    highScores = new List<UserScore>(){
                        new UserScore {rank = 0, username = "JON", score = 10000}, // Jonathan
                        new UserScore {rank = 1, username = "KRK", score = 9000},  // Jeff
                        new UserScore {rank = 2, username = "RGR", score = 8000},  // Roger
                        new UserScore {rank = 3, username = "MOZ", score = 7000},  // Moz
                        new UserScore {rank = 4, username = "S99", score = 6000},  // Sarah Bird
                        new UserScore {rank = 5, username = "SRF", score = 5000},  // Rand Fishkin
                        new UserScore {rank = 6, username = "DUD", score = 4000},  // Dudley Carr
                        new UserScore {rank = 7, username = "TSQ", score = 3000},  // The Shell Queen
                        new UserScore {rank = 8, username = "DVA", score = 2000},  // Davié
                        new UserScore {rank = 9, username = "RMK", score = 1000}   // Remember Mario Kart
                    }; 
                }
                #if PERSIST_SCORES
                SaveHighScores();
                #endif
            }

            return highScores;
        }
    }

    private static bool LoadHighScores()
    {
        //Load from file
        if(File.Exists(Application.persistentDataPath + "/scores.save"))
        {
            highScores = new List<UserScore>();
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/scores.save")))
            {
                int rank = 0;
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    highScores.Add(new UserScore {
                        rank = rank,
                        username = reader.ReadString(),
                        score = reader.ReadInt32()
                    }); 
                    ++rank;
                }
            }
            return true;
        }
        return false;
    }

    public static void SaveHighScores()
    {
        //Save to file
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/scores.save")))
        {
            for (int i = 0; i < highScores.Count; ++i)
            {
                writer.Write(highScores[i].username);
                writer.Write(highScores[i].score);
            }
        }
    }

    #if OFFLINE_MODE
    private static bool LoadStats()
    {
        //Load from file
        if(File.Exists(Application.persistentDataPath + "/stats.save"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/stats.save")))
            {
                playerOverallStats = UsageStats.Read(reader);
                lastPlayStats = UsageStats.Read(reader);
                bestPlayStats = UsageStats.Read(reader);
                PlayersHighestScore = reader.ReadInt32();
                TotalGamesPlayed = reader.ReadInt32();
            }
            return true;
        }
        else
        {
            playerOverallStats = playerOverallStats ?? new UsageStats();
            lastPlayStats = lastPlayStats ?? new UsageStats();
            bestPlayStats = bestPlayStats ?? new UsageStats();
        }
        return false;
    }

    public static void SaveStats()
    {
        //Save to file
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/stats.save")))
        {
            playerOverallStats.Write(writer);
            lastPlayStats.Write(writer);
            bestPlayStats.Write(writer);
            writer.Write(PlayersHighestScore);
            writer.Write(TotalGamesPlayed);
        }
    }
    #endif
}
