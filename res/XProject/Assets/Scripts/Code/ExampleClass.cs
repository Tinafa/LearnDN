using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public int pixWidth;
    public int pixHeight;
    public float xOrg;
    public float yOrg;
    public float scale = 1.0F;
    private Texture2D noiseTex;
    private Color[] pix;
    //private Renderer rend;
    private Rect rect;
    void Start()
    {
        //rend = GetComponent<Renderer>();
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        //rend.material.mainTexture = noiseTex;

        rect = new Rect(20, 20, noiseTex.width, noiseTex.height);
    }
    void CalcNoise()
    {
        float y = 0.0F;
        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)(y * noiseTex.width + x)] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }
    void Update()
    {
        CalcNoise();
    }

    void OnGUI()
    {
        GUI.DrawTexture(rect, noiseTex, ScaleMode.ScaleToFit, true, 1.0f);
    }
}