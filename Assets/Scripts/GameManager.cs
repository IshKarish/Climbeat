using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public float globTime;
    [HideInInspector] public float points;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    { 
        globTime += Time.deltaTime;
    }

    public void AddPoint(float points)
    {
        this.points += points;
    }

    public void LoseLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void EndLevel(string levelName)
    {
        PlayerPrefs.SetFloat(levelName + " Score", points);
    }

    public void LoadBestScore(string levelName)
    {
        PlayerPrefs.GetFloat(levelName + " Score", 0);
    }
}
