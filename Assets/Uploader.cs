using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using SimpleFileBrowser;

public class Uploader : MonoBehaviour
{
    public GameObject waveform;

    LevelEditor editor;

    string path;

    AudioClip song;

    private void Awake()
    {
        editor = GetComponent<LevelEditor>();
    }

    public void ContinueSong()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Not Virus", ".notvirus"));
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

    IEnumerator ShowContDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));
            Debug.Log(path);

            LevelData data = SaveSystem.LoadLevel(path);
            editor.secs = data.Secs;
            name = data.lvlName;
            editor.load = true;

            AudioClip clip = AudioClip.Create("Song", data.samplesLeangth / 2, data.channels, data.frequency, false);
            clip.SetData(data.samples, 0);
            editor.audioSource.clip = clip;
            editor.song = clip;

            Debug.Log(clip.length);

            waveform.GetComponent<Track>().GetComponent<AudioSource>().clip = clip;
            waveform.GetComponent<Track>().enabled = true;

            editor.yPointsArr = data.Ypoints;

            editor.FillSecTxts();

            editor.SwitchPanels(editor.openPanel, editor.editorPanel);
        }
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

            editor.SwitchPanels(editor.openPanel, editor.editorPanel);
            editor.song = song;

            Debug.Log(song.length);
            Debug.Log(song.samples * song.channels);
        }
    }
}
