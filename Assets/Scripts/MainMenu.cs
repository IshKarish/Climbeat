using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform pivot;
    public Vector3 lvlTxtSize = new Vector3(0.6f, 0.6f, 0.6f);
    public Material chooseMat;
    public Material txtMat;

    string path;
    string[] paths = new string[0];
    string[] lvls = new string[0];
    GameObject[] lvlTxts = new GameObject[0];
    GameObject curLvl;
    float time = 0;

    GameManager gameManager;

    private void Awake()
    {
        PlayerPrefs.DeleteKey("Path");
        gameManager = FindObjectOfType<GameManager>();
        //hvr = h.GetComponent<HVRHexaBodyInputs>();
    }

    private void Start()
    {
        string levelsFolder = Application.persistentDataPath + "/CustomLevels/";

        paths = Directory.GetDirectories(levelsFolder);

        for (int i = 0; i < paths.Length; i++)
        {
            //Debug.Log(paths[i]);
        }
        
        if(paths.Length > 0)
        {
            lvls = new string[paths.Length];
            lvlTxts = new GameObject[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                //LevelData data = SaveSystem.LoadLevel(paths[i]);
                lvls[i] = paths[i];
                //Debug.Log(paths[i]);

                GameObject obj = gameManager.write(lvls[i].Remove(0, levelsFolder.Length), pivot.transform.position, lvlTxtSize);

                pivot.position = new Vector3(pivot.position.x, pivot.position.y - 1, pivot.position.z);

                lvlTxts[i] = obj;
            }

            ChangeTxtMat(0, chooseMat);
        }
    }

    void ChangeTxtMat(int ind, Material mat)
    {
        curLvl = lvlTxts[ind];
        path = lvls[ind];
        for (int i = 0; i < lvlTxts[ind].transform.childCount; i++)
        {
            curLvl.transform.GetChild(i).GetComponent<Renderer>().material = mat;
        }

        if(mat == chooseMat)
        {
            path = paths[ind];
            Debug.Log(path);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || gameManager.inputs.RightController.JoystickAxis.y < 0.1f) LvlDown();

        if (Input.GetKeyDown(KeyCode.UpArrow) || gameManager.inputs.RightController.JoystickAxis.y > 0.1f) LvlUp();

        if (Input.GetKeyDown(KeyCode.E)) EnterLvl("easy");
        if (Input.GetKeyDown(KeyCode.N)) EnterLvl("normal");
        if (Input.GetKeyDown(KeyCode.H)) EnterLvl("hard");
        if (Input.GetKeyDown(KeyCode.X)) EnterLvl("expert");
    }

    public void LvlDown()
    {
        for (int i = 0; i < lvlTxts.Length; i++)
        {
            if (lvlTxts[i] == curLvl && curLvl != lvlTxts[lvlTxts.Length - 1])
            {
                ChangeTxtMat(i, txtMat);
                ChangeTxtMat(i + 1, chooseMat);
                break;
            }
        }
    }

    public void LvlUp()
    {
        for (int i = 0; i < lvlTxts.Length; i++)
        {
            if (lvlTxts[i] == curLvl && curLvl != lvlTxts[0])
            {
                ChangeTxtMat(i, txtMat);
                ChangeTxtMat(i - 1, chooseMat);
                break;
            }
        }
    }

    public void EnterLvl(string difficulty = "normal")
    {
        if (difficulty == "easy") StartLevel(path + "/Easy.fuckunity");
        else if (difficulty == "normal") StartLevel(path + "/Normal.fuckunity");
        else if (difficulty == "hard") StartLevel(path + "/Hard.fuckunity");
        else if (difficulty == "expert") StartLevel(path + "/Expert.fuckunity");
    }

    void StartLevel(string path)
    {
        if(isEdited(path))
        {
            PlayerPrefs.SetString("Path", path);
            LoadScene(1);
        }
        else
        {
            Debug.Log("Nothing at " + path);
        }
    }

    bool isEdited(string path)
    {
        LevelData levelData = SaveSystem.LoadLevel(path);
        return levelData.Secs.Length > 0;
    }

    public void Play()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Fuck Unity", ".fuckunity"));
        FileBrowser.SetDefaultFilter(".fuckunity");
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
