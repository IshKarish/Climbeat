using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    private string levelName;
    
    private float score;
    private float bestScore;

    private void Start()
    {
        bestScore = PlayerPrefs.GetFloat(levelName + " Best", 0);
    }

    public void AddPoints(float points)
    {
        score += points;
        if (score > bestScore) bestScore = score;
    }

    public void SaveScore(out string levelName, out float score)
    {
        levelName = this.levelName;
        score = this.score;
        PlayerPrefs.SetFloat(levelName + " Best", bestScore);
    }
}
