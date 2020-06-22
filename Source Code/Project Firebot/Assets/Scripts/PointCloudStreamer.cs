using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PointCloudStreamer : MonoBehaviour
{
    public string filePath; // "\PointCloud\xyzrgb_manuscript" <- test pointcloud
    private string filename;

    [SerializeField] private Material vertexMaterial;

    //GUI
    private float progress = 0;
    private string guiText = "";
    private bool loaded = false;

    /*PointCloud*/
    [Header("PointCloud")]
    private GameObject pointCloud;

    public float scale = 1;
    public bool invertYZ = false;

    [SerializeField] private Color defaultColor = Color.green;
    private const float colDiv = 0.00392156862745098f; // 1/255

    private void Start()
    {
        ///StartCoroutine("LoadOFF", filePath + ".off");
        LoadOFF(filePath + ".off");
        Debug.Log("All done!");
    }

    private void LoadOFF(string pFilePath)
    {
        StreamReader reader = new StreamReader(Application.dataPath + pFilePath);
        reader.ReadLine(); // Object File Format
        string[] buffer = reader.ReadLine().Split(); //  number of points, number of faces, number of edges

        int totalPoints = int.Parse(buffer[0]);
        Vector3[]points = new Vector3[totalPoints];
        Color[] colors = new Color[totalPoints];

        for (int i = 0; i < totalPoints; i++)
        {
            buffer = reader.ReadLine().Split();

            if (!invertYZ)
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[1]) * scale, float.Parse(buffer[2]) * scale);
            else
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[2]) * scale, float.Parse(buffer[1]) * scale);

            if (buffer.Length > 5)
                colors[i] = new Color(float.Parse(buffer[3]) * colDiv, float.Parse(buffer[4]) * colDiv, float.Parse(buffer[5]) * colDiv);
            else
                colors[i] = defaultColor;

            //GUI
            progress = (float)i / (totalPoints - 1);
            if (i % Mathf.FloorToInt(totalPoints / 20) == 0)
            {
                guiText = i.ToString() + " out of " + totalPoints.ToString() + " loaded";
                //yield return null;
            }
        }

        LoadPointGroup(totalPoints,points,colors);
    }
    
    public void LoadPointGroup(int pPointCount, Vector3[] pPoints, Color pColor)
    {
        StartCoroutine(LoadDirectly(pPointCount, pPoints, pColor));
    }

    public void LoadPointGroup(int pPointCount, Vector3[] pPoints, Vector3[] pColors)
    {
        StartCoroutine(LoadDirectly(pPointCount, pPoints, pColors));
    }

    public void LoadPointGroup(int pPointCount, Vector3[] pPoints, Vector4[] pColors)
    {
        StartCoroutine(LoadDirectly(pPointCount, pPoints, pColors));
    }

    public void LoadPointGroup(int pPointCount, Vector3[] pPoints, Color[] pColors)
    {
        int[] indicies = new int[pPointCount];

        for (int i = 0; i < pPointCount; i++)
        {
            indicies[i] = i;

            if (i % Mathf.FloorToInt(pPointCount / 30) == 0)
            {
                guiText = i.ToString() + " indicies out of " + pPointCount.ToString() + " sorted";
                //yield return null;
            }
        }

        InstantiateMesh(pPointCount, pPoints, pColors, indicies);

        //StartCoroutine(LoadDirectly(pPointCount, pPoints, pColors));
    }

    public void LoadPointGroup(int pPointCount, Vector3[] pPoints, Color[] pColors, int[] pIndicies)
    {
        StartCoroutine(LoadDirectly(pPointCount, pPoints, pColors, pIndicies));
    }

    private IEnumerator LoadDirectly(int pPointCount, Vector3[] pPoints, Color pColor)
    {
        int[] indicies = new int[pPointCount];
        Color[] colors = new Color[pPointCount];

        for (int i = 0; i < pPointCount; i++)
        {
            indicies[i] = i;
            colors[i] = pColor;
            yield return null;
        }

        InstantiateMesh(pPointCount, pPoints, colors, indicies);
    }

    private IEnumerator LoadDirectly(int pPointCount, Vector3[] pPoints, Vector3[] pColors)
    {
        int[] indicies = new int[pPointCount];
        Color[] colors = new Color[pPointCount];

        for (int i = 0; i < pPointCount; i++)
        {
            indicies[i] = i;
            colors[i] = new Color(pColors[i].x * colDiv, pColors[i].y * colDiv, pColors[i].z * colDiv);
            yield return null;
        }

        InstantiateMesh(pPointCount, pPoints, colors, indicies);
    }

    private IEnumerator LoadDirectly(int pPointCount, Vector3[] pPoints, Vector4[] pColors)
    {
        int[] indicies = new int[pPointCount];
        Color[] colors = new Color[pPointCount];

        for (int i = 0; i < pPointCount; i++)
        {
            indicies[i] = i;
            colors[i] = new Color(pColors[i].x * colDiv, pColors[i].y * colDiv, pColors[i].z * colDiv, pColors[i].w * colDiv);
            yield return null;
        }
        
        InstantiateMesh(pPointCount, pPoints, colors, indicies);
}

    private IEnumerator LoadDirectly(int pPointCount, Vector3[] pPoints, Color[] pColors)
    {
        int[] indicies = new int[pPointCount];

        for (int i = 0; i < pPointCount; i++)
        {
            indicies[i] = i;

            if (i % Mathf.FloorToInt(pPointCount / 30) == 0)
            {
                guiText = i.ToString() + " indicies out of " + pPointCount.ToString() + " sorted";
                //yield return null;
            }
        }

        Debug.Log(pPoints.Length / 3);

        InstantiateMesh(pPointCount, pPoints, pColors, indicies);
        return null;
    }

    private IEnumerator LoadDirectly(int pPointCount, Vector3[] pPoints, Color[] pColors, int[] pIndicies)
    {
        InstantiateMesh(pPointCount, pPoints, pColors, pIndicies);
        yield return null;
    }

    private void InstantiateMesh(int pPointCount, Vector3[] pPoints, Color[] pColors, int[] pIndicies, int pMeshIndex = 0)
    {
        Debug.Log("instatiating...");
        GameObject pointGroup = new GameObject("PointGroup"+pMeshIndex+" ("+pPointCount+")");
        pointGroup.AddComponent<MeshFilter>();
        pointGroup.AddComponent<MeshRenderer>();
        pointGroup.GetComponent<MeshRenderer>().material = vertexMaterial;

        pointGroup.GetComponent<MeshFilter>().mesh = CreateMesh(pPointCount, pPoints, pColors, pIndicies);
        //pointGroup.transform.parent = pointCloud.transform;
    }


    private Mesh CreateMesh(int pPointCount, Vector3[] pPoints, Color[] pColors, int[] pIndicies)
    {
        Debug.Log("creating mesh...");
        Mesh mesh = new Mesh();

        mesh.vertices = pPoints;
        Debug.Log(mesh.vertices.Length);
        mesh.colors = pColors;
        mesh.SetIndices(pIndicies, MeshTopology.Points, 0);
        mesh.uv = new Vector2[pPointCount];
        mesh.normals = new Vector3[pPointCount];

        Debug.Log("mesh done...");

        return mesh;
    }


    private void OnGUI()
    {
        if (!loaded)
        {
            GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f, 400.0f, 20));
            GUI.Box(new Rect(0, 0, 200.0f, 20.0f), guiText);
            GUI.Box(new Rect(0, 0, progress * 200.0f, 20), "");
            GUI.EndGroup();
        }
    }
}
