using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class SRTMono_TextToData : MonoBehaviour
{
    public TextAsset m_imporText;
    public SRTFileContent m_textLines = new SRTFileContent();
    public SRTFileContentScriptable m_scriptableToStore;
    public bool m_removeEmptyLine=true;
    public bool m_removeIntegerLine=true;

    [ContextMenu("Try to Parse")]
    public void TryParse(){
        SRTImportUtility.TryToImport(m_imporText.text , m_imporText.name, out m_textLines);
        if (m_removeIntegerLine) { 
            SRTImportUtility.RemoveIntegerLine(ref m_textLines);
        }
        if (m_removeEmptyLine) { 
            SRTImportUtility.RemoveEmptyLine(ref m_textLines);
        }

        if (m_scriptableToStore != null)
        {
            m_scriptableToStore.m_data = m_textLines;
        }
    }
}

public class SRTImportUtility{

    public static string m_timeLineRegexFromTo=
    @"\s*\d{2}\s*:\s*\d{2}\s*:\s*\d{2}\s*,\s*\d{3}\s*-->\s*\d{2}\s*:\s*\d{2}\s*:\s*\d{2}\s*,\s*\d{3}\s*";
    public static void TryToImport(in string text, in string fileName, out SRTFileContent fileSRT) {

        fileSRT = new SRTFileContent();
        fileSRT.m_fileName =fileName;
        TryToImport(text, out fileSRT.m_textLines);
    }
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
                outTextLines.Add(lineInBuild);
                lineInBuild = new TextLineInSRT();
                
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

    public static void RemoveEmptyLine(TextLineInSRT line)
    {

        List<string> linesToFilter = new List<string>();
        linesToFilter.AddRange(line.m_textInElapsedTime.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None));
        StringBuilder textBuilder = new StringBuilder();
        foreach (string focusLine in linesToFilter)
        {
            if (! string.IsNullOrWhiteSpace(focusLine))
            {
                textBuilder.AppendLine(focusLine.Trim());
            }
        }
        line.SetText( textBuilder.ToString().Trim());

    }
    public static void RemoveIntegerLine(TextLineInSRT line)
    {
        List<string> lines = new List<string>();
        lines.AddRange(line.m_textInElapsedTime.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None));
        StringBuilder textBuilder = new StringBuilder();
        foreach (string lineText in lines)
        {
            if (! int.TryParse(lineText, out int result))
            {
                textBuilder.AppendLine(lineText);
            }
        }
        line.SetText(textBuilder.ToString());
    }

    public static void RemoveEmptyLine(ref SRTFileContent m_textLines)
    {
        foreach (TextLineInSRT line in m_textLines.m_textLines)
        {
            RemoveEmptyLine(line);
        }
    }

    public static void RemoveIntegerLine(ref SRTFileContent m_textLines)
    {
        foreach (TextLineInSRT line in m_textLines.m_textLines)
        {
            RemoveIntegerLine(line);
        }
    }
}

[System.Serializable]
public class SRTFileContent{
    public string m_fileName="";
    public List<TextLineInSRT> m_textLines = new List<TextLineInSRT>();

    public List<TextLineInSRT> GetAffectedLine(long now)
    {
        return m_textLines.FindAll(x => 
        now >= x.m_timeSpace.GetStartInMilliseconds()  &&
        now <= x.m_timeSpace.GetEndInMilliseconds() );
    }
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
        return m_timeSpace.GetDurationInMilliseconds()/1000.0f;
    }
    public long GetDurationInMilliseconds(){
        return m_timeSpace.GetDurationInMilliseconds();
    }

    public long GetRelativeEndInMilliseconds()
    {
        return m_timeSpace.GetEndInMilliseconds();
    }
}

[System.Serializable]
public class RelativeTimeFromToInMS{
    public long m_relativeTimeInMillisecondsStart = 0;
    public long m_relativeTimeInMillisecondsEnd = 0;

    public long GetStartInMilliseconds(){
        return m_relativeTimeInMillisecondsStart;
    }
    public long GetEndInMilliseconds(){
        return m_relativeTimeInMillisecondsEnd;
    }
    public long GetDurationInMilliseconds(){
        return m_relativeTimeInMillisecondsEnd - m_relativeTimeInMillisecondsStart;
    }

}
