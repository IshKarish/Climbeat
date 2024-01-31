using TMPro;
using UnityEngine;

public class ScoreVisuallizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestTxt;
    [SerializeField] private TextMeshProUGUI scoreTxt;

    [HideInInspector] public string levelName;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        float best = PlayerPrefs.GetFloat(levelName + " Score", 0);
        bestTxt.text = "Best: " + best;
    }

    private void Update()
    {
        scoreTxt.text = gameManager.points.ToString();
    }
}
