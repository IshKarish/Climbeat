using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
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
    [HideInInspector]public float[] secs = new float[0];

    bool zoom = false;
    Camera cam;
    float songTime = 0;
    float ogFov;
    bool removeMod = false;
    [HideInInspector]public bool load = false;
    bool canMove = true;

    float lastY;

    float[] sora = new float[] {
7.96f,
8.38f,
8.97f,
9.22f,
9.43f,
9.98f,
10.89f,
11.38f,
11.87f,
12.34f,
12.83f,
13.14f,
13.31f,
15.75f,
16.71f,
17.19f,
17.67f,
18.64f,
18.88f,
19.01f,
19.11f,
19.61f,
20.57f,
21.05f,
21.31f,
22.71f,
22.99f,
23.21f,
23.48f,
23.96f,
24.21f,
24.7f,
25.15f,
25.42f,
26.6f,
26.86f,
27.11f,
27.35f,
27.83f,
28.06f,
28.53f,
29.02f,
29.26f,
30.45f,
30.72f,
31.2f,
31.47f,
31.7f,
32f,
32.42f,
32.67f,
34.33f,
34.56f,
34.84f,
35f,
35.31f,
35.55f,
36.18f,
36.53f,
36.78f,
37.22f,
37.51f,
37.76f,
38f,
38.13f,
38.47f,
38.71f,
38.92f,
39.42f,
39.67f,
39.91f,
40.4f,
40.54f,
40.89f,
41.12f,
41.38f,
41.62f,
42.28f,
42.83f,
43.77f,
44.29f,
45.25f,
46.48f,
47.19f,
48.16f,
48.6f,
48.91f,
49.1f,
50.04f,
50.55f,
51.43f,
52.33f,
52.95f,
53.11f,
55.42f,
55.88f,
57.35f,
57.81f,
59.26f,
59.73f,
60.48f,
61.23f,
62.19f,
63.12f,
63.57f,
64.12f,
65.09f,
65.3f,
65.52f,
65.81f,
66f,
66.95f,
67.44f,
67.74f,
69.89f,
70.9f,
71.03f,
71.61f,
71.88f,
72.31f,
72.83f,
72.95f,
73.09f,
73.22f,
73.31f,
73.82f,
74.75f,
75.24f,
75.51f,
76.91f,
77.18f,
77.67f,
78.15f,
78.38f,
78.83f,
79.36f,
79.62f,
80.8f,
81.04f,
81.29f,
81.55f,
82.03f,
82.25f,
82.7f,
83.21f,
84.64f,
85.39f,
85.9f,
86.6f,
86.87f,
87.15f,
88.51f,
88.76f,
89.11f,
89.76f,
90.39f,
90.73f,
91.4f,
91.7f,
91.94f,
92.19f,
92.36f,
92.67f,
92.91f,
93.13f,
93.65f,
94.13f,
94.59f,
94.74f,
95.07f,
95.58f,
95.81f,
96.52f,
96.79f,
97.02f,
97.94f,
98.47f,
99.44f,
100.64f,
101.36f,
101.82f,
102.3f,
102.59f,
102.81f,
103.3f,
103.55f,
104.25f,
104.74f,
105.59f,
106.63f,
107.13f,
107.3f,
109.56f,
110.06f,
111.54f,
112f,
113.45f,
113.93f,
117.35f,
117.78f,
119.28f,
119.74f,
121.22f,
121.66f,
121.42f,
124.15f,
126.04f,
127.66f,
129.93f,
131.59f,
132.85f,
133.55f,
135.47f,
138.36f,
139.06f,
153.84f,
154.28f,
154.56f,
155.05f,
155.58f,
155.81f,
156.04f,
156.54f,
156.63f,
157.01f,
157.51f,
157.74f,
158.47f,
158.95f,
159.43f,
159.97f,
160.01f,
160.42f,
161.39f,
162.57f,
163.3f,
163.54f,
163.76f,
164.24f,
164.69f,
165.01f,
165.24f,
165.49f,
166.17f,
167.1f,
168.49f,
169.09f,
169.34f,
171.57f,
171.99f,
173.47f,
173.94f,
175.4f,
175.85f,
176.61f,
179.24f,
179.75f,
183.12f,
183.61f,
184.33f,
185.58f,
185.82f};

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
                lastY = Input.mousePosition.y;
                songTime = lastY / screenHeight * audioSource.clip.length;

                cursor.position = new Vector3(cursor.position.x, lastY - screenHeight / 2, cursor.position.z);
                timeTxt.text = songTime.ToString("0.00");

                audioSource.time = songTime;
                audioSource.Play();
            }

            if (Input.GetKeyDown(KeyCode.Space)) if (audioSource.isPlaying) audioSource.Stop(); else audioSource.Play();

            if (Input.GetKeyDown(KeyCode.Return)) newSec(songTime.ToString());

            if (Input.GetKey(KeyCode.DownArrow)) ArrowMove(-1);

            if (Input.GetKey(KeyCode.UpArrow)) ArrowMove(1);
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
                float yPos = ((secs[i] / audioSource.clip.length) * Screen.height) - Screen.height / 2;
                newSec(secs[i].ToString(), yPos);
                Debug.Log(i);
            }
        }
        secs = new float[0];
    }

    public void QuitMod()
    {
        quitPanel.SetActive(!quitPanel.activeSelf);
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

        Array.Sort(secs);
    }

    public void SaveSong()
    {
        SamplesSave();
        SecsSave();

        secs = sora;

        if(lvlNameInput.text != "") lvlNameTxt.text = lvlNameInput.text;

        SaveSystem.SaveLevel(this, lvlNameTxt.text);
        Loadcene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

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

            secInput.text = "";

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
        float s = 0;

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
        GameObject soraVR = Instantiate(climbPoint);
        soraVR.transform.localPosition = newPos;
        soraVR.transform.parent = wall.transform;

        if (!soraVR.activeSelf) soraVR.SetActive(true);

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
