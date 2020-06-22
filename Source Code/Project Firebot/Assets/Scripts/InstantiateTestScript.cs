using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note to self: switch to binary files, it allows you to skip the "string.split" step if done properly

public class InstantiateTestScript : MonoBehaviour
{
    [SerializeField] private GameObject Point;
    [SerializeField] private int PointCount = 100;
    [SerializeField] private Vector2 Range = new Vector2(-100, 100);
    //[SerializeField] private int PointsPerUpdate = 10;

    //private int updateCounter = 0;
    private void Start()
    {
        for (int i = 0; i < PointCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(Range.x, Range.y), Random.Range(Range.x, Range.y), Random.Range(Range.x, Range.y));

            Mesh mesh = new Mesh();

            Vector3[] meshPoints = new Vector3[1] { pos };
            int[] indicies = new int[1] { 0 };
            Color[] meshColors = new Color[1] { Color.white };

            mesh.vertices = meshPoints;
            mesh.colors = meshColors;
            mesh.SetIndices(indicies, MeshTopology.Points, 0);
            mesh.uv = new Vector2[1];
            mesh.normals = new Vector3[1];

            GameObject p = new GameObject("Point" + i);

            p.AddComponent<MeshFilter>();
            p.AddComponent<MeshRenderer>();
            //p.GetComponent<MeshRenderer>().material = vertexMaterial;
            p.GetComponent<MeshFilter>().mesh = mesh;
        }        
    }

    // Update is called once per frame
    private void Update()
    {
        /*
        if (updateCounter < PointCount)
        {
            for (int i = 0; i < PointsPerUpdate; i++)
            {
                Vector3 pos = new Vector3(Random.Range(Range.x, Range.y), Random.Range(Range.x, Range.y), Random.Range(Range.x, Range.y));
                float x = 0 - pos.x;
                float y = 0 - pos.y;
                GameObject p = Instantiate(Point, pos, Quaternion.LookRotation(new Vector3(0, 0, 0), Vector3.up));
                p.transform.LookAt(Vector3.zero, Vector3.up);
                updateCounter++;
            }
        }
        if (updateCounter == PointCount)
        {
            Debug.Log("All Points Done");
            updateCounter++;
        }*/

    }
}
