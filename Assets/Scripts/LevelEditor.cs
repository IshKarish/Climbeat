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
    public GameObject openPanel;
    public GameObject editorPanel;

    public string name;
    public float[] secs = new float[0];

    public AudioClip song;
    AudioSource audioSource;
    public float[] samples;

    string path;

    public TMP_InputField secInput;
    public TextMeshProUGUI lastSecTxt;
    public GameObject canvas;

    public TextMeshProUGUI[] secTxts = new TextMeshProUGUI[0];

    public TMP_InputField lvlName;
    public TextMeshProUGUI lvlNameTxt;

    public GameObject loadPanel;

    float dTime = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        FileBrowser.AddQuickLink("Climbeat", Application.persistentDataPath, null);
        FileBrowser.AddQuickLink("Downloads", "C:\\Downloads", null);

        StartCoroutine("ShowContDialog");
    }

    public void UploadSong()
    {
        
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Wav", ".wav"));
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.AddQuickLink("Downloads", "C:\\Downloads", null);

        StartCoroutine("ShowLoadDialog");
    }

    public void SaveSong()
    {
        if (lvlNameTxt.text == "")
        {
            name = lvlName.text;
        }
        else
        {
            name = lvlNameTxt.text;
        }

        secs = new float[secTxts.Length];
        for (int i = 0; i < secTxts.Length; i++)
        {
            secs[i] = float.Parse(secTxts[i].text);
        }

        SamplesSave();

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
            name = data.lvlName;
            lvlNameTxt.text = name;

            
            song = AudioClip.Create("Clip", data.samplesLeangth, data.channels, data.frequency, false);
            song.SetData(data.samples, 0);
            audioSource.clip = song;
            audioSource.Play();

            ShowOldSec();
            changePanel(openPanel, editorPanel);
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
            changePanel(openPanel, editorPanel);
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

            lastSecTxt = sec;
            sec.gameObject.SetActive(true);

            TextMeshProUGUI[] newTxts = new TextMeshProUGUI[secs.Length];
            for (int i = 0; i < secTxts.Length; i++)
            {
                newTxts[i] = secTxts[i];
            }
            newTxts[newTxts.Length - 1] = sec;
            secTxts = newTxts;

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

            TextMeshProUGUI[] oldTxts = secTxts;
            secTxts = new TextMeshProUGUI[secTxts.Length + 1];

            for (int j = 0; j < oldTxts.Length; j++)
            {
                secTxts[j] = oldTxts[j];
            }
            secTxts[secTxts.Length - 1] = sec;
        }
        secs = new float[secTxts.Length];
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

        if (secTxt.color == Color.white)
        {
            secTxt.color = Color.blue;
        }
        else if(secTxt.color == Color.blue)
        {
            secTxt.text = secInput.text;
            secTxt.color = Color.white;
        }
    }

    public void DeleteSec()
    {
        string sec = secInput.text;

        for (int i = 0; i < secTxts.Length; i++)
        {
            if(secTxts[i].text == sec)
            {
                Destroy(secTxts[i]);

                for (int j = i; j < secTxts.Length - 1; j++)
                {
                    secTxts[j] = secTxts[j + 1];
                    Debug.Log(j);
                }

                TextMeshProUGUI[] newTxts = new TextMeshProUGUI[secTxts.Length - 1];
                for (int k = 0; k < newTxts.Length; k++)
                {
                    newTxts[k] = secTxts[k];
                }
                secTxts = newTxts;
                lastSecTxt = secTxts[secTxts.Length - 1];

                return;
            }
        }
    }

    IEnumerator LoadingScreen(float loadTime)
    {
        loadPanel.SetActive(true);
        yield return new WaitForSeconds(loadTime);
        loadPanel.SetActive(false);
    }

    void changePanel(GameObject first, GameObject second)
    {
        first.SetActive(false);
        second.SetActive(true);
    }
}
