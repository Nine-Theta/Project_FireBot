using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImportTestScript : MonoBehaviour
{
    public string FilePath = "Assets/buildingtest4.xyz.pts";
    private string fileExtension = ".SimpleXYZ";
    [Header("Instantiate Settings")]
    [SerializeField] private GameObject Point;
    [SerializeField] private int PointCount = 100;
    [SerializeField] private bool useFileLengthInstead = false;
    [SerializeField] private bool reducePoints = false;
    [SerializeField] private int OneInNPoints = 1;

    private void Start()
    {
        Debug.Log(FilePath);
        /**/
        if (FilePath.EndsWith(".pts"))
        {
            Debug.Log(".pts file loaded, checking for " + fileExtension +" ...");
            if (File.Exists(FilePath + fileExtension))
            {
                Debug.Log(fileExtension + " found! Loading that instead");
                InstatiateTriangles(FilePath + fileExtension);
            }
            else
            {
                Debug.Log(".pts file loaded, converting");
                ConvertPTStoSimpleXYZ(FilePath);
                Debug.Log(".pts file converted, loading " +fileExtension+" ...");
                InstatiateTriangles(FilePath + fileExtension);
            }
        }
        else if (FilePath.EndsWith(fileExtension))
        {
            Debug.Log(fileExtension + " file loaded, proceeding...");
            InstatiateTriangles(FilePath);

        }
        else
        {
            Debug.Log("File missing or unknown type, quitting...");
        }
        /**/
        //ReadFile(FilePath);
        //ConvertPTStoXYZPure(FilePath);
    }

    private void InstatiateTriangles (string path) //Release the Triangles!
    {
        StreamReader reader = new StreamReader(path);
        int counter = PointCount;
        int reducecounter = 0;
        

        if (useFileLengthInstead)
            counter = int.Parse(reader.ReadLine());
        else
            reader.ReadLine();

        for (int i = 0; i < PointCount; i++)
        {
            if (reducePoints)
            {
                reducecounter += 1;
                if (reducecounter >= OneInNPoints) reducecounter = 0;
                if (reducecounter != 0) continue;
            }

            string[] point = reader.ReadLine().Split(' ');
            Vector3 pos = new Vector3(float.Parse(point[0]), float.Parse(point[1]), float.Parse(point[2]));
            GameObject p = Instantiate(Point, pos, Quaternion.LookRotation(new Vector3(0, 0, 0), Vector3.up));
        }
    }



    

    private void ReadFile(string path)
    {
        StreamReader reader = new StreamReader(path);
        //Debug.Log(reader.ReadLine()); //Don't do this with a large number of lines in your file
        reader.Close();
    }

    private void ConvertPTStoSimpleXYZ(string inputPath)
    {
        StreamReader reader = new StreamReader(inputPath);

        string[] lines;
        Int32 length = Int32.Parse(reader.ReadLine()); //max of 2,147,486,647 lines. If we pass this we have bigger problems.
        Debug.Log("line count == " +length);
        lines = new string[length+1];

        lines[0] = length.ToString(); //start the file with the point count

        for (int i = 1; i <= length; i++)
        {
            string line = reader.ReadLine();
            string[] words = line.Split(' ');
            line = words[0] + " " + words[1] + " " + words[2]; //yes, I could've used a for loop, it would be proper practice, but we want performance.
            lines[i] = line;
            //Debug.Log(line); //pro-tip: don't do this with a large amount of lines
        }
        reader.Close();

        System.IO.File.WriteAllLines(inputPath + fileExtension, lines);     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
