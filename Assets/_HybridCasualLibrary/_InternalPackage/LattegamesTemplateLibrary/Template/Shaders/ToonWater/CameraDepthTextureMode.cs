using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDepthTextureMode : MonoBehaviour 
{
    [SerializeField]
    DepthTextureMode depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.DepthNormals;

    private void OnValidate()
    {
        SetCameraDepthTextureMode();
    }

    private void Awake()
    {
        SetCameraDepthTextureMode();
    }

    private void SetCameraDepthTextureMode()
    {
        GetComponent<Camera>().depthTextureMode = depthTextureMode;
    }
}
