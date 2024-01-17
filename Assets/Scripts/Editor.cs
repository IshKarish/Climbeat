using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    [SerializeField] private GameObject xAxisVisual;
    [SerializeField] private GameObject yAxisVisual;
    
    [SerializeField] private List<float> secondsLst = new List<float>();

    [SerializeField] private List<GameObject> climbPointVisuals = new List<GameObject>();
    [SerializeField] private List<GameObject> secondVisuals = new List<GameObject>();
    
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
                AddSecond();
            }

            if (moveMod)
            {
                Debug.Log("Move Mode");
                
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
        
        songTimeInput.text = null;
    }

    void AddSecond()
    {
        secondsLst.Add(audioSource.time);
        Debug.Log(audioSource.time + " added to the list");

        GameObject secondVisual = PointVisual(secondsWall, 0);
        secondVisuals.Add(secondVisual);
        
        GameObject climbPointVisual = PointVisual(climbingWall, Random.Range(100, -100));
        climbPointVisuals.Add(climbPointVisual);
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
        
        if (wall) obj.transform.SetParent(wall);

        return obj;
    }

    void SetClimbPointBtn(GameObject obj)
    {
        Button button = obj.AddComponent<Button>();
        button.onClick.AddListener(OnClimbPointPress(5));

        ButtonLongPressListener longPressListener = obj.AddComponent<ButtonLongPressListener>();
        longPressListener.holdDuration = 3;
        longPressListener.onLongPress = () => moveMod = true;

        EventTrigger trigger = obj.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener((_ => moveMod = false));
        trigger.triggers.Add(entry);
    }

    UnityAction OnClimbPointPress(float resetColorDelay)
    {
        FindMatchingVisuals(out Image climbPointImg, out Image secondImg);
        
        StartCoroutine(PaintMatchingVisuals(Color.red, climbPointImg, secondImg, 0));
        StartCoroutine(PaintMatchingVisuals(Color.black, climbPointImg, secondImg, resetColorDelay));
        
        return null;
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
        
        climbPointImg.color = color;
        secondImg.color = color;
    }
}
