using System.Collections;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using TMPro;
using UnityEngine.Networking;

public class CreateLevel : MonoBehaviour
{
    private string path;
    
    private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject playBtn;

    [SerializeField] private TMP_InputField levelNameInput;
    [SerializeField] private TMP_InputField authorInput;
    [SerializeField] private TMP_InputField bpmInput;

    [HideInInspector] public string levelName;
    [HideInInspector] public string authorName;
    [HideInInspector] public int bpm;
    
    //LevelEditor editor;
    
    //private string presistentDataPath;
    
    private IEnumerator enumerator;

    private void Start()
    {
        enumerator = FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);
    }

    private void Awake()
    {
        //editor = GetComponent<LevelEditor>();
        //presistentDataPath = Application.persistentDataPath;
    }

    public void SwitchMenu(GameObject menu)
    {
        Transform canvas = GetComponentInParent<CreateLevel>().transform;
        for (int i = 0; i < canvas.childCount; i++)
        {
            GameObject currentChild = canvas.GetChild(i).gameObject;
            if(currentChild.activeSelf) currentChild.SetActive(false);
        }
        
        menu.SetActive(true);
    }

    void CreateNewLevel()
    {
        levelName = levelNameInput.text;
        authorName = authorInput.text;
        
        if(!int.TryParse(bpmInput.text, out bpm)) Debug.LogError("Invalid BPM");
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
        
        StartCoroutine(GetAudio());
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
}
