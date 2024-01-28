using System.IO;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LettersScriptableObject lettersScriptableObject;
    
    private string[] paths;
    private string[] levels;

    private string levelsFolder;

    private void Awake()
    {
        levelsFolder = Application.persistentDataPath;
        paths = Directory.GetFiles(levelsFolder);
        
        SetLevelTexts();
    }

    void SetLevelTexts()
    {
        GameObject newText = AddLevelText(paths[0]);
        newText.transform.localScale = new Vector3(-1, 1, 1);
      
        for (int i = 1; i < paths.Length; i++)
        {
            Vector3 newPos = new Vector3(0, newText.transform.position.y - 1.5f, 0);
            
            newText = AddLevelText(paths[i]);
            newText.transform.localScale = new Vector3(-1, 1, 1);
            newText.transform.position = newPos;
        }
    }

    GameObject AddLevelText(string path)
    {
        Writer writer = new Writer(lettersScriptableObject);
        SavesManager savesManager = SaveSystem.LoadLevel(path);
        
        return writer.Write(savesManager.levelName);
    }
}
