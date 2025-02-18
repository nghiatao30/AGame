using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LatteGames.EditorUtil
{
    public class ScreenShotHelper : MonoBehaviour
    {
        [SerializeField] private List<ScreenShotConfiguration> screenShotConfigurations = new List<ScreenShotConfiguration>();
        [SerializeField] private KeyCode captureKeyCode = KeyCode.Backslash;
        [SerializeField] private Vector3 overcanvasPositionOffset = new Vector3(9999,9999,9999);
#if UNITY_EDITOR
        private int screenShotCount = 0;

        private CoroutineRunner runner;
        private void Awake()
        {
            runner = CoroutineRunner.CreateCoroutineRunner(false);
            runner.transform.SetParent(transform);
        }

        private void Update()
        {
            if(Input.GetKeyDown(captureKeyCode))
            {
                runner.StartManagedCoroutine(TakeScreenShot(), CoroutineRunner.InteruptBehaviour.Ignore);
            }
        }

        private IEnumerator TakeScreenShot()
        {
            var timescale = Time.timeScale;
            Time.timeScale = 0;
            var cameras = new List<Camera>(GameObject.FindObjectsOfType<Camera>());
            cameras.Sort((a,b)=>a.depth.CompareTo(b.depth));

            var overlayCamera = CreateOverlayCanvasCamera();
            foreach (var config in screenShotConfigurations)
            {
                RenderTexture rt = RenderTexture.GetTemporary(config.width, config.height, 24);
                foreach (var camera in cameras)
                    yield return ForceRender(camera, rt);
                /*
                    scene render completed
                    fake overlay canvas
                    clear depth as overlay canvas will need to be drawn on top of everything else
                */
                ClearDepth(rt);
                
                List<Canvas> overlayCanvas = new List<Canvas>(GameObject.FindObjectsOfType<Canvas>());
                overlayCanvas.RemoveAll(canvas => canvas.renderMode != RenderMode.ScreenSpaceOverlay);
                overlayCanvas.Sort((c1,c2)=>c1.sortingOrder.CompareTo(c2.sortingOrder));
                foreach (var canvas in overlayCanvas)
                {
                    var oldWorldCamera = canvas.worldCamera;
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = overlayCamera;
                    yield return ForceRender(overlayCamera, rt);
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.worldCamera = oldWorldCamera;
                }
                
                RenderTexture prevRT = RenderTexture.active;
                RenderTexture.active = rt;
                Texture2D tex2D = new Texture2D(config.width, config.height);
                tex2D.ReadPixels(new Rect(0, 0, config.width, config.height),0,0);
                tex2D.Apply();
                RenderTexture.active = prevRT;

                var bytes = tex2D.EncodeToJPG(100);
                var savingPath = Path.Combine(config.destinationFolder, $"{DateTime.Now.ToString("dddd, MMMM dd yyyy HH_mm_ss.f")}_{screenShotCount}.jpg");
                if(!Directory.Exists(Path.GetDirectoryName(savingPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(savingPath));

                File.WriteAllBytes(savingPath, bytes);
                rt.Release();
                DestroyImmediate(tex2D);
            }
            Destroy(overlayCamera.gameObject);
            screenShotCount++;
            Time.timeScale = timescale;
            Debug.Log("Screenshot captured");
        }

        private void ClearDepth(RenderTexture rt)
        {
            var currentActive = RenderTexture.active;
            RenderTexture.active = rt;
            GL.Clear(true, false, Color.clear);
            RenderTexture.active = currentActive;
        }

        private Camera CreateOverlayCanvasCamera()
        {
            Camera overlayCameraRender = new GameObject("overlayCameraRender").AddComponent<Camera>();
            overlayCameraRender.transform.SetParent(transform);
            overlayCameraRender.transform.position = overcanvasPositionOffset;
            overlayCameraRender.clearFlags = CameraClearFlags.Nothing;
            overlayCameraRender.enabled = false;
            return overlayCameraRender;
        }

        private IEnumerator ForceRender(Camera camera, RenderTexture targetTexture)
        {
            camera.targetTexture = targetTexture;
            yield return null;
            camera.Render();
            camera.targetTexture = null;
        }

        private void OnValidate()
        {
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;
            foreach (var config in screenShotConfigurations)
            {
                var parentProjectDir = Path.GetDirectoryName(Application.dataPath);
                if (String.IsNullOrEmpty(config.destinationFolder))
                    config.destinationFolder = Path.Combine(parentProjectDir, "Screenshot", $"{config.width}x{config.height}");
            }
        }

        //bool PrefabModeIsActive()
        //{
        //    return UnityEditor.Experimental.SceneManagement.
        //        PrefabStageUtility.GetCurrentPrefabStage() != null;
        //}
#endif
        [Serializable]
        public class ScreenShotConfiguration
        {
            public int width, height;
            public string destinationFolder;
        }
    }
}