using System.Collections;
using UnityEngine;

namespace DanKoSdk.Runtime.Infrastructure
{
  public interface ICoroutineRunner
  {
    Coroutine StartCoroutine(IEnumerator cor);
    void StopCoroutine(Coroutine cor);
  }
}