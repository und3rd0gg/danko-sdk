using UnityEngine;
using DanKoSdk.Runtime;

public class PlaytestingSample : MonoBehaviour
{
    public void ShowRewardedAd() {
        Debug.Log("reward3");
        
        Sdk.Instance.PlatformManager.ShowRewarded(() => {
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
        Sdk.Instance.PlatformManager.ShowInterstitial();
    }
}
