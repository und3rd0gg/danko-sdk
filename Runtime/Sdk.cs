using DanKoSdk.Runtime.Infrastructure;
using UnityEngine;

namespace DanKoSdk.Runtime
{
  public class Sdk : MonoSingleton<Sdk>
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