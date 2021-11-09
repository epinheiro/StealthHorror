using System;
using System.Collections;
using UnityEngine;

public static class CoroutineLib
{
    public static Coroutine DelayedCall(this MonoBehaviour mono, float delay, Action action)
    {
        return mono.StartCoroutine(DelayCoroutine(delay, action));
    }

    private static IEnumerator DelayCoroutine(float delay, Action action) 
    {
        yield return new WaitForSeconds(delay);

        action.Invoke();
    }

    public static Coroutine PeriodicallyCall(this MonoBehaviour mono, float intervalTime, float maximumTime, Action<float, float> intervalAction, Action<float> finalAction)
    {
        return mono.StartCoroutine(PeriodicallyCoroutine(intervalTime, maximumTime, intervalAction, finalAction));
    }

    public static IEnumerator PeriodicallyCoroutine(float intervalTime, float maximumTime, Action<float, float> intervalAction, Action<float> finalAction)
    {
        float count = 0;

        while( count < maximumTime )
        {
            yield return new WaitForSeconds(intervalTime);
            count += intervalTime;
            intervalAction?.Invoke(count, maximumTime);
        }

        finalAction?.Invoke(maximumTime);
    }
}
