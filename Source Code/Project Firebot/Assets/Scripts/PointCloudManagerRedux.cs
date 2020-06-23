using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(PointCloudBuilder))]
public class PointCloudManagerRedux : MonoBehaviour
{
    /*File*/    [Header("File")]
    public string filePath = "/PointCloud/xyzrgb_manuscript"; // "\PointCloud\xyzrgb_manuscript" <- test pointcloud
    private string filename;

    //GUI
    private float progress = 0;
    private string guiText = "";
    private bool loaded = false;

    /*PointCloud*/  [Header("PointCloud")]
    private GameObject pointCloud;

    public float scale = 1;
    public bool invertYZ = false;
    public bool forceReload = false;

    private Vector3[] points;
    private Color[] colors;
    private Vector3 minValue;

    private Color defaultColor = Color.green;
    private const float colDiv = 0.00392156862745098f; // 1/255

    private PointCloudBuilder builder;

    private void Start()
    {
        //Debug.Log(20*1.0f / (501 - 1));//Application.streamingAssetsPath);
        CreateFolders();

        filename = Path.GetFileName(filePath);

        builder = GetComponent<PointCloudBuilder>();

        LoadScene();
    }

    private void LoadScene()
    {
        if(!Directory.Exists(Application.dataPath + "/Resources/PointCloudMeshes" + filename))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/PointCloudMeshes", filename);
            LoadPointCloud();
        }
        else if (forceReload)
        {
            UnityEditor.FileUtil.DeleteFileOrDirectory(Application.dataPath + "/Resources/PointCloudMeshes" + filename);
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/PointCloudMeshes", filename);
            LoadPointCloud();
        }
        else
        {
            LoadStoredMeshes();
        }
    }

    private void LoadPointCloud()
    {
        if (File.Exists(Application.dataPath + filePath + ".off"))
            StartCoroutine("LoadOFF", filePath + ".off");
       /*else if (File.Exists(Application.dataPath + filePath + ".pts"))
            // load pts
            StartCoroutine("loadPTS", filePath + ".pts");
        else if (File.Exists(Application.dataPath + filePath + ".xyz"))
            // load xyz
            StartCoroutine("loadXYZ", filePath + ".xyz");*/
        else
            Debug.LogWarning("File '" +filePath+ "' could not be found");
    }

    private void LoadStoredMeshes()
    {
        Debug.Log("Using previously loaded PointCloud: " + filename);

        Instantiate(Resources.Load("PointCloudMeshes/" + filename));

        loaded = true;
    }

    private IEnumerator LoadOFF(string pFilePath)
    {
        Debug.Log("loading OFF");
        StreamReader reader = new StreamReader(Application.dataPath + pFilePath);
        reader.ReadLine(); // Object File Format
        string[] buffer = reader.ReadLine().Split(); //  number of points, number of faces, number of edges

        int totalPoints = int.Parse(buffer[0]);
        points = new Vector3[totalPoints];
        colors = new Color[totalPoints];

        for (int i = 0; i<totalPoints; i++)
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
            if(i%(totalPoints*0.05f) == 0)
            {
                guiText = i.ToString() +" out of " + totalPoints.ToString() + " loaded";
                yield return null;
            }
        }

        builder.LoadPointGroup(points, colors, filename);

        //Store PointCloud
        //UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/PointCloudMeshes/" + filename + ".prefab", pointCloud);

        loaded = true;
    }

    private IEnumerator loadPTS(string dPath)
    {

        Debug.Log("loading PTS");
        // Read file
        StreamReader sr = new StreamReader(Application.dataPath + dPath);
        string[] buffer = sr.ReadLine().Split(); // nPoints

        int totalPoints = int.Parse(buffer[0]);
        points = new Vector3[totalPoints];
        colors = new Color[totalPoints];
        minValue = new Vector3();

        for (int i = 0; i < totalPoints; i++)
        {
            buffer = sr.ReadLine().Split();

            if (!invertYZ)
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[1]) * scale, float.Parse(buffer[2]) * scale);
            else
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[2]) * scale, float.Parse(buffer[1]) * scale);

            if (buffer.Length >= 6)
                colors[i] = new Color(int.Parse(buffer[4]) * colDiv, int.Parse(buffer[5]) * colDiv, int.Parse(buffer[6]) * colDiv);
            else
                colors[i] = defaultColor;

            // Relocate Points near the origin
            //calculateMin(points[i]);

            // GUI
            progress = i * 1.0f / (totalPoints- 1) * 1.0f;
            if (i % Mathf.FloorToInt(totalPoints * 0.05f) == 0)
            {
                guiText = i.ToString() + " out of " + totalPoints.ToString() + " loaded";
                yield return null;
            }
        }


        // Instantiate Point Groups
        builder.LoadPointGroup(points, colors, filename);

        //Store PointCloud
        //UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/PointCloudMeshes/" + filename + ".prefab", pointCloud);

        loaded = true;
    }

    private IEnumerator loadXYZ(string dPath)
    {

        // Read file
        StreamReader sr = new StreamReader(Application.dataPath + dPath);
        string[] buffer = new string[8];

        int totalPoints= 56000000;
        points = new Vector3[totalPoints];
        colors = new Color[totalPoints];
        minValue = new Vector3();

        for (int i = 0; i < totalPoints; i++)
        {
            buffer = sr.ReadLine().Split();

            if (!invertYZ)
                points[i] = new Vector3(float.Parse(buffer[2]) * scale, float.Parse(buffer[3]) * scale, float.Parse(buffer[4]) * scale);
            else
                points[i] = new Vector3(float.Parse(buffer[2]) * scale, float.Parse(buffer[4]) * scale, float.Parse(buffer[3]) * scale);

            if (buffer.Length >= 5)
                colors[i] = new Color(int.Parse(buffer[5]) / 255.0f, int.Parse(buffer[6]) / 255.0f, int.Parse(buffer[7]) / 255.0f);
            else
                colors[i] = Color.cyan;

            // Relocate Points near the origin
            //calculateMin(points[i]);

            // GUI
            progress = i * 1.0f / (totalPoints- 1) * 1.0f;
            if (i % Mathf.FloorToInt(totalPoints/ 20) == 0)
            {
                guiText = i.ToString() + " out of " + totalPoints.ToString() + " loaded";
                yield return null;
            }
        }

        builder.LoadPointGroup(points, colors, filename);

        //Store PointCloud
        //UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/PointCloudMeshes/" + filename + ".prefab", pointCloud);

        loaded = true;
    }
    
    private void CalculateMin(Vector3 pPoint)
    {
        if (minValue.magnitude == 0)
            minValue = pPoint;

        if (pPoint.x < minValue.x)
            minValue.x = pPoint.x;
        if (pPoint.y < minValue.y)
            minValue.y = pPoint.y;
        if (pPoint.z < minValue.z)
            minValue.z = pPoint.z;
    }

    private void CreateFolders()
    {
        if (!Directory.Exists(Application.dataPath + "/Resources/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

        if (!Directory.Exists(Application.dataPath + "/Resources/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", "PointCloudMeshes");
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
