using UnityEngine;
using System.IO;

public static class SaveSystem
{
    //static string path = Application.persistentDataPath + "/CustomLevels";
    static string format = ".notvirus";

    public static void SaveLevel (LevelEditor editor, string name, string path)
    {
        string sPath = path + "/" + name + format;

        FileStream stream = new FileStream(path + "/" + name + format, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(stream);

        LevelData data = LevelData.FromLevel(editor);

        writer.Write(data.Secs.Length);
        for (int i = 0; i < data.Secs.Length; i++)
        {
            writer.Write(data.Secs[i]);
        }

        writer.Write(data.samplesLeangth);
        writer.Write(data.channels);
        writer.Write(data.frequency);

        writer.Write(data.samples.Length);
        for (int i = 0; i < data.samples.Length; i++)
        {
            writer.Write(data.samples[i]);
        }

        writer.Write(data.lvlName);

        writer.Write(data.xPos.Length);
        for (int i = 0; i < data.xPos.Length; i++)
        {
            writer.Write(data.xPos[i]);
        }

        stream.Close();
    }

    public static LevelData LoadLevel (string path)
    {
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);

            LevelData data = new LevelData();

            int len = reader.ReadInt32();
            data.Secs = new float[len];
            for (int i = 0; i < len; i++)
            {
                data.Secs[i] = reader.ReadSingle();
            }

            data.samplesLeangth = reader.ReadInt32();
            data.channels = reader.ReadInt32();
            data.frequency = reader.ReadInt32();

            int sLength = reader.ReadInt32();
            data.samples = new float[sLength];
            for (int i = 0; i < data.samples.Length; i++)
            {
                data.samples[i] = reader.ReadSingle();
            }

            data.lvlName = reader.ReadString();

            int yLength = reader.ReadInt32();
            data.xPos = new float[yLength];
            for (int i = 0; i < data.xPos.Length; i++)
            {
                data.xPos[i] = reader.ReadSingle();
            }

            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("There is no level file. starting assassination process...");
            return null;
        }
    }
}
