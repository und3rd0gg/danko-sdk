using UnityEngine;

namespace DanKoSdk.Runtime.Infrastructure
{
  public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static T instance;

    private static bool s_InstanceDestroyed = false;

    private static bool s_InstanceExisting = false;

    public static T Instance {
      get {
        if (instance == null) {
          instance = FindObjectOfType<T>();

          if (instance == null) {
            var singletonObject = new GameObject();
            singletonObject.AddComponent<T>();

            DontDestroyOnLoad(singletonObject);
          }
        }

        return instance;
      }
    }

    public static bool IsSInstanceDestroyed =>
      s_InstanceDestroyed;

    public static bool IsSInstanceExisting =>
      s_InstanceExisting;

    public virtual void Awake() {
      if (instance == null) {
        instance = this as T;
        s_InstanceDestroyed = false;
        s_InstanceExisting = true;
        DontDestroyOnLoad(instance.gameObject);
      }
      else if (instance.GetInstanceID() != GetInstanceID()) {
        Destroy(gameObject);
      }
    }

    public virtual void OnDestroy() {
      s_InstanceDestroyed = true;
      s_InstanceExisting = false;
    }
  }
}