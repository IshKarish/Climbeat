using System.Collections;
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

    [Header("Floats")]
    [SerializeField] private float getReadyTime = .3f;
    [SerializeField] private float heightToAdd = 357;

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
                StartCoroutine(ActiveClimbPoint(climbPoints[nextIndex]));
                nextIndex++;
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

        bool songHasEnded = Time.time > seconds[^1] + timePassed;
        if (songHasEnded) gameManager.WinLevel();;
        if (player.position.y < -0.5f) gameManager.LoadScene(0);
    }

    IEnumerator ActiveClimbPoint(GameObject climbPoint)
    {
        ColorClimbPoint(climbPoint, getReadyColor);
        
        yield return new WaitForSeconds(getReadyTime);
        
        ColorClimbPoint(climbPoint, climbColor);
        
        yield return new WaitForSeconds(getReadyTime);
        
        ColorClimbPoint(climbPoint, originalColor);
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
    }

    GameObject CreateClimbPoint(SavesManager savesManager, int i)
    {
        GameObject point = Instantiate(climbPoint, transform, true);
        Vector3 newPos = new Vector3(savesManager.positions[i].x, savesManager.positions[i].y + heightToAdd, 0);
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
