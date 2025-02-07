using System;
using System.Collections;
using DanKoSdk.Runtime.Infrastructure;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DanKoSdk.Runtime
{
  public class LaggedPlatformManager : PlatformManagerBase
  {
    private const string DEVELOPER_ID = "lagdev_14474";
    private const string ADSENSE_ID = "ca-pub-55566677781";
    
    private readonly string _gameId;
    private readonly LaggedAPIUnity _laggedApiUnity;

    public LaggedPlatformManager() {
      _laggedApiUnity = InstantiateLaggedApi();
      SubscribeOnLaggedEvents();
    }

    private void SubscribeOnLaggedEvents() {
      LaggedAPIUnity.OnPauseGame += PauseGameplay;
      LaggedAPIUnity.OnResumeGame += ResumeGameplay;
      LaggedAPIUnity.onRewardAdReady += OnRewardAdReady;
      LaggedAPIUnity.onRewardAdSuccess += OnRewardAdSuccess;
      LaggedAPIUnity.onRewardAdFailure += OnRewardAdFailure;
    }

    private void OnRewardAdFailure() {
      LogCallbackIfAppropriate(nameof(OnRewardAdFailure));
      IsShowingRewarded = false;
      RewardedError?.Invoke();
      ClearDelegates();
    }

    private void OnRewardAdSuccess() {
      LogCallbackIfAppropriate(nameof(OnRewardAdSuccess));
      IsRewardedAvailable = false;
      RewardedSuccess?.Invoke();
      ClearDelegates();
    }

    public override Platform Platform => Platform.Lagged;

    public override bool IsInterstitialAvailable { get; protected set; }
    public override bool IsRewardedAvailable { get; protected set; }

    public override IEnumerator Init(params object[] payload) {
      _laggedApiUnity.DEV_ID = DEVELOPER_ID;
      _laggedApiUnity.PUBLISHER_ID = ADSENSE_ID;
      IsRewardedAvailable = false;
      _laggedApiUnity.Init();
      IsInitialized = true;
      CanShowSticky = false;
      CoroutineRunner.Instance.StartCoroutine(CheckRewardAdRoutine());
      yield break;
    }

    protected override void PauseGameplay() {
      base.PauseGameplay();
      ADOpen?.Invoke();
    }

    protected override void ResumeGameplay() {
      base.ResumeGameplay();
      ADClosed?.Invoke();
      ClearDelegates();
    }

    private void ClearDelegates() {
      ADOpen = null;
      ADClosed = null;
      RewardedSuccess = null;
      RewardedError = null;
    }

    public override void ShowInterstitial(Action onOpen = null, Action onClose = null, Action onError = null) {
      LogCallbackIfAppropriate(nameof(ShowInterstitial));
      
      ADOpen = onOpen;
      ADClosed = onClose;
      LaggedAPIUnity.Instance.ShowAd();
    }

    public override void ShowRewarded(Action onOpen = null, Action onReward = null, Action onClose = null,
      Action onError = null) {
      LogCallbackIfAppropriate(nameof(ShowRewarded));
      
      CheckRewardAd();

      if (IsShowingRewarded) {
        Debug.LogWarning("Rewarded ad is already showing");
        return;
      }
      
      // _adOpen = onOpen;
      // _adClosed = onClose;
      RewardedSuccess += onOpen;
      RewardedSuccess += onReward;
      RewardedSuccess += onClose;
      RewardedError += onError;
      RewardedError += onClose;
      LaggedAPIUnity.Instance.PlayRewardAd();
    }

    public override void Dispose() {
      LaggedAPIUnity.OnPauseGame -= PauseGameplay;
      LaggedAPIUnity.OnResumeGame -= ResumeGameplay;
      LaggedAPIUnity.onRewardAdReady -= OnRewardAdReady;
      LaggedAPIUnity.onRewardAdSuccess -= OnRewardAdSuccess;
      LaggedAPIUnity.onRewardAdFailure -= OnRewardAdFailure;
    }

    private LaggedAPIUnity InstantiateLaggedApi() {
      const string name = "LaggedAPIUnity";
      var go = new GameObject(name);
      Object.DontDestroyOnLoad(go);
      var laggedApi = go.AddComponent<LaggedAPIUnity>();
      LaggedAPIUnity.Instance = laggedApi;
      return laggedApi;
    }

    private void OnRewardAdReady() {
      LogCallbackIfAppropriate(nameof(OnRewardAdReady));
      IsRewardedAvailable = true;
    }

    private void CheckRewardAd() {
      LogCallbackIfAppropriate(nameof(CheckRewardAd));
      
      if (IsRewardedAvailable) {
        return;
      }

      _laggedApiUnity.CheckRewardAd();
    }

    private IEnumerator CheckRewardAdRoutine() {
      while (true) {
        yield return new WaitWhile(() => IsRewardedAvailable);
        yield return new WaitForSeconds(3);
        CheckRewardAd();
      }
      // ReSharper disable once IteratorNeverReturns
    }
  }
}