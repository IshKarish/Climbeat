
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using SimpleFileBrowser;

public class CreateLevel : MonoBehaviour
{
    LevelEditor editor;
    
    // Level stats
    string lvlName;
    string authorName;
    int bpm;
    
    // New level inputs
    [SerializeField]TMP_InputField nameInput;
    [SerializeField]TMP_InputField authorInput;
    [SerializeField]TMP_InputField bpmInput;

    public GameObject waveform;
    
    List<float> secsLst = new List<float>();

    string path;
    private string presistentDataPath;

    AudioClip song;

    string newSec;

    [HideInInspector] public float[] samples;
    public float[] secs = new float[0];
    
    private IEnumerator _enumerator;

    private void Start()
    {
        _enumerator = FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);
    }

    private void Awake()
    {
        editor = GetComponent<LevelEditor>();
        presistentDataPath = Application.persistentDataPath;
    }

    public void CreateNewLevel()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Wav file", ".wav"));
        FileBrowser.AddQuickLink("Climbeat", Application.persistentDataPath, null);

        StartCoroutine("ShowLoadDialog");
    }

    IEnumerator ShowLoadDialog()
    {
        yield return _enumerator;

        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));
        }
    }

    public void EditExistingLevel()
    {
        
    }
}
