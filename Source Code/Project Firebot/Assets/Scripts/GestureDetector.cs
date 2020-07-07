using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour
{
    public float threshold = 0.1f;
    public OVRSkeleton skeleton;
    public List<Gesture> gestures;
    public bool debugMode = true;
    private List<OVRBone> fingerBones;
    private Gesture previousGesture;

    IEnumerator GetFingerBones()
    {
        do
        {
            fingerBones = new List<OVRBone>(skeleton.Bones);
            yield return null;
        } while (fingerBones.Count <= 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetFingerBones());
        previousGesture = new Gesture();
    }

    // Update is called once per frame
    void Update()
    {
        if(debugMode && fingerBones.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }

        if (debugMode && Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("List of Gestures: ");
            foreach (var gesture in gestures)
            {
                Debug.Log("Gesture: " + gesture.name);
            }
        }

        Gesture currentGesture = Recognize();
        bool hasRecognized = !currentGesture.Equals(new Gesture());
        // Check if new gesture
        if (hasRecognized && !currentGesture.Equals(previousGesture))
        {
            // New gesture
            Debug.Log("New gesture found: " + currentGesture.name);
            UIConsole.Instance.SetMessage("New gesture found: " + currentGesture.name);
            previousGesture = currentGesture;
            currentGesture.onRecognized.Invoke();
        }
    }

    void Save()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            // Finger position relative to root
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerDatas = data;
        gestures.Add(g);
    }

    Gesture Recognize()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);

                if(distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }

            if(!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }
}
