using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
    public TMP_Dropdown difficultyLevel;

    public float[] xPos;
    public List<GameObject> pointsVisuals = new List<GameObject>();

    [Header("Scripts")]
    public Track track;

    [Header("Numbers")]
    [Range(100, 1000)] public float moveSpeed;
    [Range(.1f, 1)] public float moveDelay;
    [Range(5, 30)] public float zoomedFov;

    [Header("Panels")]
    public GameObject openPanel;
    public GameObject editorPanel;
    public GameObject loadPanel;
    public GameObject savePanel;
    public GameObject quitPanel;

    [Header("Buttons")]
    public GameObject enterBtn;
    public GameObject delBtn;

    [Header("Inputs")]
    public TMP_InputField lvlNameInput;
    public TMP_InputField secInput;

    [Header("Texts")]
    public TextMeshProUGUI timeTxt;
    public TextMeshProUGUI lastSecTxt;
    public TextMeshProUGUI lvlNameTxt;

    [Header("Visuals")]
    public GameObject wall;
    public GameObject climbPoint;
    public GameObject ScrollView;
    public Transform cursor;
    public GameObject fakeWaveform;

    [Space(5)]

    List<TextMeshProUGUI> secTxts = new List<TextMeshProUGUI>();
    List<GameObject> points = new List<GameObject>();
    GameObject lastPoint = null;

    [HideInInspector]public AudioSource audioSource;
    [HideInInspector] public AudioClip song;

    [HideInInspector]public float[] samples;
    public float[] secs = new float[0];

    GameObject soraVR;
    bool zoom = false;
    Camera cam;
    float songTime = 0;
    float ogFov;
    bool removeMod = false;
    [HideInInspector]public bool load = false;
    bool canMove = true;
    bool moveCursor = false;
    bool moveMode = false;

    float lastY;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;
        ogFov = cam.fieldOfView;
    }

    private void Update()
    {
        if(audioSource.clip != null)
        {
            float screenHeight = Screen.height;

            if (Input.GetMouseButton(0) && Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height && Input.mousePosition.x > 320 && Input.mousePosition.x < 510 && !zoom && lastY != Input.mousePosition.y)
            {
                moveCursor = false;
                lastY = Input.mousePosition.y;
                songTime = lastY / screenHeight * audioSource.clip.length;

                cursor.position = new Vector3(cursor.position.x, lastY - screenHeight / 2, cursor.position.z);
                timeTxt.text = songTime.ToString("0.00");

                audioSource.time = songTime;
                audioSource.Play();
                moveCursor = true;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                audioSource.time = songTime;
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    moveCursor = false;
                }
                else
                {
                    audioSource.Play();
                    moveCursor = true;
                }
            }

            if(moveCursor)
            {
                songTime = audioSource.time;
                lastY = songTime / audioSource.clip.length * screenHeight;

                timeTxt.text = songTime.ToString("0.00");
                cursor.position = new Vector3(cursor.position.x, lastY - screenHeight / 2, cursor.position.z);
            }

            if (Input.GetKeyDown(KeyCode.Return)) newSec(songTime.ToString());

            if (Input.GetKey(KeyCode.DownArrow)) ArrowMove(-1);

            if (Input.GetKey(KeyCode.UpArrow)) ArrowMove(1);

            if(moveMode && !removeMod)
            {
                float x = Input.mousePosition.x;
                GameObject current = EventSystem.current.currentSelectedGameObject;

                RectTransform rectTransform = current.GetComponent<RectTransform>();
                float newX = x - Screen.width / 2;
                rectTransform.localPosition = new Vector3(newX, rectTransform.localPosition.y, 0);

                for (int i = 0; i < xPos.Length; i++)
                {
                    if(current == pointsVisuals[i])
                    {
                        xPos[i] = newX;
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Z))
        {
            if (!zoom)
            {
                if (cursor.localPosition.y > 200)
                    cam.transform.localPosition = new Vector3(fakeWaveform.transform.position.x, 302.5f, cam.transform.localPosition.z);
                else if (cursor.localPosition.y < -200)
                    cam.transform.localPosition = new Vector3(fakeWaveform.transform.position.x, -302.5f, cam.transform.localPosition.z);
                else cam.transform.localPosition = new Vector3(fakeWaveform.transform.position.x, cursor.position.y, cam.transform.localPosition.z);
                cam.fieldOfView = zoomedFov;

                zoom = true;
            }
            else
            {
                cam.transform.localPosition = Vector3.zero;
                cam.fieldOfView = ogFov;

                zoom = false;
            }
        }

        if (zoom && cursor.localPosition.y < 200 && cursor.localPosition.y > -200)
            cam.transform.localPosition = new Vector3(cam.transform.position.x, cursor.position.y, cam.transform.localPosition.z);

        lastY = Input.mousePosition.y;
    }

    #region Saving functions
    public void SaveMod()
    {
        if (lvlNameTxt.text == "") savePanel.SetActive(!savePanel.activeSelf); else SaveSong();
    }

    void SamplesSave()
    {
        samples = new float[song.samples * song.channels];
        song.GetData(samples, 0);
    }

    void SecsSave()
    {
        TextMeshProUGUI[] secTxtsArr = secTxts.ToArray();
        secs = new float[secTxtsArr.Length];

        for (int i = 0; i < secTxtsArr.Length; i++)
        {
            secs[i] = float.Parse(secTxtsArr[i].text);
        }

        Sort(secs, xPos);
    }

    public void SaveSong()
    {
        //audioSource.Play();

        SamplesSave();
        SecsSave();

        if (lvlNameInput.text != "")
        {
            lvlNameTxt.text = lvlNameInput.text;
        }

        string path = "";
        string levelsFolder = Application.persistentDataPath + "/CustomLevels/";
        string[] paths = Directory.GetDirectories(levelsFolder);

        for (int i = 0; i < paths.Length; i++)
        {
            string folder = paths[i].Remove(levelsFolder.Length);
            if (folder == lvlNameTxt.text)
            {
                path = folder;
                SaveSystem.SaveLevel(this, difficultyLevel.GetComponent<TextMeshProUGUI>().text, path);
                Loadcene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        Directory.CreateDirectory(levelsFolder + "/" + lvlNameTxt.text);
        path = levelsFolder + "/" + lvlNameTxt.text;

        SaveSystem.SaveLevel(this, difficultyLevel.GetComponentInChildren<TextMeshProUGUI>().text, path);
        Loadcene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    void Sort(float[] secsArr, float[] yPosArr)
    {
        for (int i = 0; i < secsArr.Length; i++)
        {
            for (int j = 0; j < secsArr.Length; j++)
            {
                if(secsArr[i] < secsArr[j])
                {
                    Switch(secsArr, i, j);
                    Switch(yPosArr, i, j);
                }
            }
        }
    }

    void Switch(float[] arr, int ind1, int ind2)
    {
        float temp = arr[ind1];
        arr[ind1] = arr[ind2];
        arr[ind2] = temp;
    }

    public void MoveOn()
    {
        moveMode = true;
    }

    public void MoveOff()
    {
        moveMode = false;
    }

    IEnumerator LockMove()
    {
        canMove = false;
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }

    void ArrowMove(float math)
    {
        if(canMove && cursor.position.y > -537 && cursor.position.y < 537)
        {
            float s = moveSpeed / audioSource.clip.length;
            if (math < 0) lastY -= s; else if (math > 0) lastY += s;

            float screenHeight = Screen.height;
            cursor.position = new Vector3(cursor.position.x, lastY - screenHeight / 2, cursor.position.z);

            songTime = lastY / screenHeight * audioSource.clip.length;

            audioSource.time = songTime;
            audioSource.Play();

            timeTxt.text = songTime.ToString("0.00");
            StartCoroutine(LockMove());
        }
    }

    public void FillSecTxts()
    {
        if (secs.Length > 0)
        {
            secTxts = new List<TextMeshProUGUI>();
            for (int i = 0; i < secs.Length; i++)
            {
                float yPosition = ((secs[i] / audioSource.clip.length) * Screen.height) - Screen.height / 2;
                newSec(secs[i].ToString(), yPosition);
                Debug.Log(i);
            }
        }
        secs = new float[0];
    }

    public void restoreYpoints()
    {
        if(xPos.Length > 0)
        {
            for (int i = 0; i < xPos.Length; i++)
            {
                RectTransform rect = pointsVisuals[i].GetComponent<RectTransform>();
                rect.localPosition = new Vector3(xPos[i], rect.localPosition.y, rect.localPosition.z);
            }
        }
    }

    public void QuitMod()
    {
        quitPanel.SetActive(!quitPanel.activeSelf);
    }

    #region Seconds functions
    public void OnPointClick()
    {
        if (removeMod) RemoveSec(); else playFromSec();
    }

    public void RemoveMod()
    {
        if(points.Count > 0)
        {
            GameObject[] pointsArr = points.ToArray();

            for (int i = 0; i < pointsArr.Length; i++)
            {
                Image img = pointsArr[i].GetComponent<Image>();
                if (img.color == Color.black)
                {
                    img.color = Color.blue;
                    removeMod = true;
                }
                else
                {
                    img.color = Color.black;
                    removeMod = false;
                }
            }
        }
    }

    public void playFromSec()
    {
        GameObject[] pointsArr = points.ToArray();
        TextMeshProUGUI[] secsArr = secTxts.ToArray();

        for (int i = 0; i < pointsArr.Length; i++)
        {
            if(EventSystem.current.currentSelectedGameObject == pointsArr[i])
            {
                audioSource.time = float.Parse(secsArr[i].text);
                audioSource.Play();
                cursor.transform.position = new Vector3(cursor.position.x, EventSystem.current.currentSelectedGameObject.transform.position.y, cursor.position.z);
            }
        }
    }

    public void EnterSecond()
    {
        newSec(secInput.text);
    }

    public TextMeshProUGUI newSec(string secToAdd, float y = 0)
    {
        if(float.TryParse(secToAdd, out float f))
        {
            GameObject climbPoint = climbpointVisual(y);
            lastPoint = climbPoint;
            climbPoint.name = secToAdd;
            points.Add(climbPoint);
            
            TextMeshProUGUI sec = Instantiate(lastSecTxt);
            sec.transform.SetParent(ScrollView.transform);
            sec.text = secToAdd;
            sec.name = secToAdd;

            Vector2 lastPos = sec.rectTransform.position;
            Debug.Log(lastPos);

            sec.transform.localScale = Vector3.one;
            sec.rectTransform.localPosition = new Vector2(lastPos.x, lastPos.y + 200);

            lastSecTxt = sec;
            sec.gameObject.SetActive(true);

            secTxts.Add(sec);

            return sec;
        }
        else
        {
            secInput.text = "";
            enterBtn.SetActive(false);
            if (delBtn.activeSelf) delBtn.SetActive(false);

            Debug.LogError("Not a number. Starting assassination process...");
            return null;
        }
    }

    GameObject climbpointVisual(float y = 0)
    {
        float x = wall.transform.position.x;
        if (lastPoint)
        {
            if (lastPoint.transform.position.x > 100) x = lastPoint.transform.position.x + UnityEngine.Random.Range(-100, 50);
            else if (lastPoint.transform.position.x < -100) x = lastPoint.transform.position.x + UnityEngine.Random.Range(50, 100);
            else x = lastPoint.transform.position.x + UnityEngine.Random.Range(-100, 100);
        }

        Vector3 newPos = new Vector3(x, track.cursor.position.y, wall.transform.position.z); ;
        if (y != 0)
        {
            newPos = new Vector3(x, y, wall.transform.position.z);
        }
        soraVR = Instantiate(climbPoint);
        soraVR.transform.localPosition = newPos;
        soraVR.transform.parent = wall.transform;

        if (!soraVR.activeSelf) soraVR.SetActive(true);

        pointsVisuals.Add(soraVR);

        xPos = new float[pointsVisuals.Count];
        for (int i = 0; i < xPos.Length; i++)
        {
            xPos[i] = pointsVisuals[i].GetComponent<RectTransform>().localPosition.x;
        }

        return soraVR;
    }

    public void RemoveSec()
    {
        if(removeMod)
        {
            GameObject[] pointsArr = points.ToArray();
            TextMeshProUGUI[] secsArr = secTxts.ToArray();
            GameObject cur = EventSystem.current.currentSelectedGameObject;

            for (int i = 0; i < pointsArr.Length; i++)
            {
                if(cur == pointsArr[i])
                {
                    for (int j = 0; j < secsArr.Length; j++)
                    {
                        if (secsArr[j].text == pointsArr[i].name)
                        {
                            Destroy(pointsArr[i]);
                            Destroy(secTxts[j]);

                            secTxts.RemoveAt(j);
                            points.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            RemoveMod();
        }
    }
    #endregion

    public void SwitchPanels(GameObject p1, GameObject p2)
    {
        p1.SetActive(false);
        p2.SetActive(true);
    }

    public void Loadcene(int ind)
    {
        SceneManager.LoadScene(ind);
    }
}
