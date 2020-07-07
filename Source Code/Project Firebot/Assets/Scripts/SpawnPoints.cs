    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
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
            Vector3 pos = new Vector3(Random.Range(Range.x, Range.y), Random.Range(Range.x, Range.y), Random.Range(Range.x, Range.y) * 3) + transform.position;
            GameObject p = Instantiate(Point, pos, Quaternion.Euler(Random.Range(0f,360f), Random.Range(0f, 360f), Random.Range(0f, 360f)), transform);
            
            //p.transform.LookAt(new Vector3(0, 0, 30), Vector3.up);
            //BoxCollider collider = p.AddComponent<BoxCollider>();
            //p.GetComponent<BoxCollider>().isTrigger = true;
            //p.GetComponent<BoxCollider>().size = new Vector3(0.1f, 0.1f, 0.1f);
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
