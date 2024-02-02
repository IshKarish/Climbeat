using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public float globTime;
    [HideInInspector] public float score;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    { 
        globTime += Time.deltaTime;
    }

    public void WinLevel()
    {
        ScoreTracker scoreTracker = FindObjectOfType<ScoreTracker>();
        scoreTracker.SaveScore(out string levelName, out float score);
        
        LoadScene(2);

        EndMenu endMenu = FindObjectOfType<EndMenu>();
        endMenu.nameText.text = levelName;
        endMenu.scoreText.text = "Score: " + score.ToString("0.0");

        float best = PlayerPrefs.GetFloat(levelName + " Best", 0);
        endMenu.bestText.text = "Best: " + best.ToString("0.0");
    }
    
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
