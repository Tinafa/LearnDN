using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MapMaker : MonoBehaviour {

    public enum ET
    {
        Perlin,Mean,FixedNoise,Curve,Random,Fun
    }
    public ET type = ET.Perlin;

    private float _seedX, _seedZ;

    [SerializeField]
    private float _width = 129;
    [SerializeField]
    private float _depth = 129;

    [SerializeField]
    private float _maxHeight = 10;

    [SerializeField]
    private bool _isPerlinNoiseMap = true;

    [SerializeField]
    private bool _isMean = true;

    [SerializeField]
    private bool _isCurve = true;

    [SerializeField]
    private bool _isAddNoise = true;

    [SerializeField]
    private float _relief = 15f;

    [SerializeField]
    private float _mapSize = 1f;

    [SerializeField]
    int A = 6, B = 1, C = 5, D = 8;
    [SerializeField]
    int E = 10, F = 4;

    int gAx = 1, gAy = 2;
    int gBx = -1, gBy = 2;
    int gCx = -2, gCy = -1;
    int gDx = 1, gDy = -2;

    public static class Settings
    {
        public static int HeightMapResolution = 33;
        public static int Length = 33;
        public static int Height = 20;
    }

    public class TerrainChunkSettings
    {

        int X = 0;
        int Z = 0;

        Terrain terrain = null;

        public void CreateTerrain(Transform transform)
        {
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = Settings.HeightMapResolution;
            terrainData.baseMapResolution = Settings.HeightMapResolution;
            terrainData.alphamapResolution = Settings.HeightMapResolution;
            terrainData.SetDetailResolution(512, 8);

            float[,] perlinHeights = GetHeights(terrainData);
            terrainData.SetHeights(0, 0, perlinHeights);

            //> Create Cubes
            for (int x = 0; x < Settings.HeightMapResolution; x++)
            {
                for (int z = 0; z < Settings.HeightMapResolution; z++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    cube.transform.localPosition = new Vector3(x, 0, z);
                    cube.transform.SetParent(transform);

                    SetY(cube,perlinHeights[z,x], Settings.Height);
                }
            }

            Debug.Log("=================================================================");
            //Print
            //PrintHeightDatas(terrainData);

            terrainData.size = new Vector3(Settings.Length, Settings.Height, Settings.Length);

            GameObject newTerrainGameObject = Terrain.CreateTerrainGameObject(terrainData);
            newTerrainGameObject.transform.position = new Vector3(X * Settings.Length, 0, Z * Settings.Length);

            terrain = newTerrainGameObject.GetComponent<Terrain>();

            terrain.Flush();

            
        }

        public void PrintHeightDatas(TerrainData terrainData)
        {
            int width = terrainData.heightmapWidth;
            int height = terrainData.heightmapHeight;

            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            for (int z = height - 1; z >= 0; z--)
            {
                sb.Length = 0;
                for (int x = 0; x < width; x++)
                {
                    sb.Append(string.Format("{0}  ", terrainData.GetHeight(x, z).ToString()));
                }
                sb.Append("\r\n");
                Debug.Log(sb.ToString());
            }
        }

        public float[,] GetFixedHeights(TerrainData terrainData)
        {
            int width = terrainData.heightmapWidth;
            int height = terrainData.heightmapHeight;

            var heightmap = new float[width, height];

            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            for (int z = height - 1; z >= 0; z--)
            {
                sb.Length = 0;
                for (int x = 0; x < width; x++)
                {                    
                    heightmap[z, x] = x*1.0f/(width-1);
                    sb.Append(string.Format("{0}  ", heightmap[z, x].ToString()));
                }
                sb.Append("\r\n");
                //Debug.Log(sb.ToString());
            }
            return heightmap;
        }

        public float[,] GetHeights(TerrainData terrainData)
        {
            int width = terrainData.heightmapWidth;
            int height = terrainData.heightmapHeight;

            var heightmap = new float[width, height];

            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            for (int z = height - 1; z >= 0; z--)
            {
                sb.Length = 0;
                for (int x = 0; x < width; x++)
                {
                    float xcoord = X + x * 1.0f / (Settings.HeightMapResolution - 1);
                    float zcoord = Z + z * 1.0f / (Settings.HeightMapResolution - 1);
                    heightmap[z, x] = Mathf.PerlinNoise(xcoord, zcoord);
                    sb.Append(string.Format("{0}  ", heightmap[z, x].ToString()));
                }
                sb.Append("\r\n");
                //Debug.Log(sb.ToString());
            }
            return heightmap;
        }

        public float SampleHeight(int x,int y,int z)
        {
            if (terrain == null) return 0.0f;
            return terrain.SampleHeight(new Vector3(x,y,z));
        }

        public void SetY(GameObject cube,float y,float _maxHeight)
        {
            y *= _maxHeight;
            cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

            Color color = Color.black;
            if (y > _maxHeight * 0.3f)
            {
                ColorUtility.TryParseHtmlString("#019540FF", out color);
            }
            else if (y > _maxHeight * 0.2f)
            {
                ColorUtility.TryParseHtmlString("#2432ADFF", out color);
            }
            else if (y > _maxHeight * 0.1f)
            {
                ColorUtility.TryParseHtmlString("#D4500EFF", out color);
            }
            cube.GetComponent<MeshRenderer>().material.color = color;
        }
    }


    private void Awake()
    {

        transform.localScale = new Vector3(_mapSize, _mapSize, _mapSize);

        /*_seedX = Random.value * 100f;
        _seedZ = Random.value * 100f;

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _depth; z++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);

                SetY(cube);
            }
        }*/

        TerrainChunkSettings terrainS = new TerrainChunkSettings();
        terrainS.CreateTerrain(transform);
        Debug.Log(terrainS.SampleHeight(0, 0, 0));

        //第二块
        /*for (int x = (int)_width; x < 2*_width; x++)
        {
            for (int z = 0; z < _depth; z++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);

                SetY(cube);
            }
        }*/
    }

    private void OnValidate()
    {

        if (!Application.isPlaying)
        {
            return;
        }

        transform.localScale = new Vector3(_mapSize, _mapSize, _mapSize);

        foreach (Transform child in transform)
        {
            SetY(child.gameObject);
        }
    }

    bool change = true;

    private void SetY(GameObject cube)
    {
        float y = 0;
        switch(type)
        {
            case ET.Perlin:
                {
                    float xSample = (cube.transform.localPosition.x + _seedX) / _relief;
                    float zSample = (cube.transform.localPosition.z + _seedZ) / _relief;
                    float noise = Mathf.PerlinNoise(xSample, zSample);
                    y = _maxHeight * noise;
                }
                break;
            case ET.Mean:
                {


                    float u = cube.transform.localPosition.x / (_width - 1);
                    float v = cube.transform.localPosition.z / (_depth - 1);
                    float yy = 0;
                    if (u>=1f)
                    {
                        u -= 1f;

                        float xx1 = (1 - u) * B + u * E;
                        float xx2 = (1 - u) * D + u * F;
                        yy = (1 - v) * xx1 + v * xx2;
                    }
                    else
                    {
                        float xx1 = (1 - u) * A + u * B;
                        float xx2 = (1 - u) * C + u * D;
                        yy = (1 - v) * xx1 + v * xx2;
                    }                    

                    y = yy;

                }
                break;
            case ET.FixedNoise:
                {
                    float u = cube.transform.localPosition.x / (_width - 1);
                    float v = cube.transform.localPosition.z / (_depth - 1);

                    float xSample = (cube.transform.localPosition.x + _seedX) / _relief;
                    float zSample = (cube.transform.localPosition.z + _seedZ) / _relief;
                    float noise = Mathf.PerlinNoise(xSample, zSample);

                    bool flag = false;
                    if (u >= 1f)
                    {
                        u -= 1f;
                        flag = true;
                    }

                    float fu = change ? fade(u):u;
                    float fv = change ? fade(v):v;

                    change = !change;

                    float yy = 0;
                    if (flag)
                    {                        
                        float xx1 = (1 - fu) * B + fu * E;
                        float xx2 = (1 - fu) * D + fu * F;
                        yy = (1 - fv) * xx1 + fv * xx2;
                    }
                    else
                    {
                        float xx1 = (1 - fu) * A + fu * B;
                        float xx2 = (1 - fu) * C + fu * D;
                        yy = (1 - fv) * xx1 + fv * xx2;
                    }
                    

                    float ff = u * u + v * v - u - v;
                    ff *= -2;
                    y = yy * (1-noise * ff);
                    //y = yy * (1 - noise * 4 * u * v * (1 - u * v));
                }
                break;
            case ET.Curve:
                {
                    float u = cube.transform.localPosition.x / (_width - 1);
                    float v = cube.transform.localPosition.z / (_depth - 1);

                    bool flag = false;
                    if (u >= 1f)
                    {
                        u -= 1f;
                        flag = true;
                    }

                    u = fade(u);
                    v = fade(v);
                    float yy = 0;
                    if (flag)
                    {
                        float xx1 = (1 - u) * B + u * E;
                        float xx2 = (1 - u) * D + u * F;
                        yy = (1 - v) * xx1 + v * xx2;
                    }
                    else
                    {
                        float xx1 = (1 - u) * A + u * B;
                        float xx2 = (1 - u) * C + u * D;
                        yy = (1 - v) * xx1 + v * xx2;
                    }

                    y = yy;
                }
                break;
            case ET.Random:
                {
                    y = Random.Range(0, _maxHeight);
                }
                break;
            case ET.Fun:
                {
                    float u = cube.transform.localPosition.x / (_width - 1);
                    float v = cube.transform.localPosition.z / (_depth - 1);

                    //bool flag = false;
                    if (u >= 1f)
                    {
                        u -= 1f;
                        //flag = true;
                    }

                    float ff = u * u + v * v - u - v;
                    y = 10*ff;
                }
                break;
        }       

        cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        Color color = Color.black;
        if (y > _maxHeight * 0.3f)
        {
            ColorUtility.TryParseHtmlString("#019540FF", out color);
        }
        else if (y > _maxHeight * 0.2f)
        {
            ColorUtility.TryParseHtmlString("#2432ADFF", out color);
        }
        else if (y > _maxHeight * 0.1f)
        {
            ColorUtility.TryParseHtmlString("#D4500EFF", out color);
        }
        cube.GetComponent<MeshRenderer>().material.color = color;
    }

    float fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}
