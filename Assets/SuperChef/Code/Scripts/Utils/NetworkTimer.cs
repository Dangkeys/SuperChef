using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
public class NetworkTimer : NetworkBehaviour
{
    private NetworkVariable<double> startTime = new NetworkVariable<double>(0.0);
    public NetworkVariable<bool> IsTimerActive { get; private set; } = new NetworkVariable<bool>(false);
    public NetworkVariable<float> Duration { get; private set; } = new NetworkVariable<float>(60);
    public event Action OnTimerStarted;
    public event Action OnTimerEnded;

    public float TimeRemaining
    {
        get
        {
            if (NetworkManager.Singleton == null) return 0f;
            double timePassed = NetworkManager.Singleton.ServerTime.Time - startTime.Value;
            return Mathf.Max(0, Duration.Value - (float)timePassed);
        }
    }
    public void StartTimer(float duration)
    {
        if (!IsServer) return;

        Duration.Value = Mathf.Max(0, duration);
        startTime.Value = NetworkManager.Singleton.ServerTime.Time;
        IsTimerActive.Value = true;

        StartCoroutine(TimerRoutine(Duration.Value));

        OnTimerStarted?.Invoke();
    }
    private IEnumerator TimerRoutine(float timeToWait)
    {

        yield return new WaitForSeconds(timeToWait);


        IsTimerActive.Value = false;
        StopAllCoroutines();

        OnTimerEnded?.Invoke();
    }

}
