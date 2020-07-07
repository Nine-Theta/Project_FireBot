using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceOptimization : MonoBehaviour
{
    [SerializeField] int numberOfGroups = 6;
    [SerializeField] float maxDistance = 200f;
    [SerializeField] float factorMultiplier = 2f;
    [SerializeField] bool isExponential = true;
    [SerializeField] GameObject camera;
    int[] groups;

    void Start()
    {
        groups = new int[numberOfGroups];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SeparatePoints();
            CullPoints();
        }
    }

    //Iterates through all points, calculaates the number of points in each group and then deletes some points in each group
    void SeparatePoints()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject point = transform.GetChild(i).gameObject;
            float distance = Vector2.Distance(new Vector2(camera.transform.position.x, camera.transform.position.z), new Vector2(point.transform.position.x, point.transform.position.z));

            if (distance > maxDistance)
            {
                Destroy(point);
                continue;
            }

            int belongedGroup = (int)Mathf.Lerp(0f, numberOfGroups - 1, distance / maxDistance); //linear calculation of the group 
            groups[belongedGroup]++;
            float cullingFactor = 0f;
            if (isExponential)
            {
                cullingFactor = Mathf.Pow(belongedGroup + 1, factorMultiplier);
            }
            else
            {
                cullingFactor = (belongedGroup + 1) * factorMultiplier;
            }

            if (groups[belongedGroup] % cullingFactor != 0) //every n point is ginna stay and all other are going to be deleted
            {                                               //where n is the culling factor
                Destroy(point);
            }
        }
    }

    void CullPoints()
    {

    }
}
