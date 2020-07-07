using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoundingBoxes : MonoBehaviour
{
    [SerializeField] int numberOfBoxes;
    [SerializeField] float maxDistance = 200f;
    [SerializeField] float cullingFactorMultiplier = 2f;
    [SerializeField] GameObject boxPrefab;

    void Start()
    {
        CreateBoxes();
    }

    void CreateBoxes()
    {
        for (int i = 0; i < numberOfBoxes; i++)
        {
            GameObject boundingBox = Instantiate(boxPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            boundingBox.GetComponent<PointCulling>().SetBoxIndex(i);
            boundingBox.GetComponent<PointCulling>().SetFactor(cullingFactorMultiplier * i); //Culling ratio to one point 
        }
    }

}