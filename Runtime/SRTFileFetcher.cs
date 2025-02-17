using System.Collections.Generic;

[System.Serializable]
public class SRTFileFetcher
{
    public SRTFileContent m_playing;
    public SRTFileContentScriptable m_useScriptable;
    public List<TextLineInSRT> m_focusLinesCurrent = new List<TextLineInSRT>();
    public List<TextLineInSRT> m_focusLinesPrevious = new List<TextLineInSRT>();
    public List<TextLineInSRT> m_newFocusLines = new List<TextLineInSRT>();
    public List<TextLineInSRT> m_outFocusLines = new List<TextLineInSRT>();
    public long m_lastReceivedTimeInMillisecondsRelative;

    public void RefreshListAndGetNewOldValue(long timeInMillisecondsRelative)
    {
        m_lastReceivedTimeInMillisecondsRelative = timeInMillisecondsRelative;
        if (m_useScriptable!=null)
        {
            m_playing = m_useScriptable.m_data;
        }
        if (m_playing == null)
        {
            return;
        }
        long timePastInMS = timeInMillisecondsRelative;
        m_focusLinesPrevious = m_focusLinesCurrent;
        m_focusLinesCurrent = m_playing.GetAffectedLine(timePastInMS);
        m_newFocusLines = m_focusLinesCurrent.FindAll(x => !m_focusLinesPrevious.Contains(x));
        m_outFocusLines = m_focusLinesPrevious.FindAll(x => !m_focusLinesCurrent.Contains(x));
    }

    public long GetDurationInMilliseconds()
    {
        long result = 0;
        foreach (TextLineInSRT line in m_playing.m_textLines)
        {
            long end = line.GetRelativeEndInMilliseconds();
            if (end > result)
            {
                result = end;
            }
        }
        return result;
    }
}
