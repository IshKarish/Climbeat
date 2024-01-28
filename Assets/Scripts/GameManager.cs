using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    public void LoadScene(int ind)
    {
        count = false;
        SceneManager.LoadScene(ind);
    }
}
