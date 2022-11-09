using System.Collections;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
    public GameObject ScrollView;

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
}
