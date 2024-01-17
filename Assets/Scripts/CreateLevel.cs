using System.Collections;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using TMPro;
using UnityEngine.Networking;

public class CreateLevel : MonoBehaviour
{    
    [SerializeField] private Transform canvas;
    
    [Header("Texts (TMPro)")]
    [SerializeField] private TMP_InputField levelNameInput;
    [SerializeField] private TMP_InputField authorInput;
    [SerializeField] private TMP_InputField bpmInput;
    
    [Header("Buttons")]
    [SerializeField] private GameObject playBtn;
    
    private string path;
    
    private AudioClip audioClip;
    private AudioSource audioSource;


    private string levelName;
    private string authorName;
    private int bpm;
    
    Editor editor;
    
    //private string presistentDataPath;
    
    private IEnumerator enumerator;

    private void Start()
    {
        enumerator = FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);
    }

    private void Awake()
    {
        editor = GetComponent<Editor>();
        audioSource = GetComponent<AudioSource>();
        //presistentDataPath = Application.persistentDataPath;
    }

    public void SwitchMenu(GameObject menu)
    {
        //Transform canvas = GetComponentInParent<Canvas>().transform;
        for (int i = 0; i < canvas.childCount; i++)
        {
            GameObject currentChild = canvas.GetChild(i).gameObject;
            if(currentChild.activeSelf) currentChild.SetActive(false);
        }
        
        menu.SetActive(true);
    }
    
    public void UploadSong()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Wav file", ".wav"));
        FileBrowser.AddQuickLink("Climbeat", Application.persistentDataPath, null);
        
        StartCoroutine(LoadDialog(true));
    }

    public void LoadLevel()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Not Virus", ".notvirus"));
        FileBrowser.AddQuickLink("Climbeat", Application.persistentDataPath, null);
        
        StartCoroutine(LoadDialog(false));
    }

    IEnumerator LoadDialog(bool getAudio)
    {
        yield return enumerator;
        
        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];
            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));
        }
        
        if(getAudio) StartCoroutine(GetAudio());
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
            audioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
            audioSource.clip = audioClip;
            playBtn.SetActive(true);
        }
    }
    
    public void CreateNewLevel(GameObject editorMenu)
    {
        levelName = levelNameInput.text;
        authorName = authorInput.text;
        
        if(!int.TryParse(bpmInput.text, out bpm)) Debug.LogError("Invalid BPM");

        editor.enabled = true;
        editor.EditorSetting(levelName, authorName, bpm, true);
        SwitchMenu(editorMenu);
    }
}
