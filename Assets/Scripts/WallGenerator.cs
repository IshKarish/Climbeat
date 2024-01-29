using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [Header("Climb Point")]
    [SerializeField] private GameObject climbPoint;
    [SerializeField] private GameObject[] climbPoints;
    
    [Header("Colors")]
    [SerializeField] private Color getReadyColor = Color.yellow;
    [SerializeField] private Color climbColor = Color.red;

    [Header("Times")]
    [SerializeField] private float getReadyTime = .3f;
    
    private string path;
    private AudioSource audioSource;

    private float[] seconds;
    private int nextIndex;

    private float time;
    private bool startLevel;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        path = PlayerPrefs.GetString("Path");
        LoadLevel();
        PlayerPrefs.DeleteKey("Path");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) startLevel = true;
        
        if (startLevel && nextIndex < seconds.Length)
        {
            if (Time.time >= seconds[nextIndex] - getReadyTime)
            {
                ColorClimbPoint(climbPoints[nextIndex], getReadyColor);
                Debug.Log("Get Ready...");
            }

            if (Time.time >= seconds[nextIndex])
            {
                ColorClimbPoint(climbPoints[nextIndex], climbColor);
                Debug.Log("Now!");
                nextIndex++;
            }
            
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else time += Time.deltaTime;
    }

    void ColorClimbPoint(GameObject point, Color color)
    {
        point.GetComponent<Renderer>().material.color = color;
    }

    void LoadLevel()
    {
        SavesManager savesManager = SaveSystem.LoadLevel(path);
        audioSource.clip = savesManager.clip;

        seconds = savesManager.seconds;
        climbPoints = new GameObject[seconds.Length];

        for (int i = 0; i < climbPoints.Length; i++)
        {
            climbPoints[i] = CreateClimbPoint(savesManager, i);
        }
        
        transform.localScale = new Vector3(10, 5000, 1);
        
        foreach (GameObject point in climbPoints)
        {
            FixPointScale(point);
        }
    }

    GameObject CreateClimbPoint(SavesManager savesManager, int i)
    {
        GameObject point = Instantiate(climbPoint, transform, true);
        Vector3 newPos = new Vector3(savesManager.positions[i].x, savesManager.positions[i].y + 360, 0);
        point.transform.position = newPos;

        return point;
    }

    void FixPointScale(GameObject point)
    {
        point.transform.SetParent(null);
            
        Vector3 newScale = new Vector3(0.3f, 0.3f, 0.3f);
        point.transform.localScale = newScale;
            
        point.transform.SetParent(transform);
    }
}
