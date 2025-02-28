using UnityEngine;
using System.Collections;

public static class TimerManager
{
    
    public static void PauseGame ()
    {
        Time.timeScale = 0;
    }

    public static void ResumeGame ()
    {
        Time.timeScale = 1;
    }
    private class TimerRunner : MonoBehaviour { } // for running coroutines

    private static TimerRunner _runner;

    private static TimerRunner Runner
    {
        get
        {
            if (_runner == null)
            {
                GameObject obj = new GameObject("TimerManager");
                Object.DontDestroyOnLoad(obj);
                _runner = obj.AddComponent<TimerRunner>();
            }
            return _runner;
        }
    }

    public static void StartTimer(
        float interval, 
        System.Action action,
        bool shouldInvokeOnlyOnce = false)
    {
        Runner.StartCoroutine(RunTimer(interval, action,shouldInvokeOnlyOnce));
    }

    public static void StartTimer(System.Action action)
    {
        action?.Invoke();
    }

    private static IEnumerator RunTimer(
        float interval, 
        System.Action action,
        bool shouldInvokeOnlyOnce)
    {   
        while (true)
            {
            yield return new WaitForSeconds(interval);
            action?.Invoke();
            if(shouldInvokeOnlyOnce) break;
            }
        
    }
}