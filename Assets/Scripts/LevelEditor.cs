using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
    public float[] secs = new float[0];

    public AudioClip song;
    AudioSource audioSource;
    public float[] samples;

    string path;

    public TMP_InputField secInput;
    public TextMeshProUGUI lastSecTxt;
    public GameObject canvas;

    //TextMeshProUGUI[] secTxts = new TextMeshProUGUI[0];

    public TMP_InputField lvlName;
    public TextMeshProUGUI lvlNameTxt;

    float dTime = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        dTime = PlayerPrefs.GetFloat("dTime");
    }

    private void Update()
    {
        dTime += Time.deltaTime;
    }

    private void SamplesSave()
    {
        samples = new float[song.samples * song.channels];
        song.GetData(samples, 0);

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = samples[i] * .5f;
        }
    }

    public void ContinueSong()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Not Virus", ".notvirus"));
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        StartCoroutine("ShowContDialog");
    }

    public void UploadSong()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Wav", ".wav"));
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        StartCoroutine("ShowLoadDialog");
    }

    public void SaveSong()
    {
        string name;

        if(lvlNameTxt.text == "")
        {
            name = lvlName.text;
        }
        else
        {
            name = lvlNameTxt.text;
        }
        SaveSystem.SaveLevel(this, name);
    }

    IEnumerator ShowContDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if(FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));
            Debug.Log(path);

            LevelData data = SaveSystem.LoadLevel(path);
            secs = data.Secs;
            lvlNameTxt.text = data.lvlName;

            /*
            song = AudioClip.Create("Clip", data.samplesLeangth, data.channels, data.frequency, false);
            song.SetData(data.samples, 0);
            audioSource.clip = song;
            audioSource.Play();
            */

            ShowOldSec();
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

            audioSource.clip = song;
            audioSource.Play();

            SamplesSave();
        }
    }

    public void EnterSecond()
    {
        newSec();
    }

    TextMeshProUGUI newSec()
    {
        if(float.TryParse(secInput.text, out float f))
        {
            float[] oldSecs = secs;
            secs = new float[oldSecs.Length + 1];

            for (int i = 0; i < oldSecs.Length; i++)
            {
                secs[i] = oldSecs[i];
            }

            TextMeshProUGUI sec = Instantiate(lastSecTxt);
            sec.transform.SetParent(canvas.transform);
            sec.text = secInput.text;
            sec.name = "SecTxt" + secs.Length;

            Vector2 lastPos = sec.rectTransform.position;
            Debug.Log(lastPos);

            sec.transform.localScale = Vector3.one;
            sec.rectTransform.localPosition = new Vector2(lastPos.x, lastPos.y + 200);

            secs[secs.Length - 1] = float.Parse(secInput.text);

            lastSecTxt = sec;
            sec.gameObject.SetActive(true);

            /*
            TextMeshProUGUI[] newTxts = new TextMeshProUGUI[secs.Length];
            for (int i = 0; i < secTxts.Length; i++)
            {
                newTxts[i] = secTxts[i];
            }
            newTxts[newTxts.Length - 1] = sec;
            secTxts = newTxts;
            */

            return sec;
        }
        else
        {
            Debug.LogError("Not a number. Starting assassination process...");
            return null;
        }
    }

    void ShowOldSec()
    {
        for (int i = 0; i < secs.Length; i++)
        {
            float lastY = lastSecTxt.transform.localPosition.y;

            TextMeshProUGUI sec = Instantiate(lastSecTxt);
            sec.transform.SetParent(canvas.transform);
            sec.text = secs[i].ToString();
            sec.name = "SecTxt" + secs.Length;

            Vector2 lastPos = sec.rectTransform.position;
            Debug.Log(lastPos);

            sec.transform.localScale = Vector3.one;
            sec.rectTransform.localPosition = new Vector2(0, lastY -50);

            lastSecTxt = sec;
            sec.gameObject.SetActive(true);
        }
    }

    public void LoadScene(int ind)
    {
        PlayerPrefs.SetFloat("dTime", dTime);
        SceneManager.LoadScene(ind);
    }

    public void ChangeSec()
    {
        GameObject secObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<TextMeshProUGUI>().gameObject;
        TextMeshProUGUI secTxt = secObj.GetComponentInParent<TextMeshProUGUI>();
        float sec = float.Parse(secTxt.text);

        if (secTxt.color == Color.white)
        {
            secTxt.color = Color.blue;
        }
        else if(secTxt.color == Color.blue)
        {
            for (int i = 0; i < secs.Length; i++)
            {
                if(secs[i] == sec)
                {
                    secs[i] = float.Parse(secInput.text);
                    secTxt.text = secInput.text;
                }
            }
            secTxt.color = Color.white;
        }
    }
}
