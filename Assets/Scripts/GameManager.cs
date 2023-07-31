using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool vrMode = true;
    public GameObject vrController;
    public GameObject pcCam;

    public HVRHexaBodyInputs inputs;
    public float globTime;

    public GameObject[] letters = new GameObject[26];
    public Material txtMat;

    bool count;

    private void Awake()
    {
        if(!Directory.Exists(Application.persistentDataPath + "/CustomLevels"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/CustomLevels");
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        count = true;

        if(vrMode)
        {
            vrController.SetActive(true);
            pcCam.SetActive(false);
        }
        else
        {
            vrController.SetActive(false);
            pcCam.SetActive(true);
        }
    }

    private void Update()
    {
        if (count) globTime += Time.deltaTime;

        if (Input.GetKey(KeyCode.M)) LoadScene(2);
        if (Input.GetKey(KeyCode.Escape)) LoadScene(0);
        if (Input.GetKey(KeyCode.R) && SceneManager.GetActiveScene().buildIndex == 1) LoadScene(1);

        if(vrMode)
        {
            if (inputs == null) inputs = GameObject.FindGameObjectWithTag("HVR").GetComponentInChildren<HVRHexaBodyInputs>();
            if (inputs.RightController.PrimaryButton) LoadScene(0);
            if (inputs.LeftController.PrimaryButton && SceneManager.GetActiveScene().buildIndex == 1) LoadScene(1);
        }
    }

    public GameObject write(string txt, Vector3 pivot, Vector3 size)
    {
        GameObject txt3d = new GameObject();
        txt3d.name = txt + "Txt";

        string abcStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] abc = abcStr.ToCharArray();

        char[] chars = txt.ToUpper().ToCharArray();

        float half = 0;
        if (txt.Length % 2 == 0) half = (chars.Length / 2) - 2; else half = (chars.Length / 2) - 1;

        for (int i = 0; i < chars.Length; i++)
        {
            for (int j = 0; j < abc.Length; j++)
            {
                if(chars[i] == abc[j])
                {
                    GameObject charObj = Instantiate(letters[j]);
                    charObj.transform.position = new Vector3(half, 0, 0);
                    charObj.transform.parent = txt3d.transform;
                    charObj.GetComponent<Renderer>().material = txtMat;
                    half--;
                    break;
                }
            }
        }

        txt3d.transform.position = pivot;
        txt3d.transform.rotation = Quaternion.Euler(0, -180, 0);
        txt3d.transform.localScale = size;
        
        return txt3d;
    }

    public void LoadScene(int ind)
    {
        count = false;
        SceneManager.LoadScene(ind);
    }
}
