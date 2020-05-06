using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//prototype script, not intended for release

public class SimpleFollowScript : MonoBehaviour
{
    [SerializeField] private GameObject Target;
    [SerializeField] private Vector3 Offset;
    void Update()
    {
        transform.position = Target.transform.position + Offset;
    }
}
