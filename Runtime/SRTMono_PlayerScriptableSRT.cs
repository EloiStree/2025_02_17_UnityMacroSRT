using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SRTMono_PlayerScriptableSRT : MonoBehaviour
{
    public bool m_autoStart = true;
    public SRTFileFetchAndPush m_process;
    [Header("Debug")]
    public long m_startTime;
    public long m_relativeTimeInMilliseconds;
    public float m_relativeTimeInSeconds;
    void Update()
    {
        Refresh();
    }

    public void Start()
    {
        if (m_autoStart)
        {
            StartNow();
        }
    }
    public void StartNow() { 
    
        m_startTime = GetTimeNowAsMilliseconds();
    }

    private DateTime GetTimeNow()
    {
        return DateTime.UtcNow;
    }

    private void Refresh()
    {
        m_relativeTimeInMilliseconds =GetTimeNowAsMilliseconds() - m_startTime;
        m_relativeTimeInSeconds = m_relativeTimeInMilliseconds / 1000.0f;
        m_process.PushNewAndOld(m_relativeTimeInMilliseconds);
    }

    private long GetTimeNowAsMilliseconds()
    {
        long now=  DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        long start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / TimeSpan.TicksPerMillisecond;
        return start - now;
    }
}
