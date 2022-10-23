using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilder : MonoBehaviour
{
    public GameManager gameManager;
    float gTime;

    public float[] secs = new float[0];
    bool startTheThing = false;

    public GameObject cube;
    GameObject[] cubes = new GameObject[0];
    AudioSource aud;

    string lvl;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        lvl = PlayerPrefs.GetString("Path");
        Debug.Log(lvl);

        LevelData data = SaveSystem.LoadLevel(lvl);
        secs = data.Secs;


        for (int i = 0; i < secs.Length; i++)
        {
            secs[i] += gameManager.globTime;
        }

        int secLen = secs.Length;
        float[] newSec = new float[secLen + 1];
        for (int i = 0; i < secLen; i++)
        {
            newSec[i] = secs[i];
        }
        secs = newSec;

        cube.transform.position = Vector3.zero;

        aud = GetComponent<AudioSource>();
        AudioClip clip = AudioClip.Create("Song", data.samplesLeangth, data.channels, data.frequency, false);
        clip.SetData(data.samples, 0);
        aud.clip = clip;

        cubes = new GameObject[secs.Length];
        cubes[0] = cube;

        for (int i = 1; i < cubes.Length; i++)
        {
            GameObject nc = Instantiate(newCube(i-1));

            nc.name = "cube " + i;
            //nc.transform.SetParent(transform);
            nc.GetComponent<Renderer>().material.color = Color.white;
            cubes[i] = nc;
        }

        Destroy(cubes[cubes.Length-1]);

        PlayerPrefs.DeleteAll();
    }

    private void Start()
    {
        Debug.Log(Time.time);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            startTheThing = true;
            aud.Play();
        }

        if (startTheThing)
        {
            for (int i = 0; i < secs.Length - 1; i++)
            {
                if (Time.time >= secs[i])
                {
                    cubes[i].GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else
        {
            for (int i = 0; i < secs.Length - 1; i++)
            {
                secs[i] += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        Camera cam = Camera.main;
        Vector3 camPos = cam.transform.position;

        if (startTheThing)
        {
            cam.transform.position = Vector3.Lerp(camPos, new Vector3(camPos.x, camPos.y + 1, camPos.z), Time.deltaTime);
        }
    }

    GameObject newCube(int num)
    {
        GameObject lastCube = cubes[num];
        GameObject newCube = lastCube;

        Vector3 lastPos = lastCube.transform.position;

        float rand = Random.Range(-1.4f, 1.4f);

        if (lastPos.x <= -3.5f)
        {
            newCube.transform.position = new Vector3(lastPos.x + 1, lastPos.y + 1, lastPos.z);
        }
        else if (lastPos.x >= 3.5f)
        {
            newCube.transform.position = new Vector3(lastPos.x - 1, lastPos.y + 1, lastPos.z);
        }
        else
        {
            newCube.transform.position = new Vector3(lastPos.x + rand, lastPos.y + 1, lastPos.z);
        }

        return newCube;
    }

    private void OnApplicationQuit()
    {
        cube.transform.position = Vector3.zero;
    }
}
