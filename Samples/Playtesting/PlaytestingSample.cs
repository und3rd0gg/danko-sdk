using DanKoSdk.Samples.Example;
using UnityEngine;

namespace DanKoSdk.Samples.Playtesting
{
    public class PlaytestingSample : MonoBehaviour
    {
        public void ShowRewardedAd() {
            Debug.Log("reward3");
        
            SdkInitializer.Instance.PlatformManager.ShowRewarded(() => {
                Debug.Log("open");
            }, () => {
                Debug.Log("reward");
            }, () => {
                Debug.Log("close");
            }, () => {
                Debug.Log("error");
            });
        }

        public void ShowInter() {
            SdkInitializer.Instance.PlatformManager.ShowInterstitial();
        }
    }
}
