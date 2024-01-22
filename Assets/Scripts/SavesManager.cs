using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavesManager
{
    public string levelName;
    public string author;
    public int bpm;

    public int samplesLength;
    public int channels;
    public int frequency;
    public float[] samples;

    public float[] seconds;

    public SavesManager(string levelName, string author, int bpm, AudioClip clip, float[] seconds)
    {
        this.levelName = levelName;
        this.author = author;
        this.bpm = bpm;
        
        CreateAudioData(clip, out int samplesLength, out int channels, out int frequency, out float[] samples);
        this.samplesLength = samplesLength;
        this.channels = channels;
        this.frequency = frequency;
        this.samples = samples;

        this.seconds = seconds;
    }

    public void SaveLevel()
    {
        SaveSystem.SaveLevel(this, levelName, Application.persistentDataPath);
    }

    void CreateAudioData(AudioClip clip, out int samplesLength, out int channels, out int frequency, out float[] samples)
    {
        channels = clip.channels;
        samplesLength = clip.samples * channels;
        samples = new float[samplesLength];
        frequency = clip.frequency;
    }
}
