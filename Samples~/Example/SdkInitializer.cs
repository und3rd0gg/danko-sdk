using DanKoSdk.Runtime.Infrastructure;
using DanKoSdk.Runtime.Platforms.Common;
using DanKoSdk.Runtime.Platforms.Lagged;
using UnityEngine;

namespace DanKoSdk.Samples.Example
{
  public class SdkInitializer : MonoSingleton<SdkInitializer>
  {
    [SerializeField] private Platform _platform;
  
    public IPlatformManager PlatformManager { get; private set; }

    private void Start() {
      switch (_platform) {
        case Platform.Lagged:
          PlatformManager = new LaggedPlatformManager();
          break;
        case Platform.Yandex:
          break;
      }
      
      PlatformManager.SwitchCallbackLogging(true);
    
      StartCoroutine(PlatformManager.Init());
    }
  }
}