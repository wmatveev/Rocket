using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//analytics
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine.Analytics;
public class AnalyticsManager : MonoBehaviour
{
    #region Singleton
    public static AnalyticsManager Instance { get; private set; }
    #endregion
    void Start()
    {
        Instance = this;
        StartFirebase();
    }

    #region Analytics
    //firebase info
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public void StartFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
                Debug.LogError("Firebase analytics failed: " + dependencyStatus);
        });
    }

    void InitializeFirebase()
    {
        //Enabling data collection
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        //We can add it later:
        //Set the user's sign up method
        //FirebaseAnalytics.SetUserProperty(FirebaseAnalytics.UserPropertySignUpMethod, "Google");
        //Set the user ID
        //FirebaseAnalytics.SetUserId("uber_user_510");
    }

    public void EndlevelAnalytics()
    {
        float winRate;
        if (!PlayerPrefs.HasKey("LosingOnThisLevel"))
            winRate = 1f;
        else
        {
            winRate = 1f / (1f + (float)PlayerPrefs.GetInt("LosingOnThisLevel"));
            PlayerPrefs.DeleteKey("LosingOnThisLevel");
        }

        LevelInfo.Instance.SetEndLevelInfo(winRate);

        string param = "winRateOnLevel" + LevelInfo.Instance.currentLevel.ToString();
#if ENABLE_CLOUD_SERVICES_ANALYTICS
        Analytics.CustomEvent("level_" + LevelInfo.Instance.currentLevel + "_end", new Dictionary<string, object>
        {
            { param, winRate },
            { "score", LevelInfo.Instance.score }
        });
#endif
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, FirebaseAnalytics.ParameterLevel, LevelInfo.Instance.currentLevel);
        FirebaseAnalytics.LogEvent("winrate", param, winRate);
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPostScore, FirebaseAnalytics.ParameterScore, LevelInfo.Instance.score);
    }
    #endregion
}
