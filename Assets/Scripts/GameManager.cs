using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float globTime = 0;
    public GameObject[] letters = new GameObject[26];
    public Material txtMat;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        globTime += Time.deltaTime;
    }

    public void write(string txt, Vector3 pivot)
    {
        GameObject txt3d = new GameObject();
        txt3d.name = txt + "Txt";

        string abcStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] abc = abcStr.ToCharArray();

        char[] chars = txt.ToUpper().ToCharArray();

        float half = 0;
        if (txt.Length % 2 == 0) half = (chars.Length / 2) - 2; else half = (chars.Length / 2);

        for (int i = 0; i < chars.Length; i++)
        {
            for (int j = 0; j < abc.Length; j++)
            {
                if(chars[i] == abc[j])
                {
                    GameObject charObj = Instantiate(letters[j]);
                    charObj.transform.position = new Vector3(half, 0, 0);
                    charObj.transform.parent = txt3d.transform;
                    charObj.GetComponent<Renderer>().material = txtMat;
                    half--;
                    break;
                }
            }
        }

        txt3d.transform.position = pivot;
        txt3d.transform.rotation = Quaternion.Euler(0, -180, 0);
        txt3d.transform.localScale = new Vector3(.6f, .6f, .6f);
    }

    public void LoadScene(int ind)
    {
        SceneManager.LoadScene(ind);
    }
}
