using UnityEngine;
using System;

public class Timer
{
    private float duration;
    private float elapsed;
    private bool isRunning;
    private Action onComplete;

    public Timer(float duration, Action onComplete)
    {
        this.duration = duration;
        this.onComplete = onComplete;
    }

    public void Start()
    {
        elapsed = 0f;
        isRunning = true;
    }

    public void Update()
    {
        if (!isRunning) return;

        elapsed += Time.unscaledDeltaTime;

        if (elapsed >= duration)
        {
            isRunning = false;
            onComplete?.Invoke();
        }
    }

    public void Stop()
    {
        isRunning = false;
    }

    public bool IsRunning => isRunning;
}
