using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public GameObject cursor;
    public GameObject ScrollView;
    public Image waveformTexture;
    public TextMeshProUGUI firstSec;
    public TextMeshProUGUI lastSec;

    List<float> songSecs = new List<float>();

    [Header("Panels")]
    public GameObject openPanel;
    public GameObject editorPanel;
    public GameObject loadPanel;

    [Header("Buttons")]
    public GameObject enterBtn;
    public GameObject delBtn;

    [Header("Inputs")]
    public TMP_InputField lvlNameInput;
    public TMP_InputField secInput;

    [Header("Texts")]
    public TextMeshProUGUI lastSecTxt;
    public TextMeshProUGUI lvlNameTxt;

    [Space(5)]

    TextMeshProUGUI[] secTxts = new TextMeshProUGUI[0];

    AudioSource audioSource;
    [HideInInspector] public AudioClip song;

    [HideInInspector]public float[] samples;
    [HideInInspector] public float[] secs = new float[0];

    string path;
    bool moveCursor = false;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (secInput.text == "")
        {
            delBtn.SetActive(false);
            enterBtn.SetActive(false);
        }

        if(moveCursor)
        {
            for (int i = 0; i < song.length / 2; i++)
            {
                cursor.transform.position = new Vector2(cursor.transform.position.x + .01f, cursor.transform.position.y);
            }
        }
    }

    public void OnInput()
    {
        if (secInput.text == "")
        {
            delBtn.SetActive(false);
            enterBtn.SetActive(false);
        }
        else
        {
            for (int i = 0; i < secTxts.Length; i++) if (secInput.text == secTxts[i].text) delBtn.SetActive(true); else delBtn.SetActive(false);
            enterBtn.SetActive(true);
        }
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

    public void SaveSong()
    {
        if (lvlNameTxt.text == "")
        {
            name = lvlNameInput.text;
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
            Texture2D waveform = PaintWaveformSpectrum(song, 1000, 200, Color.yellow);
            waveformTexture.overrideSprite = Sprite.Create(waveform, new Rect(0, 0, waveform.width, waveform.height), Vector2.zero);

            fillSecs();
            moveCursor = true;
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
            sec.transform.SetParent(ScrollView.transform);
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

            secInput.text = "";

            return sec;
        }
        else
        {
            secInput.text = "";
            enterBtn.SetActive(false);
            if (delBtn.activeSelf) delBtn.SetActive(false);

            Debug.LogError("Not a number. Starting assassination process...");
            return null;
        }
    }

    public void DeleteSec()
    {
        string sec = secInput.text;

        for (int i = 0; i < secTxts.Length; i++)
        {
            if (secTxts[i].text == sec)
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

                secInput.text = "";

                return;
            }
        }
    }

    void ShowOldSec()
    {
        for (int i = 0; i < secs.Length; i++)
        {
            float lastY = lastSecTxt.transform.localPosition.y;

            TextMeshProUGUI sec = Instantiate(lastSecTxt);
            sec.transform.SetParent(ScrollView.transform);
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
            secInput.text = "";
        }
    }

    IEnumerator LoadingScreen(float loadTime)
    {
        loadPanel.SetActive(true);
        yield return new WaitForSeconds(loadTime);
        loadPanel.SetActive(false);
    }

    void changePanel(GameObject first, GameObject second, float loadTime = 0)
    {
        first.SetActive(false);
        if(loadTime > 0) StartCoroutine(LoadingScreen(loadTime));
        second.SetActive(true);

    }

    public Texture2D PaintWaveformSpectrum(AudioClip audio, int width, int height, Color col)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[audio.samples];
        float[] waveform = new float[width];
        audio.GetData(samples, 0);
        int packSize = (audio.samples / width) + 1;
        int s = 0;
        for (int i = 0; i < audio.samples; i += packSize)
        {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.black);
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                tex.SetPixel(x, (height / 2) + y, col);
                tex.SetPixel(x, (height / 2) - y, col);
            }
        }
        tex.Apply();

        return tex;
    }

    void fillSecs()
    {
        float cur = 0;
        while(cur < song.length)
        {
            cur += 0.01f;
            songSecs.Add(cur);
        }
    }
}
