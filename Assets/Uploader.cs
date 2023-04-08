using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using SimpleFileBrowser;
using TMPro;

public class Uploader : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField authorInput;
    public TMP_InputField bpmInput;

    public GameObject waveform;

    string lvlName;
    string authorName;
    int bpm;
    List<float> secsLst = new List<float>();
    LevelEditor editor;

    string path;

    AudioClip song;

    string newSec;

    [HideInInspector] public float[] samples;
    public float[] secs = new float[0];

    private void Awake()
    {
        editor = GetComponent<LevelEditor>();
    }

    public void ContinueSong()
    {
        //FileBrowser.SetFilters(false, new FileBrowser.Filter("Not Virus", ".notvirus"));
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.AddQuickLink("Climbeat", Application.persistentDataPath, null);
        //FileBrowser.AddQuickLink("Downloads", "C:\\Downloads", null);

        StartCoroutine("ShowContDialog");
    }

    public void UploadSong()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Wav", ".wav"));
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        //FileBrowser.AddQuickLink("Downloads", "C:\\Downloads", null);

        StartCoroutine("ShowLoadDialog");
    }

    public void StealSong()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Dat", ".dat"));
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        //FileBrowser.AddQuickLink("Downloads", "C:\\Downloads", null);

        StartCoroutine("ShowinfoDialog");
    }

    IEnumerator ShowinfoDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));

            StreamReader reader = new StreamReader(path);
            string levelData = reader.ReadToEnd();
            reader.Close();

            string arr = levelData.Split("\"_songName\": \"")[1];
            char[] cArr = arr.ToCharArray();

            for (int i = 0; i < arr.Length; i++)
            {
                if (cArr[i] != '"') lvlName += cArr[i]; else break;
            }

            arr = levelData.Split("\"_beatsPerMinute\": ")[1];
            cArr = arr.ToCharArray();
            string bpmStr = "";

            for (int i = 0; i < arr.Length; i++)
            {
                if (cArr[i] != ',') bpmStr += cArr[i]; else break;
            }
            bpm = int.Parse(bpmStr);

            arr = levelData.Split("\"_levelAuthorName\": \"")[1];
            cArr = arr.ToCharArray();

            for (int i = 0; i < arr.Length; i++)
            {
                if (cArr[i] != '"') authorName += cArr[i]; else break;
            }

            Debug.Log(lvlName);
            Debug.Log(bpm);
            Debug.Log(authorName);

            FileBrowser.SetFilters(false, new FileBrowser.Filter("Dat", ".dat"));
            FileBrowser.AddQuickLink("Users", "C:\\Users", null);
            //FileBrowser.AddQuickLink("Downloads", "C:\\Downloads", null);

            StartCoroutine("ShowStealDialog");
        }
    }

    IEnumerator ShowStealDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));

            StreamReader reader = new StreamReader(path);
            string levelData = reader.ReadToEnd();
            reader.Close();

            string[] arr = levelData.Split("\"_time\":");
            List<string> lst = new List<string>();
            lst.Add(arr[0]);

            for (int i = 1; i < arr.Length - 1; i++)
            {
                string sec = arr[i];
                char[] cArr = sec.ToCharArray();
                for (int j = 0; j < cArr.Length; j++)
                {
                    if (cArr[j] != ',') newSec += cArr[j]; else break;
                }
                lst.Add(newSec);
                newSec = "";
            }

            arr = lst.ToArray();
            lst = new List<string>();

            for (int i = 1; i < arr.Length; i++)
            {
                if (!lst.Contains(arr[i]))
                {
                    lst.Add(arr[i]);
                    Debug.Log(arr[i]);
                }
            }

            Array.Sort(arr);
            arr = lst.ToArray();

            for (int i = 0; i < arr.Length; i++)
            {
                string str = float.Parse(arr[i]).ToString("0.00");
                secsLst.Add(float.Parse(str));
            }

            FileBrowser.SetFilters(false, new FileBrowser.Filter("Egg", ".ogg", ".egg"));
            FileBrowser.AddQuickLink("Users", "C:\\Users", null);
            //FileBrowser.AddQuickLink("Downloads", "C:\\Downloads", null);

            StartCoroutine("ShowBsDialog");
        }
    }

    IEnumerator ShowContDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders);

        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res)) + "/Normal.notvirus";
            Debug.Log(path);

            LevelData data = SaveSystem.LoadLevel(path);
            editor.secs = data.Secs;
            name = data.lvlName;
            editor.load = true;

            AudioClip clip = AudioClip.Create("Song", data.samplesLeangth / 2, data.channels, data.frequency, false);
            clip.SetData(data.samples, 0);
            editor.audioSource.clip = clip;
            editor.song = clip;

            waveform.GetComponent<Track>().GetComponent<AudioSource>().clip = clip;
            waveform.GetComponent<Track>().enabled = true;

            editor.FillSecTxts();

            editor.lvlNameTxt.text = data.lvlName;

            editor.xPos = data.xPos;
            editor.yPos = data.yPos;
            editor.restoreYpoints();

            editor.authorTxt.text = "Author:\n" + data.authorName;
            editor.bpmTxt.text = data.bpm;

            string lvl = path.Remove(0, (Application.persistentDataPath + "/CustomLevels/").Length);
            Debug.Log(lvl);

            if (lvl.Contains("Easy")) editor.difficultyLevel.value = 0;
            else if (lvl.Contains("Normal")) editor.difficultyLevel.value = 1;
            else if(lvl.Contains("Hard")) editor.difficultyLevel.value = 2;
            else if(lvl.Contains("Expert")) editor.difficultyLevel.value = 3;
            else editor.difficultyLevel.value = 1;

            editor.SwitchPanels(editor.openPanel, editor.editorPanel);
        }
    }

    public void ChangeDifficultyLevel(string path)
    {
        Debug.Log(path);

        LevelData data = SaveSystem.LoadLevel(path);
        editor.secs = data.Secs;
        name = data.lvlName;
        editor.load = true;

        AudioClip clip = AudioClip.Create("Song", data.samplesLeangth / 2, data.channels, data.frequency, false);
        clip.SetData(data.samples, 0);
        editor.audioSource.clip = clip;
        editor.song = clip;

        waveform.GetComponent<Track>().GetComponent<AudioSource>().clip = clip;
        waveform.GetComponent<Track>().enabled = true;

        editor.FillSecTxts();

        editor.lvlNameTxt.text = data.lvlName;

        editor.xPos = data.xPos;
        editor.yPos = data.yPos;
        editor.restoreYpoints();

        editor.authorTxt.text = "Author:\n" + data.authorName;
        editor.bpmTxt.text = data.bpm;

        string lvl = path.Remove(0, (Application.persistentDataPath + "/CustomLevels/").Length);
        Debug.Log(lvl);

        if (lvl.Contains("Easy")) editor.difficultyLevel.value = 0;
        else if (lvl.Contains("Normal")) editor.difficultyLevel.value = 1;
        else if (lvl.Contains("Hard")) editor.difficultyLevel.value = 2;
        else if (lvl.Contains("Expert")) editor.difficultyLevel.value = 3;
        else editor.difficultyLevel.value = 1;

        editor.SwitchPanels(editor.openPanel, editor.editorPanel);
    }

    IEnumerator ShowLoadDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));
            Debug.Log(path);

            StartCoroutine("GetAudio");
        }
    }

    IEnumerator GetAudio()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, AudioType.WAV);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            song = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;

            editor.audioSource.clip = song;

            waveform.GetComponent<Track>().GetComponent<AudioSource>().clip = song;
            waveform.GetComponent<Track>().enabled = true;

            editor.song = song;
            editor.lvlNameTxt.text = nameInput.text;
            editor.authorTxt.text = authorInput.text;
            editor.bpmTxt.text = bpmInput.text;

            editor.SaveSong(false);
            editor.difficultyLevel.value = 1;
            editor.SaveSong(false);
            editor.difficultyLevel.value = 2;
            editor.SaveSong(false);
            editor.difficultyLevel.value = 3;
            editor.SaveSong(false);
            editor.difficultyLevel.value = 1;

            editor.SwitchPanels(editor.openPanel, editor.editorPanel);
        }
    }

    IEnumerator ShowBsDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));
            //path = Path.ChangeExtension(path, ".wav");
            Debug.Log(path);

            StartCoroutine("AudFromBS");
        }
    }

    IEnumerator AudFromBS()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, AudioType.OGGVORBIS);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            song = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;

            editor.audioSource.clip = song;
            editor.song = song;

            float[] secsArr = new float[(int)song.length];

            for (int i = 0; i < secsArr.Length; i++)
            {
                secsArr[i] = secsLst[i];
            }
            
            editor.secs = secsArr;
            editor.xPos = new float[editor.secs.Length];

            for (int i = 0; i < editor.secs.Length; i++)
            {
                //editor.xPos[i] = UnityEngine.Random.Range(-200, 200);
            }

            editor.lvlNameTxt.text = lvlName;
            editor.authorTxt.text = "Author:\n" + authorName;
            editor.bpmTxt.text = bpm.ToString();

            waveform.GetComponent<Track>().GetComponent<AudioSource>().clip = song;
            waveform.GetComponent<Track>().enabled = true;

            editor.FillSecTxts();

            if (path.Contains("Easy")) editor.difficultyLevel.value = 0;
            else if (path.Contains("Normal")) editor.difficultyLevel.value = 1;
            else if (path.Contains("Hard")) editor.difficultyLevel.value = 2;
            else if (path.Contains("Expert")) editor.difficultyLevel.value = 3;
            else editor.difficultyLevel.value = 1;

            editor.SwitchPanels(editor.openPanel, editor.editorPanel);

            //editor.SaveSong();
        }
    }
}
