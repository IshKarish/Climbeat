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

    public static LevelData FromLevel (LevelEditor editor)
    {
        return new LevelData
        {
            Secs = editor.secs,
            lvlName = editor.lvlNameInput.text,

            samplesLeangth = editor.song.samples * editor.song.channels,
            channels = editor.song.channels,
            frequency = editor.song.frequency,
            samples = editor.samples,
        };
    }
}
