using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCulling : MonoBehaviour
{
    [SerializeField] float cullingFactor = 2f;
    List<GameObject> pointsList;
    int boxIndex;
    bool culled;

    private void Start()
    {
        pointsList = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CullPoints();
        }

        //if (!culled)
        //{
        //    CullPoints();
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        pointsList.Add(other.gameObject);
    }

    void CullPoints()
    {
        for (int i = 0; i < pointsList.Count; i++)
        {
            if (i % cullingFactor != 0)
            {
                Destroy(pointsList[i]);
            }
        }

        pointsList.Clear();
        culled = true;
    }

    public void SetBoxIndex(int value)
    {
        boxIndex = value;
    }

    public void SetFactor(float value)
    {
        cullingFactor = value;
    }
}
