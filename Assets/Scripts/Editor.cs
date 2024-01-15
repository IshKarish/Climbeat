using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Editor : MonoBehaviour
{
    private List<float> seconds = new List<float>();
    
    [SerializeField] private TextMeshProUGUI lvlNameTxt;
    [SerializeField] private TextMeshProUGUI authorTxt;
    [SerializeField] private TextMeshProUGUI bpmTxt;

    [SerializeField] private TMP_InputField songTimeInupt;

    [SerializeField] private Transform cursor;

    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform wall;

    private AudioSource audioSource;

    private string presistentDataPath;
    private float screenHeight;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        presistentDataPath = Application.persistentDataPath;
    }

    public void EditorSetting(string lvlName, string author, int bpm, bool IsNewLevel)
    {
        lvlNameTxt.text = lvlName;
        authorTxt.text = author;
        bpmTxt.text = bpm.ToString();

        GetComponentInChildren<Track>().enabled = true;
        
        screenHeight = Screen.height;
    }

    private void Update()
    {
        if (audioSource.clip)
        {
            bool isMouseInBoundsY = Input.mousePosition.y >= 0 && Input.mousePosition.y <= screenHeight;
            bool isMouseInBoundsX = Input.mousePosition.x > 320 && Input.mousePosition.x < 510;

            if (Input.GetMouseButton(0) && isMouseInBoundsY & isMouseInBoundsX)
            {
                float newSongTime;
                
                newSongTime = MoveCursor(Input.mousePosition.y);
                JumpToSecond(newSongTime);
            }
            else
            {
                MoveCursor(FindCursorPos(audioSource.time));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(audioSource.isPlaying) audioSource.Pause();
                else audioSource.Play();
            }
            else songTimeInupt.placeholder.GetComponent<TextMeshProUGUI>().text = audioSource.time.ToString("0.00");

            if (Input.GetKeyDown(KeyCode.Return))
            {
                AddSecond();
            }
        }
    }

    float MoveCursor(float yPos)
    {
        float songTime = yPos / screenHeight * audioSource.clip.length;

        float yCursor = yPos - screenHeight / 2;
        Vector3 position = cursor.position;
        position = new Vector3(position.x, yCursor, position.z);
        cursor.position = position;

        return songTime;
    }

    float FindCursorPos(float songTime)
    {
        float yPos = songTime / audioSource.clip.length * screenHeight;
        return yPos;
    }
    
    void JumpToSecond(float second, bool play = true)
    {
        audioSource.time = second;
        
        if(play) audioSource.Play();
        else audioSource.Pause();
    }

    public void SetSecondManually(TMP_InputField secondInput)
    {
        if (!float.TryParse(secondInput.text, out float second) && second > audioSource.clip.length)
        {
            Debug.LogError("Invalid number");
            second = 0;
        }

        float newCursorYPos = FindCursorPos(second);
        MoveCursor(newCursorYPos);
        
        JumpToSecond(second, false);
        
        songTimeInupt.text = null;
    }

    void AddSecond()
    {
        seconds.Add(audioSource.time);
        Debug.Log(audioSource.time + " added to the list");

        Instantiate(secondVisual());
    }

    GameObject secondVisual()
    {
        float x = wall.position.x;
        Vector3 newPos = new Vector3(x, cursor.position.y, wall.position.z);

        GameObject obj = pointPrefab;
        obj.transform.localPosition = newPos;

        return obj;
    }
}
