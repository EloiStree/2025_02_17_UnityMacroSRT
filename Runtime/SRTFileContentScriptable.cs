using UnityEngine;

[CreateAssetMenu(fileName = "SRTFileContent", menuName = "ScriptableObjects/SRTFileContent", order = 1)]
public class SRTFileContentScriptable : ScriptableObject
{
    public SRTFileContent m_data;
    public SRTFileContent GetData() => m_data;
    public void SetData(SRTFileContent data) => m_data = data;
    public void GetData(out SRTFileContent data) => data = m_data;

}
