using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//prototype script, not intended for release

//[RequireComponent(typeof(Rigidbody))]
public class SimplePlayerController : MonoBehaviour
{
    [SerializeField] private Transform CamTransform;
    [SerializeField] private Rigidbody body;

    void Start()
    {
        //body = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,Input.GetAxis("Mouse X"),0);
        CamTransform.Rotate(-Input.GetAxis("Mouse Y"), 0, 0);
        body.AddRelativeForce(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
    }
}
