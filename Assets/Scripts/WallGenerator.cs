using UnityEngine;
using UnityEngine.InputSystem;

public class WallGenerator : MonoBehaviour
{
    [SerializeField] private GameObject floor;
    [SerializeField] private Transform player;
    
    [Header("Climb Point")]
    [SerializeField] private GameObject climbPoint;
    [SerializeField] private float climbPointSize = 0.5f;
    private GameObject[] climbPoints;
    
    [Header("Colors")]
    [SerializeField] private Color getReadyColor = Color.yellow;
    [SerializeField] private Color climbColor = Color.red;
    private Color originalColor = Color.black;

    [Header("Times")]
    [SerializeField] private float getReadyTime = .3f;

    [Header("Inputs")]
    [SerializeField] private InputActionProperty startInput;
    
    private string path;
    private AudioSource audioSource;

    private float[] seconds;
    private int nextIndex;

    private float timePassed;
    private bool startLevel;

    private GameManager gameManager;

    private string levelName;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
        
        path = PlayerPrefs.GetString("Path");
        LoadLevel();
        PlayerPrefs.DeleteKey("Path");

        for (int i = 0; i < seconds.Length; i++)
        {
            seconds[i] += gameManager.globTime;
        }
    }

    void Update()
    {
        if (!startLevel && startInput.action.WasPerformedThisFrame()) startLevel = true;
        
        if (startLevel && nextIndex < seconds.Length)
        {
            if (Time.time >= (seconds[nextIndex] - getReadyTime) + timePassed)
            {
                ColorClimbPoint(climbPoints[nextIndex], getReadyColor);
                Debug.Log("Get Ready...");
            }

            if (Time.time >= seconds[nextIndex] + timePassed)
            {
                ColorClimbPoint(climbPoints[nextIndex], climbColor);
                Debug.Log("Now!");
                nextIndex++;
                if (floor.activeSelf) floor.SetActive(false);
            }
            
            if (Time.time >= (seconds[nextIndex] + getReadyTime) + timePassed)
            {
                //ColorClimbPoint(climbPoints[nextIndex - 1], originalColor);
                Debug.Log("Get Ready...");
            }
            
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            for (int i = 0; i < seconds.Length; i++)
            {
                seconds[i] += Time.deltaTime;
            }
        }

        bool isLevelEnded = Time.time > seconds[^1] + timePassed;
        if (isLevelEnded) gameManager.EndLevel(levelName);
        if (player.position.y < 0.5f) gameManager.LoseLevel();
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

        levelName = savesManager.levelName;
        gameManager.LoadBestScore(levelName);

        GetComponent<ScoreVisuallizer>().levelName = levelName;
    }

    GameObject CreateClimbPoint(SavesManager savesManager, int i)
    {
        GameObject point = Instantiate(climbPoint, transform, true);
        Vector3 newPos = new Vector3(savesManager.positions[i].x, savesManager.positions[i].y + 360, 0);
        point.transform.position = newPos;
        point.GetComponent<MeshRenderer>().material.color = originalColor;

        return point;
    }

    void FixPointScale(GameObject point)
    {
        point.transform.SetParent(null);
            
        Vector3 newScale = new Vector3(climbPointSize, climbPointSize, climbPointSize);
        point.transform.localScale = newScale;
            
        point.transform.SetParent(transform);
    }
}
