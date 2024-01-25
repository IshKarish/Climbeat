using UnityEngine;
using System.IO;

public static class SaveSystem
{
    static string format = ".notvirus";

    public static void SaveLevel (SavesManager savesManager, string name, string path)
    {
        string sPath = path + "/" + name + format;

        FileStream stream = new FileStream(sPath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(savesManager.seconds.Length);
        foreach (float s in savesManager.seconds)
        {
            writer.Write(s);
        }

        writer.Write(savesManager.samplesLength);
        writer.Write(savesManager.channels);
        writer.Write(savesManager.frequency);

        writer.Write(savesManager.samples.Length);
        foreach (float s in savesManager.samples)
        {
            writer.Write(s);
        }

        writer.Write(savesManager.levelName);
        writer.Write(savesManager.author);
        writer.Write(savesManager.bpm);

        writer.Write(savesManager.positions.Length);
        for (int i = 0; i < savesManager.positions.Length; i++)
        {
            writer.Write(savesManager.positions[i].x);
            writer.Write(savesManager.positions[i].y);
        }

        stream.Close();
    }

    public static SavesManager LoadLevel (string path)
    {
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);

            int len = reader.ReadInt32();
            float[] secondsArr = new float[len];
            for (int i = 0; i < len; i++)
            {
                secondsArr[i] = reader.ReadSingle();
            }

            int samplesLength = reader.ReadInt32();
            int channels = reader.ReadInt32();
            int frequency = reader.ReadInt32();
           
            int sLength = reader.ReadInt32();
            float[] samples = new float[sLength];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = reader.ReadSingle();
            }
            AudioClip clip = AudioClip.Create("Clip", samplesLength / 2, channels, frequency, false);
            clip.SetData(samples, 0);

            string levelName = reader.ReadString();
            string author = reader.ReadString();
            int bpm = reader.ReadInt32();

            int positionsLength = reader.ReadInt32();
            float[] xPositions = new float[positionsLength];
            float[] yPositions = new float[positionsLength];
            Vector2[] positions = new Vector2[positionsLength];
            
            for (int i = 0; i < positionsLength; i++)
            {
                xPositions[i] = reader.ReadSingle();
                yPositions[i] = reader.ReadSingle();
                
                positions[i] = new Vector2(xPositions[i], yPositions[i]);
            }
            
            SavesManager savesManager = new SavesManager(levelName, author, bpm, clip, secondsArr, positions);

            stream.Close();

            return savesManager;
        }
        else
        {
            Debug.LogError("There is no level file. starting assassination process...");
            return null;
        }
    }
}

/*
 * Saving/Loading order:
 * 1. Seconds array length
 * 2. Seconds array values
 * 3. Audio samples length
 * 4. Audio channels
 * 5. Audio frequency
 * 6. Audio samples array length
 * 7. Audio samples array values
 * 8. Level name
 * 9. Author
 * 10. BPM
 * 11. Positions array length
 * 12. X Positions array values
 * 13. Y Positions array values
 */