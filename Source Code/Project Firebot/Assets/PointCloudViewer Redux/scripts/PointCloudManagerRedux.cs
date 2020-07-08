using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(PointCloudBuilder))]
public class PointCloudManagerRedux : MonoBehaviour
{
    /*File*/
    [SerializeField] private TextAsset file = null;     // "\PointCloud\xyzrgb_manuscript" <- test pointcloud
    private enum supportedFileTypes { OFF, PTS, XYZ }   // more types have to added manually
    [SerializeField] private supportedFileTypes fileType = supportedFileTypes.OFF;

    //GUI
    private float progress = 0;
    private string guiText = "";
    private bool loaded = false;

    /*PointCloud*/
    private GameObject pointCloud;
    public float scale = 1;
    public bool invertYZ = false;
    //public bool forceReload = false;  //for use with loadStoredMeshes()
    
    private Color defaultColor;
    private const float colDiv = 0.00392156862745098f; // 1/255 but more efficient

    private PointCloudBuilder builder;

    private void Start()
    {
        //CreateFolders(); // not used in this version

        builder = GetComponent<PointCloudBuilder>();
        defaultColor = builder.DefaultColor;

        LoadScene();
    }

    private void LoadScene()
    {
        if (file == null) { Debug.LogWarning("pointCloud file is null"); return; }

        LoadPointCloud();

        //LoadStoredMeshes(); //disabled for now, doesn't work properly atm
    }

    private void LoadPointCloud()
    {
        switch (fileType)
        {
            case supportedFileTypes.OFF:
                StartCoroutine(LoadOFF(file));
                break;
            case supportedFileTypes.PTS:
                StartCoroutine(LoadPTS(file));
                break;
            case supportedFileTypes.XYZ:
                StartCoroutine(LoadXYZ(file));
                break;
            default:
                Debug.LogWarning("Unsupported file format selected, somehow");
                break;
        }
    }

    private void LoadStoredMeshes()
    {
        Debug.Log("Using previously loaded PointCloud: " + file.name);

        Instantiate(Resources.Load("PointCloudMeshes/" + file.name));

        loaded = true;
    }

    private IEnumerator LoadOFF(TextAsset pFile)
    {
        Debug.Log("loading OFF");

        string[] lines = pFile.text.Split('\n');
        string[] buffer = lines[1].Split(); //number of points is stored at the secondline, first line is Object File Format identifier
        
        int totalPoints = int.Parse(buffer[0]);
        Vector3[] points = new Vector3[totalPoints];
        Color[] colors = new Color[totalPoints];

        for (int i = 0; i < totalPoints; i++)
        {
            buffer = lines[i + 2].Split();

            if (!invertYZ)
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[1]) * scale, float.Parse(buffer[2]) * scale);
            else
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[2]) * scale, float.Parse(buffer[1]) * scale);

            if (buffer.Length > 5) //OFF uses RGBA
                colors[i] = new Color(float.Parse(buffer[3]) * colDiv, float.Parse(buffer[4]) * colDiv, float.Parse(buffer[5]) * colDiv);
            else
                colors[i] = defaultColor;

            //GUI
            progress = (float)i / (totalPoints - 1);
            if (i % (totalPoints * 0.05f) == 0)
            {
                guiText = i.ToString() + " out of " + totalPoints.ToString() + " loaded";
                yield return null;
            }
        }

        pointCloud = new GameObject(pFile.name);

        builder.LoadPoints(points, colors, pointCloud);
        
        #if UNITY_EDITOR
        //Store PointCloud
        //UnityEditor.PrefabUtility.SaveAsPrefabAsset(pointCloud, "Assets/Resources/PointCloudMeshes/" + pFile.name + ".prefab");
        #endif

        loaded = true;
    }

    private IEnumerator LoadPTS(TextAsset pFile)
    {
        Debug.Log("loading PTS");

        string[] lines = pFile.text.Split('\n');
        string[] buffer = lines[0].Split(); //number of points stored at first line

        int totalPoints = int.Parse(buffer[0]);
        Vector3[] points = new Vector3[totalPoints];
        Color[] colors = new Color[totalPoints];

        for (int i = 0; i < totalPoints; i++)
        {
            buffer = lines[i + 1].Split();

            if (!invertYZ)
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[1]) * scale, float.Parse(buffer[2]) * scale);
            else
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[2]) * scale, float.Parse(buffer[1]) * scale);

            if (buffer.Length >= 6) //PTS uses ARGB
                colors[i] = new Color(int.Parse(buffer[4]) * colDiv, int.Parse(buffer[5]) * colDiv, int.Parse(buffer[6]) * colDiv);
            else
                colors[i] = defaultColor;

            // GUI
            progress = (float)i / (totalPoints - 1);
            if (i % (totalPoints * 0.05f) == 0)
            {
                guiText = i.ToString() + " out of " + totalPoints.ToString() + " loaded";
                yield return null;
            }
        }

        pointCloud = new GameObject(pFile.name);

        builder.LoadPoints(points, colors, pointCloud);

#if UNITY_EDITOR
        //Store PointCloud
        //UnityEditor.PrefabUtility.SaveAsPrefabAsset(pointCloud, "Assets/Resources/PointCloudMeshes/" + pFile.name + ".prefab");
#endif

        loaded = true;
    }

    private IEnumerator LoadXYZ(TextAsset pFile)
    {
        Debug.Log("Loading XYZ");

        string[] lines = pFile.text.Split('\n');
        string[] buffer;

        int totalPoints = lines.Length;
        Vector3[] points = new Vector3[totalPoints];
        Color[] colors = new Color[totalPoints];

        for (int i = 0; i < totalPoints; i++)
        {
            buffer = lines[i].Split();

            if (!invertYZ)
                points[i] = new Vector3(float.Parse(buffer[2]) * scale, float.Parse(buffer[3]) * scale, float.Parse(buffer[4]) * scale);
            else
                points[i] = new Vector3(float.Parse(buffer[2]) * scale, float.Parse(buffer[4]) * scale, float.Parse(buffer[3]) * scale);

            if (buffer.Length >= 5)
                colors[i] = new Color(int.Parse(buffer[5]) * colDiv, int.Parse(buffer[6]) * colDiv, int.Parse(buffer[7]) * colDiv);
            else
                colors[i] = Color.cyan;

            // GUI
            progress = (float)i / (totalPoints - 1);
            if (i % (totalPoints * 0.05f) == 0)
            {
                guiText = i.ToString() + " out of " + totalPoints.ToString() + " loaded";
                yield return null;
            }
        }

        pointCloud = new GameObject(pFile.name);

        builder.LoadPoints(points, colors, pointCloud);

#if UNITY_EDITOR
        //Store PointCloud
        //UnityEditor.PrefabUtility.SaveAsPrefabAsset(pointCloud, "Assets/Resources/PointCloudMeshes/" + pFile.name + ".prefab");
#endif

        loaded = true;
    }

    private void CreateFolders()
    {
#if UNITY_EDITOR
        if (!Directory.Exists(Application.dataPath + "/Resources/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

        if (!Directory.Exists(Application.dataPath + "/Resources/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", "PointCloudMeshes");
#endif
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