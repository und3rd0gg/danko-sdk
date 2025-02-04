namespace DanKoSdk.Runtime.Infrastructure
{
  public class CoroutineRunner : MonoSingleton<CoroutineRunner>, ICoroutineRunner
  {
    public override void Awake() {
      base.Awake();
      name = $"[{ nameof(CoroutineRunner).ToUpper()}]";
    }
  }
}