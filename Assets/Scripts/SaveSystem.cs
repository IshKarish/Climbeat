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
        for (int i = 0; i < savesManager.seconds.Length; i++)
        {
            writer.Write(savesManager.seconds[i]);
        }

        writer.Write(savesManager.samplesLength);
        writer.Write(savesManager.channels);
        writer.Write(savesManager.frequency);

        writer.Write(savesManager.samples.Length);
        for (int i = 0; i < savesManager.samples.Length; i++)
        {
            writer.Write(savesManager.samples[i]);
        }

        writer.Write(savesManager.levelName);
        writer.Write(savesManager.author);
        writer.Write(savesManager.bpm);

        //writer.Write(data.xPos.Length);
        //for (int i = 0; i < data.xPos.Length; i++)
        //{
        //    writer.Write(data.xPos[i]);
        //}
//
        //writer.Write(data.yPos.Length);
        //for (int i = 0; i < data.yPos.Length; i++)
        //{
        //    writer.Write(data.yPos[i]);
        //}

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

            string lvlName = reader.ReadString();
            string authorName = reader.ReadString();
            int bpm = reader.ReadInt32();

            SavesManager savesManager = new SavesManager(lvlName, authorName, bpm, clip, secondsArr);
//
            //int xLength = reader.ReadInt32();
            //savesManager.xPos = new float[xLength];
            //for (int i = 0; i < savesManager.xPos.Length; i++)
            //{
            //    savesManager.xPos[i] = reader.ReadSingle();
            //}
//
            //int yLength = reader.ReadInt32();
            //savesManager.yPos = new float[yLength];
            //for (int i = 0; i < savesManager.yPos.Length; i++)
            //{
            //    savesManager.yPos[i] = reader.ReadSingle();
            //}

            stream.Close();

            return savesManager;
        }
        else
        {
            Debug.LogError("There is no level file. starting assassination process...");
            return null;
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
     * 11. X Positions array length
     * 12. X Positions array values
     * 13. Y Positions array length
     * 14. Y Positions array values
     */
}
