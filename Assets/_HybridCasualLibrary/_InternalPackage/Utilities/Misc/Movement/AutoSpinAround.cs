using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpinAround : MonoBehaviour
{
    [SerializeField]
    Vector3 rotateDirection = new Vector3(0f, 0f, 0.15f);
    [SerializeField]
    float speed = 1f;
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime * rotateDirection, Space.Self);
    }
}