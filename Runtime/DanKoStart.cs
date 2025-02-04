using DanKoSdk.Runtime.Infrastructure;
using UnityEngine;

namespace DanKoSdk.Runtime
{
  public class DanKoSdk : MonoSingleton<DanKoSdk>
  {
    [SerializeField] private Platform _platform;
    [SerializeField] private string _laggedGameId;
  
    public IPlatformManager PlatformManager { get; private set; }

    private void Start() {
      switch (_platform) {
        case Platform.Lagged:
          PlatformManager = new LaggedPlatformManager();
          break;
        case Platform.Yandex:
          break;
      }
    
      StartCoroutine(PlatformManager.Init(_laggedGameId));
    }
  }
}