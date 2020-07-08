using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsole : MonoBehaviour
{
    private static UIConsole _instance;

    [SerializeField] private Text text;
    private Queue<string> queue = new Queue<string>();

    public static UIConsole Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIConsole>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<UIConsole>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null) Destroy(this);
        DontDestroyOnLoad(this);
    }

    public void SetMessage(string message)
    {
        if (queue.Count == 16) queue.Dequeue();
        
        queue.Enqueue(message + '\n');

        string temp = "";
        foreach(string m in queue)
        {
            temp += m;
        }
        text.text = temp;
    }
}
