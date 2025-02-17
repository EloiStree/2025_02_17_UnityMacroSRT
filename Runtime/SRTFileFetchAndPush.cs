using System;
using UnityEngine.Events;

[System.Serializable]
public class SRTFileFetchAndPush {
    public SRTFileFetcher m_fetcher;
    public UnityEvent<TextLineInSRT> m_onStartRunning;
    public UnityEvent<TextLineInSRT> m_onEndRunning;
    public void PushNewAndOld(long timeInMilliseconds)
    {        
        m_fetcher.RefreshListAndGetNewOldValue(
            timeInMilliseconds);
        foreach (TextLineInSRT line in m_fetcher. m_newFocusLines)
        {
            m_onStartRunning.Invoke(line);
        }
        foreach (TextLineInSRT line in m_fetcher. m_outFocusLines)
        {
            m_onEndRunning.Invoke(line);
        }
    }

}
