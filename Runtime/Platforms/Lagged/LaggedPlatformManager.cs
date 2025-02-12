using System;
using System.Collections;
using DanKoSdk.Runtime.Infrastructure;
using DanKoSdk.Runtime.Platforms.Common;
using DanKoSdk.Runtime.Providers.Lagged;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DanKoSdk.Runtime.Platforms.Lagged
{
  public class LaggedPlatformManager : PlatformManagerBase
  {
    private const string DEVELOPER_ID = "lagdev_14474";
    private const string ADSENSE_ID = "ca-pub-55566677781";
    
    private readonly string _gameId;
    private readonly LaggedAPIUnity _laggedApiUnity;
    private GameObject _imageGo;

    public LaggedPlatformManager() {
      _laggedApiUnity = InstantiateLaggedApi();
      CreateBlocker();
      SwitchBlockBackground(false);
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
    }

    private void OnRewardAdSuccess() {
      LogCallbackIfAppropriate(nameof(OnRewardAdSuccess));
      IsRewardedAvailable = false;
      IsShowingRewarded = false;
      RewardedSuccess?.Invoke();
    }

    public override Platform Platform => Platform.Lagged;

    public override bool IsInterstitialAvailable { get; protected set; }
    public override bool IsRewardedAvailable { get; protected set; }

    public override IEnumerator Init(params object[] payload) {
      _laggedApiUnity.DEV_ID = DEVELOPER_ID;
      _laggedApiUnity.PUBLISHER_ID = ADSENSE_ID;
      CanShowSticky = false;
      IsRewardedAvailable = false;
      _laggedApiUnity.Init();
      IsInitialized = true;
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
    }

    private void ClearDelegates() {
      ADOpen = null;
      ADClosed = null;
      RewardedSuccess = null;
      RewardedError = null;
    }

    public override void ShowInterstitial(Action onOpen = null, Action onClose = null, Action onError = null) {
      LogCallbackIfAppropriate(nameof(ShowInterstitial));
      ClearDelegates();
      
      ADOpen = onOpen;
      ADClosed = onClose;
      LaggedAPIUnity.Instance.ShowAd();
    }

    public override void ShowRewarded(Action onOpen = null, Action onReward = null, Action onClose = null,
      Action onError = null) {
      LogCallbackIfAppropriate(nameof(ShowRewarded));
      ClearDelegates();
      
      if (IsShowingRewarded || !IsRewardedAvailable) {
        Debug.LogWarning("Rewarded ad is not available!");
        onError?.Invoke();
        return;
      }

      IsShowingRewarded = true;
      SwitchBlockBackground(true);
      
      RewardedSuccess += onOpen;
      RewardedSuccess += onReward;
      RewardedSuccess += onClose;
      RewardedSuccess += () => SwitchBlockBackground(false);
      
      RewardedError += onError;
      RewardedError += onClose;
      RewardedError += () => SwitchBlockBackground(false);
      LaggedAPIUnity.Instance.PlayRewardAd();
    }

    private void SwitchBlockBackground(bool state) {
      _imageGo.SetActive(state);
    }

    public override void SetHighScore(int score, string board) {
      LaggedAPIUnity.Instance.CallHighScore(score, board);
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
    
    private void CreateBlocker()
    {
      var canvasGo = new GameObject("[LaggedCanvas]");
      Object.DontDestroyOnLoad(canvasGo);
      var canvas = canvasGo.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      var canvasScaler = canvasGo.AddComponent<CanvasScaler>();
      canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
      canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
      
      canvasGo.AddComponent<GraphicRaycaster>();

      _imageGo = new GameObject("dimmer");
      _imageGo.transform.SetParent(canvasGo.transform, false);

      var image = _imageGo.AddComponent<Image>();
      image.color = new Color32(134, 49, 255, 152);

      var canvasGroup = _imageGo.AddComponent<CanvasGroup>();
      canvasGroup.blocksRaycasts = true;

      var rectTransform = image.GetComponent<RectTransform>();
      rectTransform.anchorMin = Vector2.zero;
      rectTransform.anchorMax = Vector2.one;
      rectTransform.offsetMin = Vector2.zero;
      rectTransform.offsetMax = Vector2.zero;
    }
  }
}