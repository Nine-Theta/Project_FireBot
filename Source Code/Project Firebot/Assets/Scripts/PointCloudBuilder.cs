using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PointCloudBuilder : MonoBehaviour
{
    //public string filePath; // "\PointCloud\xyzrgb_manuscript" <- test pointcloud

    [SerializeField] private Material vertexMaterial;

    [SerializeField] private int meshPointLimit = 65535;

    //GUI
    private float progress = 0;
    private string guiText = "";
    private bool loaded = false;
    
    [SerializeField] private Color defaultColor = Color.green;
    private const float colDiv = 0.00392156862745098f; // 1/255

    private void Start()
    {
        //LoadOFF(filePath + ".off");
    }

    /// <summary>
    /// Decides how many points are allowed in a single group(mesh), before a new one is made.
    /// Lower values provide smoother loading, but decrease overall performance.
    /// At the time of writing Unity has an internal limit of 65535 vertices (uint16), afterwhich it will no longer display additional points.
    /// </summary>
    public int MeshPointLimit
    {
        get { return meshPointLimit; }
        set { meshPointLimit = value; }
    }

    public Color DefaultColor
    {
        get { return defaultColor; }
        set { defaultColor = value; }
    }

    public Material VertexMaterial
    {
        get { return vertexMaterial; }
        set { vertexMaterial = value; }
    }

    public void LoadPoints(Vector3[] pPoints, GameObject pParent = null)
    {
        if (pParent == null) pParent = gameObject;
        LoadPoints(pPoints, defaultColor, pParent);
    }
    
    public void LoadPoints(Vector3[] pPoints, Color pColor, GameObject pParent = null)
    {
        if (pParent == null) pParent = gameObject;
        StartCoroutine(Load(pPoints, pColor, pParent));
    }

    public void LoadPoints(Vector3[] pPoints, Vector3[] pColors, GameObject pParent = null)
    {
        if (pParent == null) pParent = gameObject;
        StartCoroutine(Load(pPoints, pColors, pParent));
    }

    public void LoadPoints(Vector3[] pPoints, Vector4[] pColors, GameObject pParent = null)
    {
        if (pParent == null) pParent = gameObject;
        StartCoroutine(Load(pPoints, pColors, pParent));
    }

    public void LoadPoints(Vector3[] pPoints, Color[] pColors, GameObject pParent = null)
    {
        if (pParent == null) pParent = gameObject;
        StartCoroutine(Load(pPoints, pColors, pParent));
    }

    #region IEnumerators

    private IEnumerator Load(Vector3[] pPoints, Color pColor, GameObject pParent)
    {
        int pointCount = pPoints.Length;
        int groupCount = pointCount / meshPointLimit;       //zero indexed

        Vector3[] groupPoints = new Vector3[meshPointLimit];
        int[] indicies = new int[meshPointLimit];
        Color[] groupColors = new Color[meshPointLimit];

        int lastGroupCount = pointCount - (meshPointLimit * groupCount);

        for (int i = 0; i < groupCount; i++)
        {
            for (int j = 0; j < meshPointLimit; j++)
            {
                groupPoints[j] = pPoints[i * meshPointLimit + j];
                indicies[j] = j;
                groupColors[j] = pColor;
                    }
            InstantiateMesh(groupPoints, groupColors, indicies, pParent, i);

            guiText = i.ToString() + " out of " + groupCount.ToString() + " PointGroups loaded";
            yield return null;
        }

        //remaining points
        groupPoints = new Vector3[lastGroupCount];
        indicies = new int[lastGroupCount];
        groupColors = new Color[lastGroupCount];
        for (int j = 0; j < lastGroupCount; j++)
        {
            groupPoints[j] = pPoints[groupCount * meshPointLimit + j];
            indicies[j] = j;
            groupColors[j] = pColor;
        }

        InstantiateMesh(groupPoints, groupColors, indicies, pParent, groupCount);
    }

    private IEnumerator Load(Vector3[] pPoints, Vector3[] pColors, GameObject pParent)
    {
        int pointCount = pPoints.Length;
        int groupCount = pointCount / meshPointLimit;       //zero indexed

        Vector3[] groupPoints = new Vector3[meshPointLimit];
        int[] indicies = new int[meshPointLimit];
        Color[] groupColors = new Color[meshPointLimit];

        int lastGroupCount = pointCount - (meshPointLimit * groupCount);

        for (int i = 0; i < groupCount; i++)
        {
            for (int j = 0; j < meshPointLimit; j++)
            {
                groupPoints[j] = pPoints[i * meshPointLimit + j];
                indicies[j] = j;
                groupColors[j] = new Color(pColors[i * meshPointLimit + j].x * colDiv, pColors[i * meshPointLimit + j].y * colDiv, pColors[i * meshPointLimit + j].z * colDiv);
            }
            InstantiateMesh(groupPoints, groupColors, indicies, pParent, i);

            guiText = i.ToString() + " out of " + groupCount.ToString() + " PointGroups loaded";
            yield return null;
        }

        //remaining points
        groupPoints = new Vector3[lastGroupCount];
        indicies = new int[lastGroupCount];
        groupColors = new Color[lastGroupCount];
        for (int j = 0; j < lastGroupCount; j++)
        {
            groupPoints[j] = pPoints[groupCount * meshPointLimit + j];
            indicies[j] = j;
            groupColors[j] = groupColors[j] = new Color(pColors[groupCount * meshPointLimit + j].x * colDiv, pColors[groupCount * meshPointLimit + j].y * colDiv, pColors[groupCount * meshPointLimit + j].z * colDiv);
        }

        InstantiateMesh(groupPoints, groupColors, indicies, pParent, groupCount);
    }

    private IEnumerator Load(Vector3[] pPoints, Vector4[] pColors, GameObject pParent)
    {
        int pointCount = pPoints.Length;
        int groupCount = pointCount / meshPointLimit;       //zero indexed

        Vector3[] groupPoints = new Vector3[meshPointLimit];
        int[] indicies = new int[meshPointLimit];
        Color[] groupColors = new Color[meshPointLimit];

        int lastGroupCount = pointCount - (meshPointLimit * groupCount);

        for (int i = 0; i < groupCount; i++)
        {
            for (int j = 0; j < meshPointLimit; j++)
            {
                groupPoints[j] = pPoints[i * meshPointLimit + j];
                indicies[j] = j;
                groupColors[j] = pColors[i * meshPointLimit + j];
            }
            InstantiateMesh(groupPoints, groupColors, indicies, pParent, i);

            guiText = i.ToString() + " out of " + groupCount.ToString() + " PointGroups loaded";
            yield return null;
        }

        //remaining points
        groupPoints = new Vector3[lastGroupCount];
        indicies = new int[lastGroupCount];
        groupColors = new Color[lastGroupCount];
        for (int j = 0; j < lastGroupCount; j++)
        {
            groupPoints[j] = pPoints[groupCount * meshPointLimit + j];
            indicies[j] = j;
            groupColors[j] = pColors[groupCount * meshPointLimit + j];
        }

        InstantiateMesh(groupPoints, groupColors, indicies, pParent, groupCount);
}

    private IEnumerator Load(Vector3[] pPoints, Color[] pColors, GameObject pParent)
    {
        int pointCount = pPoints.Length;
        int groupCount = pointCount / meshPointLimit;       //zero indexed

        Vector3[] groupPoints = new Vector3[meshPointLimit];
        int[] indicies = new int[meshPointLimit];
        Color[] groupColors = new Color[meshPointLimit];
        
        int lastGroupCount = pointCount - (meshPointLimit * groupCount);

        for (int i = 0; i < groupCount; i++)
        {          
            for(int j = 0; j < meshPointLimit; j++)
            {
                groupPoints[j] = pPoints[i * meshPointLimit + j];
                indicies[j] = j;
                groupColors[j] = pColors[i * meshPointLimit + j];
            }
            InstantiateMesh(groupPoints, groupColors, indicies, pParent, i);

            guiText = i.ToString() + " out of " + groupCount.ToString() + " PointGroups loaded";
            yield return null;
        }

        //remaining points
        groupPoints = new Vector3[lastGroupCount];
        indicies = new int[lastGroupCount];
        groupColors = new Color[lastGroupCount];
        for (int j = 0; j < lastGroupCount; j++)
        {
            groupPoints[j] = pPoints[groupCount * meshPointLimit + j];
            indicies[j] = j;
            groupColors[j] = pColors[groupCount * meshPointLimit + j];
        }

        InstantiateMesh(groupPoints, groupColors, indicies, pParent, groupCount);
    }

    #endregion IEnumerators

    private void InstantiateMesh(Vector3[] pPoints, Color[] pColors, int[] pIndicies, GameObject pParent, int pMeshIndex = 0)
    {
        Debug.Log("instatiating...");
        GameObject pointGroup = new GameObject("PointGroup"+pMeshIndex+" ("+pPoints.Length+")");
        pointGroup.AddComponent<MeshFilter>();
        pointGroup.AddComponent<MeshRenderer>();
        pointGroup.GetComponent<MeshRenderer>().material = vertexMaterial;

        pointGroup.GetComponent<MeshFilter>().mesh = CreateMesh(pPoints, pColors, pIndicies);
        pointGroup.transform.parent = pParent.transform;
    }


    private Mesh CreateMesh(Vector3[] pPoints, Color[] pColors, int[] pIndicies)
    {
        Debug.Log("creating mesh...");
        Mesh mesh = new Mesh();

        int pointCount = pPoints.Length;

        mesh.vertices = pPoints;
        Debug.Log(mesh.vertices.Length);
        mesh.colors = pColors;
        mesh.SetIndices(pIndicies, MeshTopology.Points, 0);
        mesh.uv = new Vector2[pointCount];
        mesh.normals = new Vector3[pointCount];

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
