using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public float[] Secs { get; set; }
    public string lvlName;

    public int samplesLeangth { get; set; }
    public int channels { get; set; }
    public int frequency { get; set; }
    public float[] samples { get; set; }

    public float[] xPos { get; set; }
    public string authorName;
    public string bpm;
    public float[] yPos { get; set; }

    public static LevelData FromLevel (LevelEditor editor)
    {
        return new LevelData
        {
            Secs = editor.secs,
            lvlName = editor.lvlNameTxt.text,

            samplesLeangth = editor.samples.Length,
            channels = editor.song.channels,
            frequency = editor.song.frequency,
            samples = editor.samples,

            xPos = editor.xPos,
            authorName = editor.authorInput.text,
            bpm = editor.bpmInput.text,
            yPos = editor.yPos,
        };
    }
}
