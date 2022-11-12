using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
    public Track track;
    public GameObject waveform;

    public GameObject ScrollView;

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
    public TextMeshProUGUI lastSecTxt;
    public TextMeshProUGUI lvlNameTxt;

    [Space(5)]

    TextMeshProUGUI[] secTxts = new TextMeshProUGUI[0];

    [HideInInspector]public AudioSource audioSource;
    [HideInInspector] public AudioClip song;

    [HideInInspector]public float[] samples;
    [HideInInspector] public float[] secs = new float[0];

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    #region Saving functions
    void SamplesSave()
    {
        samples = new float[song.samples * song.channels];
        song.GetData(samples, 0);

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = samples[i] * .5f;
        }
    }

    public void SaveSong()
    {
        secs = new float[secTxts.Length];
        for (int i = 0; i < secTxts.Length; i++)
        {
            secs[i] = float.Parse(secTxts[i].text);
        }

        SamplesSave();

        SaveSystem.SaveLevel(this, name);
    }
    #endregion

    #region Seconds functions
    public void EnterSecond()
    {
        newSec(secInput.text);
    }

    public TextMeshProUGUI newSec(string secToAdd)
    {
        if(float.TryParse(secToAdd, out float f))
        {
            float[] oldSecs = secs;
            secs = new float[oldSecs.Length + 1];

            for (int i = 0; i < oldSecs.Length; i++)
            {
                secs[i] = oldSecs[i];
            }
            
            TextMeshProUGUI sec = Instantiate(lastSecTxt);
            sec.transform.SetParent(ScrollView.transform);
            sec.text = secToAdd;
            sec.name = "SecTxt" + secs.Length;

            Vector2 lastPos = sec.rectTransform.position;
            Debug.Log(lastPos);

            sec.transform.localScale = Vector3.one;
            sec.rectTransform.localPosition = new Vector2(lastPos.x, lastPos.y + 200);

            lastSecTxt = sec;
            sec.gameObject.SetActive(true);

            TextMeshProUGUI[] newTxts = new TextMeshProUGUI[secs.Length];
            for (int i = 0; i < secTxts.Length; i++)
            {
                newTxts[i] = secTxts[i];
            }
            newTxts[newTxts.Length - 1] = sec;
            secTxts = newTxts;

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

    public void DeleteSec()
    {
        string sec = secInput.text;

        for (int i = 0; i < secTxts.Length; i++)
        {
            if (secTxts[i].text == sec)
            {
                Destroy(secTxts[i]);

                for (int j = i; j < secTxts.Length - 1; j++)
                {
                    secTxts[j] = secTxts[j + 1];
                    Debug.Log(j);
                }

                TextMeshProUGUI[] newTxts = new TextMeshProUGUI[secTxts.Length - 1];
                for (int k = 0; k < newTxts.Length; k++)
                {
                    newTxts[k] = secTxts[k];
                }
                secTxts = newTxts;
                lastSecTxt = secTxts[secTxts.Length - 1];

                secInput.text = "";

                return;
            }
        }
    }

    public void ShowOldSec()
    {
        for (int i = 0; i < secs.Length; i++)
        {
            float lastY = lastSecTxt.transform.localPosition.y;

            TextMeshProUGUI sec = Instantiate(lastSecTxt);
            sec.transform.SetParent(ScrollView.transform);
            sec.text = secs[i].ToString();
            sec.name = "SecTxt" + secs.Length;

            Vector2 lastPos = sec.rectTransform.position;
            Debug.Log(lastPos);

            sec.transform.localScale = Vector3.one;
            sec.rectTransform.localPosition = new Vector2(0, lastY -50);

            lastSecTxt = sec;
            sec.gameObject.SetActive(true);

            TextMeshProUGUI[] oldTxts = secTxts;
            secTxts = new TextMeshProUGUI[secTxts.Length + 1];

            for (int j = 0; j < oldTxts.Length; j++)
            {
                secTxts[j] = oldTxts[j];
            }
            secTxts[secTxts.Length - 1] = sec;
        }
        secs = new float[secTxts.Length];
    }

    public void ChangeSec()
    {
        GameObject secObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<TextMeshProUGUI>().gameObject;
        TextMeshProUGUI secTxt = secObj.GetComponentInParent<TextMeshProUGUI>();

        if (secTxt.color == Color.white)
        {
            secTxt.color = Color.blue;
        }
        else if(secTxt.color == Color.blue)
        {
            secTxt.text = secInput.text;
            secTxt.color = Color.white;
            secInput.text = "";
        }
    }
    #endregion

    public void SwitchPanels(GameObject p1, GameObject p2)
    {
        p1.SetActive(false);
        p2.SetActive(true);
    }
}
