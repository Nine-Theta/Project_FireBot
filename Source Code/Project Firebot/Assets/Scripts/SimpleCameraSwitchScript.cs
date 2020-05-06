using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//prototype script, not intended for release

public class SimpleCameraSwitchScript : MonoBehaviour
{
    [SerializeField] private Camera MainCam;
    [SerializeField] private CamSwitch[] Cameras;
    private void Start()
    {
        MainCam = Cameras[0].cam;
        Cameras[0].cam.enabled = true;
    }

    private void Update()
    {
        for (int i = 0; i < Cameras.Length; i++)
        {
            if (Input.GetKeyDown(Cameras[i].key))
            {
                MainCam = Cameras[i].cam;
                Cameras[i].cam.enabled = true;
            }

            if (MainCam != Cameras[i].cam) Cameras[i].cam.enabled = false;
        }
    }
}

[System.Serializable]
public class CamSwitch
{
    [SerializeField] public Camera cam;
    [SerializeField] public KeyCode key;
}


[CustomPropertyDrawer(typeof(CamSwitch))]
public class DamageValuesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // __prefab__ override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate rects
        Rect typeRect = new Rect(position.x, position.y, 100, position.height);
        Rect valueRect = new Rect(position.x + 100, position.y, 90, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("cam"), GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("key"), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
