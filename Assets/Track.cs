using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Sprite))]
public class Track : MonoBehaviour
{
    public LevelEditor editor;
    public Transform cursor;
    public Transform editorParent;
    public Image fakeWaveform;

    public int width = 1024;
    public int height = 64;
    public Color background = Color.black;
    public Color foreground = Color.yellow;

    Camera cam = null;
    private AudioSource aud = null;
    private SpriteRenderer sprend = null;
    private int samplesize;
    private float[] samples = null;
    private float[] waveform = null;
    float songTime = 0;
    float lastX;

    int halfWidth;
    int halfHeight;

    Vector2 mousePos;

    private void Start()
    {
        halfWidth = Screen.width / 2;
        halfHeight = Screen.height / 2;

        // reference components on the gameobject
        aud = this.GetComponent<AudioSource>();
        sprend = this.GetComponent<SpriteRenderer>();
        cam = Camera.main;
        //editor = GetComponent<LevelEditor>();

        Texture2D texwav = GetWaveform();
        Rect rect = new Rect(Vector2.zero, new Vector2(width, height));
        sprend.sprite = Sprite.Create(texwav, rect, Vector2.zero);

        //cam.transform.position = new Vector3(0f, 0f, -1f);
        //cam.transform.Translate(Vector3.right * (sprend.size.x / 2f));

        GetComponent<RectTransform>().localPosition = new Vector2(4.0935f, 0);
        GetComponent<RectTransform>().localScale = new Vector3(.2005f, .2005f, .2005f);

        fakeWaveform.sprite = sprend.sprite;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width)
        {
            float xPos = Input.mousePosition.x;
            float screenWidth = Screen.width;
            songTime = xPos / screenWidth * aud.clip.length;

            cursor.position = new Vector3(xPos, cursor.position.y, cursor.position.z);
            Debug.Log("lastX = " + lastX);
            Debug.Log("xPos = " + (xPos - (screenWidth / 2)));

            aud.time = songTime;
            aud.Play();
        } else 

        //if (Input.GetMouseButtonUp(0)) aud.Play();
        if (Input.GetKeyDown(KeyCode.Space)) if (aud.isPlaying) aud.Stop(); else aud.Play();
        if(Input.GetKeyDown(KeyCode.Return)) editor.newSec(songTime.ToString());

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            aud.time -= .5f;
            cursor.position = new Vector3(cursor.position.x - .5f, cursor.position.y, cursor.position.z);
            aud.Play();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            aud.time += .5f;
            cursor.position = new Vector3(cursor.position.x + .5f, cursor.position.y, cursor.position.z);
            aud.Play();
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (editorParent.localScale == new Vector3(1, 1, 1))
            {
                cam.transform.position = Input.mousePosition;
                editorParent.localScale = new Vector3(3, 3, 3);
                
                //editorParent.position = mousePos;
            }
            else
            {
                editorParent.localScale = new Vector3(1, 1, 1);
                
            }
        }

        lastX = Input.mousePosition.x;
    }

    private Texture2D GetWaveform()
    {
        int halfheight = height / 2;
        float heightscale = (float)height * 0.75f;

        // get the sound data
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        waveform = new float[width];

        samplesize = aud.clip.samples * aud.clip.channels;
        samples = new float[samplesize];
        aud.clip.GetData(samples, 0);

        int packsize = (samplesize / width);
        for (int w = 0; w < width; w++)
        {
            waveform[w] = Mathf.Abs(samples[w * packsize]);
        }

        // map the sound data to texture
        // 1 - clear
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, background);
                
            }
        }

        // 2 - plot
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < waveform[x] * heightscale; y++)
            {
                tex.SetPixel(x, halfheight + y, foreground);
                tex.SetPixel(x, halfheight - y, foreground);
            }
        }

        tex.Apply();

        return tex;
    }
}
