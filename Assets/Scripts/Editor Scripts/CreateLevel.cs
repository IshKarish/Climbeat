using System.Collections;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        editor = GetComponent<Editor>();
        audioSource = GetComponent<AudioSource>();
        //presistentDataPath = Application.persistentDataPath;
    }
    
    private void Start()
    {
        enumerator = FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);
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
        
        StartCoroutine(LoadDialog(true, null));
    }

    public void LoadLevel(GameObject editorMenu)
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Not Virus", ".notvirus"));
        FileBrowser.AddQuickLink("Climbeat", Application.persistentDataPath, null);
        
        StartCoroutine(LoadDialog(false, editorMenu));
    }

    IEnumerator LoadDialog(bool getAudio, GameObject editorMenu)
    {
        yield return enumerator;
        
        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];
            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));
        }
        else LoadScene(0);
        
        if(getAudio) StartCoroutine(GetAudio());
        else
        {
            editor.enabled = true;
            editor.LoadLevel(path);
        
            SwitchMenu(editorMenu);
        }
    }

    IEnumerator GetAudio()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, AudioType.WAV);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) LoadScene(0);
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

    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
