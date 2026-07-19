using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    public string SpeakerName;
    [TextArea(2, 4)] public string Text;
}