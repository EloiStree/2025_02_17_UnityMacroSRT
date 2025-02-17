using UnityEngine;
using UnityEngine.Events;

public class SRTMono_SplitLine : MonoBehaviour
{

    public TextLineInSRT m_lastReceived;
    public UnityEvent<string> m_onTextPushed;
    public UnityEvent<string[]> m_onLinesPushed;
    public UnityEvent<float> m_onDurationPushed;
    public UnityEvent<long> m_onRelativeStartTimePushed;
    public UnityEvent<long> m_onRelativeStopTimePushed;
    public UnityEvent<long, long> m_onRelativeStartStopTimePushed;

    public void PushIn(TextLineInSRT linePushed)
    {

        m_lastReceived = linePushed;
        if(m_lastReceived==null)
        {
            return;
        }
        m_onTextPushed.Invoke(linePushed.m_textInElapsedTime);
        m_onDurationPushed.Invoke(linePushed.GetDurationInSeconds());
        m_onRelativeStartTimePushed.Invoke(linePushed.m_timeSpace.GetStartInMilliseconds());
        m_onRelativeStopTimePushed.Invoke(linePushed.m_timeSpace.GetEndInMilliseconds());
        m_onRelativeStartStopTimePushed.Invoke(linePushed.m_timeSpace.GetStartInMilliseconds(), linePushed.m_timeSpace.GetEndInMilliseconds());
        m_onLinesPushed.Invoke(linePushed.m_textInElapsedTime.Split('\n'));
    }
}
