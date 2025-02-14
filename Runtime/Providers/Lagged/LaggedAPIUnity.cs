using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LaggedAPIUnity : MonoBehaviour
{

    public static LaggedAPIUnity Instance;

    public string GAME_KEY = "YOUR_GAME_KEY";

    public static Action OnResumeGame;
    public static Action OnPauseGame;
    public static Action onRewardAdReady;
    public static Action onRewardAdSuccess;
    public static Action onRewardAdFailure;

    [DllImport("__Internal")]
    private static extern void SDK_Init(string gameKey);

    [DllImport("__Internal")]
    private static extern void SDK_CallHighScore(int SCORE, string BOARD_ID);

    [DllImport("__Internal")]
    private static extern void SDK_SaveAchievement(string AWARD);

    [DllImport("__Internal")]
    private static extern void SDK_ShowAd();

    [DllImport("__Internal")]
    private static extern void SDK_CheckRewardAd();

    [DllImport("__Internal")]
    private static extern void SDK_PlayRewardAd();

    void Awake()
    {
        if (LaggedAPIUnity.Instance == null)
            LaggedAPIUnity.Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);

        Init();
    }

    internal void Init()
    {
        try
        {
            SDK_Init(GAME_KEY);
            Debug.Log($"SDK INITIALIZED WITH KEY {GAME_KEY}");
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("LaggedAPI initialization failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }
    internal void ShowAd()
    {
        try
        {
            SDK_ShowAd();
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("LaggedAPI ShowAd failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    internal void CheckRewardAd()
    {
        try
        {
            SDK_CheckRewardAd();
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("LaggedAPI Reward Ad failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    internal void PlayRewardAd()
    {
        try
        {
            SDK_PlayRewardAd();
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("LaggedAPI Reward Ad failed. Make sure you are running a WebGL build in a browser:" + e.Message);
        }
    }

    internal void ShowRewardedAd()
    {
        try
        {
            //SDK_ShowAd("rewarded");
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("LaggedAPI Reward Ad failed. Make sure you are running a WebGL build in a browser: " + e.Message);
        }
    }

    internal void CallHighScore(int score, string board)
    {
        try
        {
            SDK_CallHighScore(score,board);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("LaggedAPI Call High Score failed: " + e.Message);
        }
    }

    internal void SaveAchievement(string award)
    {
        try
        {
            SDK_SaveAchievement(award);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("LaggedAPI Save Achievement failed: " + e.Message);
        }
    }

    ///
    /// Called when ad is completed and the game should start.
    ///
    void ResumeGameCallback()
    {
        if (OnResumeGame != null) OnResumeGame();
    }

    ///
    /// Called when ad starts, game/music should pause
    ///
    void PauseGameCallback()
    {
        if (OnPauseGame != null) OnPauseGame();
    }

    ///
    /// if reward ad is ready
    ///
    void RewardAdReadyCallback()
    {
        if (onRewardAdReady != null) onRewardAdReady();
    }

    ///
    /// if reward is successful, give player reward
    ///
    void RewardAdSuccessCallback()
    {
        if (onRewardAdSuccess != null) onRewardAdSuccess();
    }

    ///
    /// if reward ad failed, no reward
    ///
    void RewardAdFailCallback()
    {
        if (onRewardAdFailure != null) onRewardAdFailure();
    }

}
