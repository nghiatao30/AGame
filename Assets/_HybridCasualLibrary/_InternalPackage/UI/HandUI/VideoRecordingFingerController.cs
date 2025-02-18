using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LazyTransform))]
public class VideoRecordingFingerController : MonoBehaviour {
    public LazyTransform lazyTransform;
    public float mouseDownScale = 0.9f;
    private void Update () {
        lazyTransform.Position = Input.mousePosition;
        if (Input.GetMouseButtonDown (0)) {
            lazyTransform.LocalScale = Vector3.one * mouseDownScale;
        }
        if (Input.GetMouseButtonUp (0)) {
            lazyTransform.LocalScale = Vector3.one;
        }
    }
}