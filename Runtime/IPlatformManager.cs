using System;
using System.Collections;
using DanKoSdk.Runtime.Infrastructure;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DanKoSdk.Runtime
{
  public interface IPlatformManager : IDisposable
  {
    Platform Platform { get; }
    bool IsInitialized { get; }
    bool IsInterstitialAvailable { get; }
    bool IsRewardedAvailable { get; }
    bool CanShowSticky { get; }
    bool IsShowingRewarded { get; }

    IEnumerator Init(params object[] payload);
    void ShowInterstitial(Action onSuccess = null, Action onClose = null, Action onError = null);
    void ShowRewarded(Action onOpen = null, Action onReward = null, Action onClose = null, Action onError = null);
  }

  public abstract class PlatformManagerBase : IPlatformManager
  {
    public virtual void PauseGameplay() {
      Time.timeScale = 0f;
      AudioListener.pause = true;
    }

    public virtual void ResumeGameplay() {
      Time.timeScale = 1f;
      AudioListener.pause = false;
    }

    public abstract Platform Platform { get; }
    public abstract bool IsInitialized { get; }
    public abstract bool CanShowSticky { get; }
    public abstract bool IsInterstitialAvailable { get; protected set; }
    public abstract bool IsRewardedAvailable { get; protected set; }
    public abstract bool IsShowingRewarded { get; }
  
    public abstract IEnumerator Init(params object[] payload);
    public abstract void ShowInterstitial(Action onSuccess = null, Action onClose = null, Action onError = null);
    public abstract void ShowRewarded(Action onOpen = null, Action onReward = null, Action onClose = null, Action onError = null);
    public abstract void Dispose();
  }

  public class LaggedPlatformManager : PlatformManagerBase
  {
    private const string DEVELOPER_ID = "lagdev_14474";
    private const string ADSENSE_ID = "ca-pub-55566677781";
    private readonly string _gameId;
    private readonly LaggedAPIUnity _laggedApiUnity;
  
    private Action _onRewardedOpen;
    private Action _onRewardedSuccess;
    private Action _onRewardedClose;
    private Action _onRewardedError;

    public LaggedPlatformManager() {
      _laggedApiUnity = InstantiateLaggedApi();
      SubscribeOnLaggedEvents();
      CoroutineRunner.Instance.StartCoroutine(CheckRewardAdLoop());
    }

    private void SubscribeOnLaggedEvents() {
      LaggedAPIUnity.OnPauseGame += PauseGameplay;
      LaggedAPIUnity.OnResumeGame += ResumeGameplay;
      LaggedAPIUnity.onRewardAdReady += OnRewardAdReady;
      LaggedAPIUnity.onRewardAdSuccess += OnRewardAdSuccess;
      LaggedAPIUnity.onRewardAdFailure += OnRewardAdFailure;
    }

    private void OnRewardAdFailure() {
      IsRewardedAvailable = false;
    }

    private void OnRewardAdSuccess() {
      _onRewardedSuccess?.Invoke();
      IsRewardedAvailable = false;
    }

    public override Platform Platform => Platform.Lagged;
    public override bool IsInitialized { get; }
    public override bool CanShowSticky => false;
    public override bool IsShowingRewarded { get; }

    public override bool IsInterstitialAvailable { get; protected set; }
    public override bool IsRewardedAvailable { get; protected set; }

    public override IEnumerator Init(params object[] payload) {
      var gameId = (string)payload[0];
      _laggedApiUnity.DEV_ID = gameId;
      _laggedApiUnity.PUBLISHER_ID = ADSENSE_ID;
      _laggedApiUnity.Init();
      yield break;
    }

    public override void ShowInterstitial(Action onSuccess = null, Action onClose = null, Action onError = null) {
      throw new NotImplementedException();
    }

    public override void ShowRewarded(Action onOpen = null, Action onReward = null, Action onClose = null, Action onError = null) {
      if (IsShowingRewarded) {
        Debug.LogWarning("Rewarded ad is already showing");
        return;
      }

      _onRewardedOpen = onOpen;
      _onRewardedSuccess = onReward;
      _onRewardedClose = onClose;
      _onRewardedError = onError;
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
      var go = new GameObject(nameof(LaggedPlatformManager));
      Object.DontDestroyOnLoad(go);
      var laggedApi = go.AddComponent<LaggedAPIUnity>();
      LaggedAPIUnity.Instance = laggedApi;
      return laggedApi;
    }
  
    private void OnRewardAdReady()
    {
      IsRewardedAvailable = true;
    }
  
    private void CheckRewardAd()
    {
      if (IsRewardedAvailable) {
        return;
      }
    
      _laggedApiUnity.CheckRewardAd();
      Debug.LogWarning("CheckRewardAd");
    }
  
    private IEnumerator CheckRewardAdLoop()
    {
      while (true) {
        yield return new WaitUntil(() => IsRewardedAvailable);
        yield return new WaitForSeconds(3);
        CheckRewardAd();
      }
    }
  }

  public enum Platform
  {
    Yandex,
    Lagged,
  }
}