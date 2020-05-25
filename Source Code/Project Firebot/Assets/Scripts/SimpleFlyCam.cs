using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class SimpleFlyCam : MonoBehaviour
{
    [SerializeField] private float SpeedMod = 0.1f;
    
    private Vector3 velocity = Vector3.zero;
    private float drag = 0.9f;
    private Vector3 rotation;

    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        gameObject.transform.Rotate(-Input.GetAxis("Mouse Y"), 0, 0);
        gameObject.transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
        rotation = gameObject.transform.rotation.eulerAngles;
        gameObject.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);

        if (Input.GetKey(KeyCode.W))
            velocity += gameObject.transform.forward * SpeedMod;
        if (Input.GetKey(KeyCode.A))
            velocity -= gameObject.transform.right * SpeedMod;
        if (Input.GetKey(KeyCode.S))
            velocity -= gameObject.transform.forward * SpeedMod;
        if (Input.GetKey(KeyCode.D))
            velocity += gameObject.transform.right * SpeedMod;

        if (Input.GetKey(KeyCode.Space))
            velocity += gameObject.transform.up * SpeedMod;
        if (Input.GetKey(KeyCode.LeftControl))
            velocity -= gameObject.transform.up * SpeedMod;

        gameObject.transform.position += velocity;
        velocity *= 0.9f;
    }
}
