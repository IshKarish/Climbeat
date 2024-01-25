using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [SerializeField] private GameObject climbPoint;
    [SerializeField] private GameObject[] climbPoints;
    
    private string path = "C:/Users/danie/AppData/LocalLow/The Banana Project/Climbeat/PetahTikva.notvirus";
    private AudioSource audioSource;

    private float[] seconds;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        LoadLevel();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    void LoadLevel()
    {
        SavesManager savesManager = SaveSystem.LoadLevel(path);
        audioSource.clip = savesManager.clip;
        audioSource.Play();

        seconds = savesManager.seconds;
        climbPoints = new GameObject[seconds.Length];

        for (int i = 0; i < climbPoints.Length; i++)
        {
            climbPoints[i] = Instantiate(climbPoint);
            Vector3 newPos = new Vector3(savesManager.positions[i].x, savesManager.positions[i].y + 360, 0);
            climbPoints[i].transform.position = newPos;
            climbPoints[i].transform.SetParent(transform);
        }
        //transform.localScale = new Vector3(10, 5000, 1);
        foreach (GameObject point in climbPoints)
        {
            //point.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        }
    }
}
