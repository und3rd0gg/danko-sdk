using System;
using System.Collections;
using UnityEngine;

namespace DanKoSdk.Runtime
{
  public abstract class PlatformManagerBase : IPlatformManager
  {
    public abstract Platform Platform { get; }
    public abstract bool IsInterstitialAvailable { get; protected set; }
    public abstract bool IsRewardedAvailable { get; protected set; }

    public bool IsShowingRewarded { get; protected set; }
    public bool IsInitialized { get; protected set; }
    public bool CanShowSticky { get; protected set; }
    public bool CallbackLoggingEnabled { get; private set; }
    
    protected Action ADOpen;
    protected Action ADClosed;
    protected Action RewardedSuccess;
    protected Action RewardedError;

    public abstract IEnumerator Init(params object[] payload);
    public abstract void ShowInterstitial(Action onOpen = null, Action onClose = null, Action onError = null);

    public abstract void ShowRewarded(Action onOpen = null, Action onReward = null, Action onClose = null,
      Action onError = null);

    public abstract void Dispose();

    public void SwitchCallbackLogging(bool state) {
      CallbackLoggingEnabled = state;
    }

    protected void LogCallbackIfAppropriate(string message) {
      if (CallbackLoggingEnabled) {
        Debug.Log($"<color=#46FF00>DanKoSdk: {message}</color>");
      }
    }

    protected virtual void PauseGameplay() {
      LogCallbackIfAppropriate(nameof(PauseGameplay));
      Time.timeScale = 0f;
      AudioListener.pause = true;
      ADOpen?.Invoke();
    }

    protected virtual void ResumeGameplay() {
      LogCallbackIfAppropriate(nameof(ResumeGameplay));
      Time.timeScale = 1f;
      AudioListener.pause = false;
      ADClosed?.Invoke();
    }
  }
}