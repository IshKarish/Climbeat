using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Editor : MonoBehaviour
{
     private List<float> secondsLst = new List<float>();

     private List<GameObject> climbPointVisuals = new List<GameObject>();
     private List<GameObject> secondVisuals = new List<GameObject>();

     private List<Vector2> climbPointsPositions = new List<Vector2>();
    
    [Header("Texts (TMPro)")]
    [SerializeField] private TextMeshProUGUI lvlNameTxt;
    [SerializeField] private TextMeshProUGUI authorTxt;
    [SerializeField] private TextMeshProUGUI bpmTxt;
    [Space(5)]
    [SerializeField] private TMP_InputField songTimeInput;
    
    [Header("Walls")]
    [SerializeField] private Transform climbingWall;
    [SerializeField] private Transform secondsWall;
    
    [Header("Other visuals")]
    [SerializeField] private Transform cursor;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject xAxisVisual;
    [SerializeField] private GameObject yAxisVisual;

    private AudioSource audioSource;

    private Camera cam;
    [SerializeField][Range(5, 30)] private float zoomFov = 15;
    private float ogFov;
    private bool isZooming;

    private bool moveMod;

    private float screenWidth;
    private float screenHeight;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;

        if (cam != null) ogFov = cam.fieldOfView;
    }

    public void EditorSetting(string lvlName, string author, int bpm, bool isNewLevel)
    {
        lvlNameTxt.text = lvlName;
        authorTxt.text = author;
        bpmTxt.text = bpm.ToString();

        GetComponentInChildren<Track>().enabled = true;

        screenWidth = Screen.width;
        screenHeight = Screen.height;
        
        if (isNewLevel) SaveLevel();
    }

    private void Update()
    {
        if (audioSource.clip)
        {
            bool isMouseInBoundsY = Input.mousePosition.y >= 0 && Input.mousePosition.y <= screenHeight;
            bool isMouseInBoundsX = Input.mousePosition.x > 320 && Input.mousePosition.x < 510;

            if (Input.GetMouseButton(0) && isMouseInBoundsY & isMouseInBoundsX && !isZooming)
            {
                float newSongTime = MoveCursor(Input.mousePosition.y);
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
            else songTimeInput.placeholder.GetComponent<TextMeshProUGUI>().text = audioSource.time.ToString("0.00");

            if (Input.GetKeyDown(KeyCode.Return))
            {
                AddSecond(true);
            }

            if (moveMod)
            {
                if(Input.GetKey(KeyCode.X)) MovePoint('x');
                else if(Input.GetKey(KeyCode.Y)) MovePoint('y');
                else MovePoint();
            }
            else
            {
                xAxisVisual.SetActive(false);
                yAxisVisual.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(2))
            CameraZoom(!isZooming);
    }

    void CameraZoom(bool zoomIn = true)
    {
        if (!zoomIn)
        {
            isZooming = false;
            
            cam.transform.localPosition = Vector2.zero;
            cam.fieldOfView = ogFov;
            
            return;
        }

        isZooming = true;
        
        Vector3 wallPos = climbingWall.position;
        float yPos = cursor.position.y;

        if (cursor.localPosition.y > 200)
            yPos = 408.5f;
        else if (cursor.localPosition.y < -200)
            yPos = -408.5f;

        cam.transform.localPosition = new Vector2(wallPos.x, yPos);
        cam.fieldOfView = zoomFov;
    }
    
    void MovePoint(char axis = ' ')
    {
        Transform current = EventSystem.current.currentSelectedGameObject.transform;

        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        
        Vector2 mousePos = new Vector2(mouseX, mouseY);
        Vector2 halfScreen = new Vector2(screenWidth, screenHeight) / 2;

        Vector3 currentPos = current.position;
        Vector2 newPos = mousePos - halfScreen;

        Vector2 axisVisualPos = new Vector2(xAxisVisual.transform.position.x, yAxisVisual.transform.position.y);
        switch (axis)
        {
            case 'x':
                current.position = new Vector3(newPos.x, currentPos.y, currentPos.z);
                
                xAxisVisual.SetActive(true);
                yAxisVisual.SetActive(false);
                xAxisVisual.transform.position = new Vector3(axisVisualPos.x, currentPos.y, currentPos.z);
                
                break;
            case 'y':
                current.position = new Vector3(currentPos.x, newPos.y, currentPos.z);
                
                xAxisVisual.SetActive(false);
                yAxisVisual.SetActive(true);
                yAxisVisual.transform.position = new Vector3(currentPos.x, axisVisualPos.y, currentPos.z);
                
                break;
            default:
                xAxisVisual.SetActive(true);
                yAxisVisual.SetActive(true);
                
                xAxisVisual.transform.position = new Vector3(axisVisualPos.x, currentPos.y, currentPos.z);
                yAxisVisual.transform.position = new Vector3(currentPos.x, axisVisualPos.y, currentPos.z);
                
                break;
        }

        for (int i = 0; i < climbPointVisuals.Count; i++)
        {
            if (climbPointVisuals[i] == EventSystem.current.currentSelectedGameObject)
            {
                climbPointsPositions[i] = current.localPosition;
            }
        }
    }

    float MoveCursor(float yPos)
    {
        float songTime = yPos / screenHeight * audioSource.clip.length;

        float yCursor = yPos - screenHeight / 2;
        Vector3 pos = cursor.position;
        pos = new Vector3(pos.x, yCursor, pos.z);
        cursor.position = pos;

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
        
        audioSource.Pause();
        if (play) audioSource.Play();
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
        
        JumpToSecond(second);
        
        songTimeInput.text = null;
    }

    void SetSecondManually(float second)
    {
        float newCursorYPos = FindCursorPos(second);
        MoveCursor(newCursorYPos);
        
        JumpToSecond(second);
        
        AddSecond(false);
    }

    void AddSecond(bool newSecond)
    {
        secondsLst.Add(audioSource.time);

        GameObject secondVisual = PointVisual(secondsWall, 0);
        secondVisuals.Add(secondVisual);
        SetSecondVisualBtn(secondVisual);
        
        GameObject climbPointVisual = PointVisual(climbingWall, Random.Range(100, -100));
        climbPointVisuals.Add(climbPointVisual);
        if (newSecond) climbPointsPositions.Add(climbPointVisual.transform.localPosition);
        else
        {
            Vector2 currentPos = climbPointsPositions[^1];
            Vector3 newPos = new Vector3(currentPos.x, currentPos.y, 0);
            climbPointVisual.transform.localPosition = newPos;
        }
        
        SetClimbPointBtn(climbPointVisual);
    }

    GameObject PointVisual(Transform wall, float xPos)
    {
        Vector3 pos = wall.position;
        xPos += pos.x;
        
        Vector3 newPos = new Vector3(xPos, cursor.position.y, pos.z);

        GameObject obj = Instantiate(pointPrefab);
        obj.transform.localPosition = newPos;
        obj.name = audioSource.time.ToString("0.0000");

        obj.GetComponentInChildren<TextMeshProUGUI>().text = secondsLst.Count.ToString();
        
        if (wall) obj.transform.SetParent(wall);

        return obj;
    }

    void SetClimbPointBtn(GameObject obj)
    {
        Button button = obj.AddComponent<Button>();
        button.onClick.AddListener((Call));
        void Call() {OnVisualBtnPress(5);}

        ButtonLongPressListener longPressListener = obj.AddComponent<ButtonLongPressListener>();
        longPressListener.holdDuration = .5f;
        longPressListener.onLongPress = () => moveMod = true;

        ButtonDoubleClickListener doubleClickListener = obj.AddComponent<ButtonDoubleClickListener>();
        doubleClickListener.onDoubleClick = OnDoubleClick;
        void OnDoubleClick() {OnVisualDoubleClick();}

        EventTrigger trigger = obj.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener((_ => moveMod = false));
        trigger.triggers.Add(entry);
    }

    void SetSecondVisualBtn(GameObject obj)
    {
        Button button = obj.AddComponent<Button>();
        button.onClick.AddListener((Call));
        void Call() {OnVisualBtnPress(5);}

        ButtonDoubleClickListener doubleClickListener = obj.AddComponent<ButtonDoubleClickListener>();
        doubleClickListener.onDoubleClick = OnDoubleClick;
        void OnDoubleClick() {OnVisualDoubleClick();}
    }

    void OnVisualBtnPress(float resetColorDelay)
    {
        FindMatchingVisuals(out Image climbPointImg, out Image secondImg);
        
        StartCoroutine(PaintMatchingVisuals(Color.red, climbPointImg, secondImg, 0));
        JumpToSecond(float.Parse(secondImg.gameObject.name));
        StartCoroutine(PaintMatchingVisuals(Color.black, climbPointImg, secondImg, resetColorDelay));
    }
    
    void OnVisualDoubleClick()
    {
        FindMatchingVisuals(out Image climbPointImg, out Image secondImg);
        RemovePoints(climbPointImg, secondImg);
    }

    void FindMatchingVisuals(out Image climbPointImg, out Image secondImg)
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < secondsLst.Count; i++)
        {
            if (current.name == climbPointVisuals[i].name)
            {
                climbPointImg = climbPointVisuals[i].GetComponent<Image>();
                secondImg = secondVisuals[i].GetComponent<Image>();
                return;
            }
        }

        climbPointImg = null;
        secondImg = null;
    }

    IEnumerator PaintMatchingVisuals(Color color, Image climbPointImg, Image secondImg, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (climbPointImg) climbPointImg.color = color;
        if (secondImg) secondImg.color = color;
    }

    void RemovePoints(Image climbPointImg, Image secondImg)
    {
        secondsLst.Remove(float.Parse(climbPointImg.name));
        
        climbPointVisuals.Remove(climbPointImg.gameObject);
        secondVisuals.Remove(secondImg.gameObject);
        
        Destroy(climbPointImg.gameObject);
        Destroy(secondImg.gameObject);
    }

    public void SaveLevel()
    {
        string levelName = lvlNameTxt.text;
        string author = authorTxt.text;
        int bpm = int.Parse(bpmTxt.text);

        float[] secondsArr = secondsLst.ToArray();
        Vector2[] positionsArr = climbPointsPositions.ToArray();
        SortSecondsAndPositions(secondsArr, positionsArr);
        
        SavesManager savesManager = new SavesManager(levelName, author, bpm, audioSource.clip, secondsArr, positionsArr);
        savesManager.SaveLevel();
    }

    public void LoadLevel(string path)
    {
        SavesManager savesManager = SaveSystem.LoadLevel(path);
        EditorSetting(savesManager.levelName, savesManager.author, savesManager.bpm, false);
        audioSource.clip = savesManager.clip;
        
        for (int i = 0; i < savesManager.seconds.Length; i++)
        {
            Debug.Log(savesManager.seconds[i]);
            climbPointsPositions.Add(savesManager.positions[i]);
            SetSecondManually(savesManager.seconds[i]);
        }

        
        audioSource.Pause();
        audioSource.time = 0;
        audioSource.Play();
    }

    void SortSecondsAndPositions(float[] seconds, Vector2[] positions)
    {
        for (int i = 0; i < seconds.Length; i++)
        {
            for (int j = 0; j < seconds.Length; j++)
            {
                if(seconds[i] < seconds[j])
                {
                    Switch(seconds, i, j);
                    Switch(positions, i, j);
                }
            }
        }
    }
    
    void Switch(float[] arr, int ind1, int ind2)
    {
        (arr[ind1], arr[ind2]) = (arr[ind2], arr[ind1]);
    }
    
    void Switch(Vector2[] arr, int ind1, int ind2)
    {
        (arr[ind1], arr[ind2]) = (arr[ind2], arr[ind1]);
    }
}
