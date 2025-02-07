using System;
using System.Collections;

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
    void SwitchCallbackLogging(bool state);
    void ShowInterstitial(Action onOpen = null, Action onClose = null, Action onError = null);
    void ShowRewarded(Action onOpen = null, Action onReward = null, Action onClose = null, Action onError = null);
  }
}