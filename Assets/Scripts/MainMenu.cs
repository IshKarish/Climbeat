using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform pivot;

    string path;
    string[] paths = new string[0];
    float time = 0;

    GameManager gameManager;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        paths = Directory.GetFiles(Application.persistentDataPath);
        for (int i = 0; i < 1; i++)
        {
            LevelData data = SaveSystem.LoadLevel(paths[i]);
            gameManager.write(data.lvlName, pivot.transform.position);
        }
    }

    public void Play()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Not Virus", ".notvirus"));
        FileBrowser.SetDefaultFilter(".notvirus");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.ShowHiddenFiles = true;

        StartCoroutine("ShowLoadDialog");
    }

    IEnumerator ShowLoadDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if (FileBrowser.Success)
        {
            string res = FileBrowser.Result[0];

            path = Path.Combine(FileBrowserHelpers.GetDirectoryName(res), FileBrowserHelpers.GetFilename(res));
            Debug.Log(path);

            PlayerPrefs.SetString("Path", path);
            Debug.Log(Time.time);

            PlayerPrefs.SetFloat("dTime", time);

            SceneManager.LoadScene(1);
        }
    }

    public void LoadScene(int ind)
    {
        SceneManager.LoadScene(ind);
    }
}
