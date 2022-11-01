using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float globTime = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        globTime += Time.deltaTime;
    }

    public void LoadScene(int ind)
    {
        SceneManager.LoadScene(ind);
    }
}
