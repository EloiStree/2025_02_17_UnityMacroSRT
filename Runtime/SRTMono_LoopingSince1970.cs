using System;
using UnityEngine;
using UnityEngine.Events;

public class SRTMono_LoopingSince1970 : MonoBehaviour {

    public SRTFileFetchAndPush m_process;
    public long m_loopTimeInMilliseconds = 0;
    public long m_ntpOffset;

    public long m_millisecondsSince1970;
    public long m_secondsSince1970;
    public long m_relativeTimeInMilliseconds;

    public void Update()
    {
        m_millisecondsSince1970 = GetTimeNowUtcOffsetAsMilliseconds();
        m_loopTimeInMilliseconds = m_process.m_fetcher.GetDurationInMilliseconds();
        if (m_loopTimeInMilliseconds == 0)
            m_loopTimeInMilliseconds = 22*60*1000;
        m_secondsSince1970 = m_millisecondsSince1970 / 1000;
        m_relativeTimeInMilliseconds = m_millisecondsSince1970 % m_loopTimeInMilliseconds;
        m_process.PushNewAndOld(m_relativeTimeInMilliseconds);
    }

    private long GetTimeNowUtcOffsetAsMilliseconds()
    {
        long now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        long start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / TimeSpan.TicksPerMillisecond;
        return (now- start) + m_ntpOffset;
    }
}
