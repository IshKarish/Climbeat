using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject buttonPrefab;
    
    private string[] paths;
    private string[] levels;

    private string levelsFolder;

    private void Awake()
    {
        levelsFolder = Application.persistentDataPath;
        paths = Directory.GetFiles(levelsFolder);
        
        SetLevelList();
    }

    void SetLevelList()
    {
        GameObject button = LevelButton(paths[0]);
        button.transform.localPosition = Vector3.zero;
        for (int i = 1; i < paths.Length; i++)
        {
            Vector3 currentPos = button.transform.localPosition;
            Vector3 nextPos = new Vector3(currentPos.x, currentPos.y - 150, currentPos.z);
            
            button = LevelButton(paths[i]);
            button.transform.localPosition = nextPos;
        }
    }

    GameObject LevelButton(string path)
    {
        GameObject button = Instantiate(buttonPrefab, scrollViewContent, true);
        Button buttonComponent = button.GetComponent<Button>();
        buttonComponent.onClick.AddListener((() => {LoadLevel(path);}));

        SavesManager savesManager = SaveSystem.LoadLevel(path);
        string levelName = savesManager.levelName;
        button.GetComponentInChildren<TextMeshProUGUI>().text = levelName;
        button.name = levelName + " Button";

        return button;
    }

    void LoadLevel(string path)
    {
        PlayerPrefs.SetString("Path", path);
        SceneManager.LoadScene(1);
    }
}
