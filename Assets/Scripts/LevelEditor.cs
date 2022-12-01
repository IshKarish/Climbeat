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

    [Header("Panels")]
    public GameObject openPanel;
    public GameObject editorPanel;
    public GameObject loadPanel;

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
    List<float> yPoints = new List<float>();
    GameObject lastPoint = null;

    [HideInInspector]public AudioSource audioSource;
    [HideInInspector] public AudioClip song;

    public float[] samples;
    public float[] secs = new float[0];
    public float[] yPointsArr = new float[0];

    bool zoom = false;
    Camera cam;
    float songTime = 0;

    bool removeMod = false;
    public bool load = false;
    bool canMove = true;

    float[] sora = new float[] { 1.14f,
1.31f,
1.51f,
1.9f,
2.28f,
2.48f,
2.63f,
2.98f,
3.43f,
3.61f,
3.78f,
4.09f,
4.38f,
4.58f,
4.74f,
4.9f,
5.21f,
5.4f,
5.67f,
5.83f,
6.02f,
6.35f,
6.76f,
6.95f,
7.11f,
7.83f,
8.03f,
8.24f,
8.61f,
8.83f,
9f,
9.19f,
9.34f,
9.71f,
10.47f,
11.21f,
12.29f,
12.53f,
12.97f,
13.27f,
13.48f,
13.7f,
13.86f,
14.13f,
14.39f,
14.65f,
14.98f,
15.69f,
16.07f,
16.43f,
16.84f,
17.02f,
17.19f,
17.93f,
18.29f,
19.01f,
19.37f,
25.65f,
26f,
26.39f,
26.76f,
26.95f,
27.12f,
27.36f,
27.83f,
28.06f,
28.22f,
28.57f,
28.96f,
29.15f,
29.3f,
29.54f,
30.08f,
30.44f,
30.79f,
31.16f,
31.49f,
32.21f,
33.12f,
33.48f,
34.06f,
34.39f,
43.13f,
43.48f,
44.24f,
45.33f,
46.09f,
46.47f,
46.62f,
46.77f,
55.25f,
55.6f,
56.35f,
56.75f,
57.45f,
57.83f,
58.53f,
58.93f,
59.65f,
60.03f,
60.72f,
61.12f,
61.38f,
61.8f,
62.21f,
62.57f,
63.12f,
67.3f,
68.08f,
68.38f,
68.55f,
68.78f,
69.58f,
69.96f,
70.67f,
71.06f,
74.04f,
74.43f,
74.82f,
75.11f,
75.28f,
75.53f,
76.24f,
76.62f,
77f,
77.31f,
77.51f,
77.71f,
82.87f,
83.28f,
83.62f,
84f,
84.34f,
85.09f,
85.48f,
85.83f,
86.19f,
86.55f,
87.28f,
87.46f,
88.38f,
88.77f,
89.42f,
90.55f,
91.58f,
91.92f,
92.65f,
93f,
95.97f,
96.46f,
97.17f,
97.56f,
97.98f,
98.32f,
114.91f,
115.12f,
115.31f,
115.71f,
116.03f,
116.23f,
116.45f,
117.18f,
117.53f,
117.89f,
118.26f,
118.63f,
123.86f,
124.92f,
126.05f,
126.83f,
127.32f,
128.32f,
129.02f,
129.39f,
130.5f,
130.87f,
131.56f,
131.98f,
141.63f,
141.96f,
142.36f,
142.75f,
143.17f,
143.86f,
144.6f,
144.89f,
145.11f,
145.32f,
145.6f,
145.85f,
152.77f,
153.16f,
153.54f,
153.87f,
154.1f,
154.97f,
155.34f,
156.08f,
156.43f,
157.2f,
157.56f,
158.26f,
158.6f,
159.34f,
159.71f,
160.1f,
160.47f,
160.82f,
161.23f,
161.64f,
166.01f,
166.4f,
166.8f,
167.16f,
167.34f,
166.54f,
168.34f,
168.74f,
169.46f,
169.85f,
172.91f,
173.27f,
173.66f,
174f,
174.2f,
174.39f,
175.12f,
175.5f,
176.19f,
176.35f,
176.59f,
176.86f,
181.83f,
182.3f,
182.7f,
183.05f,
183.4f,
184.14f,
184.56f,
184.92f,
185.63f,
186.4f,
187.38f,
187.64f,
188.64f,
189.01f,
189.39f,
189.78f,
190.15f,
190.89f,
191.25f,
191.98f,
192.78f,
195.37f,
195.79f,
196.5f,
196.92f,
197.32f,
197.9f,
199.2f,
199.9f,
200.29f};

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height && Input.mousePosition.x > 320 && Input.mousePosition.x < 510 && !zoom)
        {
            float yPos = Input.mousePosition.y;
            float screenHeight = Screen.height;
            songTime = yPos / screenHeight * audioSource.clip.length;

            cursor.position = new Vector3(cursor.position.x, yPos - screenHeight / 2, cursor.position.z);
            timeTxt.text = songTime.ToString("0.00");

            audioSource.time = songTime;
            audioSource.Play();
        }
        else

        if (Input.GetKeyDown(KeyCode.Space)) if (audioSource.isPlaying) audioSource.Stop(); else audioSource.Play();
        if (Input.GetKeyDown(KeyCode.Return)) newSec(songTime.ToString());

        if (Input.GetKey(KeyCode.DownArrow) && cursor.localPosition.y > -359 && canMove)
        {
            audioSource.time -= 1;
            cursor.position = new Vector3(cursor.position.x, cursor.position.y - 1, cursor.position.z);
            audioSource.Play();
            timeTxt.text = songTime.ToString("0.00");
            StartCoroutine(LockMove());
        }

        if (Input.GetKey(KeyCode.UpArrow) && cursor.localPosition.y < 359 && canMove)
        {
            audioSource.time += 1;
            cursor.position = new Vector3(cursor.position.x, cursor.position.y + 1, cursor.position.z);
            audioSource.Play();
            timeTxt.text = songTime.ToString();
            StartCoroutine(LockMove());
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
                cam.fieldOfView = 16.738f;

                zoom = true;
            }
            else
            {
                cam.transform.localPosition = Vector3.zero;
                cam.fieldOfView = 56.738f;

                zoom = false;
            }
        }

        if (zoom && cursor.localPosition.y < 200 && cursor.localPosition.y > -200)
            cam.transform.localPosition = new Vector3(cam.transform.position.x, cursor.position.y, cam.transform.localPosition.z);
    }

    IEnumerator LockMove()
    {
        canMove = false;
        yield return new WaitForSeconds(.5f);
        canMove = true;
    }

    public void FillSecTxts()
    {
        if (secs.Length > 0)
        {
            secTxts = new List<TextMeshProUGUI>();
            for (int i = 0; i < secs.Length; i++)
            {
                newSec(secs[i].ToString(), yPointsArr[i]);
                Debug.Log(i);
            }
        }
        //secs = new float[0];
        //yPointsArr = new float[0];
    }

    #region Saving functions
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
    }

    void PointsSave()
    {
        float[] yPointsArray = yPoints.ToArray();
        yPointsArr = new float[yPointsArray.Length];
        for (int i = 0; i < yPointsArr.Length; i++)
        {
            yPointsArr[i] = yPointsArray[i];
        }
    }

    public void SaveSong()
    {
        SamplesSave();
        SecsSave();
        PointsSave();

        SaveSystem.SaveLevel(this, lvlNameInput.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region Seconds functions
    public void OnPointClick()
    {
        if (removeMod) RemoveSec(); else playFromSec();
    }

    public void RemoveMod()
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
        float x = wall.transform.position.x;
        if (lastPoint)
        {
            if (lastPoint.transform.position.x > 100) x = lastPoint.transform.position.x + Random.Range(-100, 50);
            else if (lastPoint.transform.position.x < -100) x = lastPoint.transform.position.x + Random.Range(50, 100);
            else x = lastPoint.transform.position.x + Random.Range(-100, 100);
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

        yPoints.Add(newPos.y);

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
}
