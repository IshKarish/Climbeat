using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexabodyVR;
using HurricaneVR;

public class WallBuilder : MonoBehaviour
{
    public AudioSource impactSFX;
    public GameObject wall;

    public GameManager gameManager;
    float gTime;
    int nxtInd = 0;

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
        AudioClip clip = AudioClip.Create("Song", data.samplesLeangth / 2, data.channels, data.frequency, false);
        clip.SetData(data.samples, 0);
        aud.clip = clip;

        cubes = new GameObject[secs.Length];
        cubes[0] = cube;

        for (int i = 1; i < cubes.Length; i++)
        {
            GameObject nc = Instantiate(newCube(i-1, data.xPos[i-1]));

            nc.name = "cube " + i;
            //nc.transform.SetParent(wall.transform);
            nc.GetComponent<Renderer>().material.color = Color.white;

            //cubes[i].GetComponent<HurricaneVR.Framework.Core.HVRGrabbable>().enabled = false;
            cubes[i] = nc;
        }

        cubes[0].GetComponent<HurricaneVR.Framework.Core.HVRGrabbable>().enabled = true;
        Destroy(cubes[cubes.Length-1]);

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].transform.SetParent(transform);
        }

        wall.transform.localScale = new Vector3(10, 5000, 1);

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].transform.localScale = new Vector3(0.03f, cubes[i].transform.localScale.y, cubes[i].transform.localScale.z);
        }

        PlayerPrefs.DeleteKey("Path");
    }

    private void Start()
    {
        Debug.Log(Time.time);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) start();

        if (startTheThing)
        {
            if (Time.time >= secs[nxtInd] - .3f)
            {
                cubes[nxtInd].GetComponent<HurricaneVR.Framework.Core.HVRGrabbable>().enabled = true;
                cubes[nxtInd].GetComponent<BoxCollider>().size = Vector3.one;
                cubes[nxtInd].GetComponent<Renderer>().material.color = Color.yellow;
            }

            if (Time.time >= secs[nxtInd])
            {
                cubes[nxtInd].GetComponent<Renderer>().material.color = Color.red;
                if(nxtInd < secs.Length-1) nxtInd++;
                Debug.Log("now");
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

    public void start()
    {
        if(!startTheThing)
        {
            startTheThing = true;
            aud.Play();
        }
    }

    GameObject newCube(int num, float xPos)
    {
        GameObject lastCube = cubes[num];
        GameObject newCube = lastCube;

        Vector3 lastPos = lastCube.transform.position;

        //newCube.transform.localScale = new Vector3(0.0025f, 0.0001f, .1f);
        newCube.transform.localPosition = new Vector3(xPos, lastPos.y + .7f, 0);

        newCube.GetComponent<HurricaneVR.Framework.Core.HVRGrabbable>().enabled = true;

        return newCube;
    }

    private void OnApplicationQuit()
    {
        cube.transform.position = Vector3.zero;
    }
}
