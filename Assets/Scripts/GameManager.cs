using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public float globTime;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    { 
        globTime += Time.deltaTime;
    }
}
