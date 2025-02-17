using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class SRTMono_TextToData : MonoBehaviour
{

    public TextAsset m_imporText;
    public List<TextLineInSRT> m_textLines;

    [ContextMenu("Parse")]
    public void Parse(){
        SRTImportUtility.TryToImport(m_imporText.text, out m_textLines);
    }
}

public class SRTImportUtility{
    public static string m_timeLineRegexFromTo=
    @"\s*\d{2}\s*:\s*\d{2}\s*:\s*\d{2}\s*,\s*\d{3}\s*-->\s*\d{2}\s*:\s*\d{2}\s*:\s*\d{2}\s*,\s*\d{3}\s*";
    public static void TryToImport(in string text, out List<TextLineInSRT> outTextLines){
        
            Regex regex = new Regex(m_timeLineRegexFromTo);
        outTextLines = new List<TextLineInSRT>();
        string [] lines = text.Split(new string[] { "\r\n", "\n","\r" }, StringSplitOptions.None);
        StringBuilder textBuilder = new StringBuilder();
        TextLineInSRT lineInBuild = new TextLineInSRT();
        foreach (string line in lines)
        {
            string t = line.Trim();
            // From to time
            //00:00:00,260 --> 00:00:11,679
            if ( t.IndexOf("-->")>0 && t.IndexOf(":")>0){
                
                t = t.Replace(",",":");
                t = t.Replace("-->",":");
                t=t.Replace("00","0");
                t=t.Replace("000","0");
                t=t.Replace("0000","0");
            
                string [] timeStrings = t.Split(':');
                if (timeStrings.Length!=8){
                    continue;
                }
                lineInBuild.SetText(textBuilder.ToString());
                textBuilder.Clear();
                lineInBuild= new TextLineInSRT();
                
                int.TryParse(timeStrings[0].Trim(), out int hoursFrom) ;
                int.TryParse(timeStrings[1].Trim(), out int minutesFrom) ;
                int.TryParse(timeStrings[2].Trim(), out int secondsFrom) ;
                int.TryParse(timeStrings[3].Trim(), out int millisecondsFrom) ;
                int.TryParse(timeStrings[4].Trim(), out int hoursTo) ;
                int.TryParse(timeStrings[5].Trim(), out int minutesTo) ;
                int.TryParse(timeStrings[6].Trim(), out int secondsTo) ;
                int.TryParse(timeStrings[7].Trim(), out int millisecondsTo);
               
                RelativeTimeFromToInMS timeSpace = new RelativeTimeFromToInMS();
                timeSpace.m_relativeTimeInMillisecondsStart = (long)(hoursFrom*3600000 + minutesFrom*60000 + secondsFrom*1000 + millisecondsFrom);
                timeSpace.m_relativeTimeInMillisecondsEnd = (long)(hoursTo*3600000 + minutesTo*60000 + secondsTo*1000 + millisecondsTo);
                lineInBuild.SetRelativeTime(timeSpace);
            }
            else{
                textBuilder.AppendLine(t);
            }

        }
        
    }

}

[System.Serializable]
public class SRTFileContent{
    public string m_fileName="";
    public List<TextLineInSRT> m_textLines = new List<TextLineInSRT>();
}
[System.Serializable]
public class TextLineInSRT{
    public string m_textInElapsedTime="";
    public RelativeTimeFromToInMS m_timeSpace= new RelativeTimeFromToInMS();

    public void SetRelativeTime(RelativeTimeFromToInMS timeSpace)
    {
        m_timeSpace = timeSpace;
    }
    public void SetText(string text)
    {
        m_textInElapsedTime = text;
    }
    public bool IsMultipleLines(){
        return m_textInElapsedTime.Contains("\n");
    }
    public bool IsOneLine(){
        return !IsMultipleLines();
    }
    public int TextLength(){
        return m_textInElapsedTime.Length;
    }
    public float GetDurationInSeconds(){
        return m_timeSpace.GetDurationInMS()/1000.0f;
    }
    public long GetDurationInMilliseconds(){
        return m_timeSpace.GetDurationInMS();
    }
}

[System.Serializable]
public class RelativeTimeFromToInMS{
    public long m_relativeTimeInMillisecondsStart = 0;
    public long m_relativeTimeInMillisecondsEnd = 0;

    public long GetStartInMS(){
        return m_relativeTimeInMillisecondsStart;
    }
    public long GetEndInMS(){
        return m_relativeTimeInMillisecondsEnd;
    }
    public long GetDurationInMS(){
        return m_relativeTimeInMillisecondsEnd - m_relativeTimeInMillisecondsStart;
    }

}
