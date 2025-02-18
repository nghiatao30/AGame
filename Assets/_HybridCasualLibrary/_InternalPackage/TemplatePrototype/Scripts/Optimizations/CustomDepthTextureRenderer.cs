using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("HyrphusQ/Rendering/CustomDepthTextureRenderer")]
public class CustomDepthTextureRenderer : MonoBehaviour
{
    private readonly static string ShadowCasterPassName = "ShadowCaster";
    private readonly static string BufferUpdateDepthTextureName = "UpdateDepthTexture";
    private readonly static string BufferDepthNormalsTextureName = "UpdateDepthNormalsTexture";
    private readonly static string DepthNormalsTextureShader = "Hidden/Internal-DepthNormalsTexture";
    private readonly static string DepthTextureShader = "Legacy Shaders/VertexLit";
    private readonly static int DepthTexture_ID = Shader.PropertyToID("_CustomCameraDepthTexture");
    private readonly static int DepthNormalsTexture_ID = Shader.PropertyToID("_CustomCameraDepthNormalsTexture");

    public DepthTextureMode m_DepthTextureMode;
    [SerializeField]
    private List<Renderer> m_InitialRenderers;

    private Camera m_MainCamera;
    private Material m_DepthMaterial, m_DepthNormalsMaterial;
    private SortedDictionary<int, List<Renderer>> m_RenderersDictionary;
    private SortedDictionary<int, List<Renderer>> renderersDictionary
    {
        get
        {
            if (m_RenderersDictionary == null)
                m_RenderersDictionary = new SortedDictionary<int, List<Renderer>>();
            return m_RenderersDictionary;
        }
    }
    private Dictionary<DepthTextureMode, CommandBuffer> m_CommandBufferDictionary;
    private Dictionary<DepthTextureMode, CommandBuffer> commandBufferDictionary
    {
        get
        {
            if(m_CommandBufferDictionary == null)
                m_CommandBufferDictionary = new Dictionary<DepthTextureMode, CommandBuffer>();
            return m_CommandBufferDictionary;
        }
    }
    private void Awake()
    {
        m_MainCamera = Camera.main;
        m_DepthMaterial = new Material(Shader.Find(DepthTextureShader));
        m_DepthNormalsMaterial = new Material(Shader.Find(DepthNormalsTextureShader));
        if (m_InitialRenderers != null && m_InitialRenderers.Count > 0)
        {
            for (int i = 0; i < m_InitialRenderers.Count; i++)
            {
                AddRenderer(m_InitialRenderers[i]);
            }
        }
    }
    private void OnEnable()
    {
        InitializeCommandBuffer();
    }
    private void OnDisable()
    {
        RemoveCommandBuffer();
    }
    private void OnDestroy()
    {
        RemoveCommandBuffer();
        m_RenderersDictionary.Clear();
        m_RenderersDictionary = null;
        m_CommandBufferDictionary = null;
        Destroy(m_DepthMaterial);
        Destroy(m_DepthNormalsMaterial);
    }

    private void InitializeCommandBuffer()
    {
        if (commandBufferDictionary.Count > 0)
            commandBufferDictionary.Clear();
        if ((m_DepthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.Depth)
        {
            var command = new CommandBuffer();
            command.name = BufferUpdateDepthTextureName;
            command.GetTemporaryRT(DepthTexture_ID, -1, -1, 24, FilterMode.Bilinear, RenderTextureFormat.Depth, RenderTextureReadWrite.Default);
            command.SetRenderTarget(DepthTexture_ID);
            command.ClearRenderTarget(true, true, Color.clear);
            foreach (var item in renderersDictionary)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    DrawDepthRenderer(item.Value[i], ref command);
                }
            }
            command.ReleaseTemporaryRT(DepthTexture_ID);

            // Add command buffer to camera events
            m_MainCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, command);

            // Cached into to dictionary
            commandBufferDictionary.Add(DepthTextureMode.Depth, command);
        }
        if ((m_DepthTextureMode & DepthTextureMode.DepthNormals) == DepthTextureMode.DepthNormals)
        {
            var command = new CommandBuffer();
            command.name = BufferDepthNormalsTextureName;
            command.GetTemporaryRT(DepthNormalsTexture_ID, -1, -1, 24, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            command.SetRenderTarget(DepthNormalsTexture_ID);
            command.ClearRenderTarget(true, true, new Color(0.5f, 0.5f, 1f, 1f));
            foreach (var item in renderersDictionary)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    DrawDepthNormalsRenderer(item.Value[i], ref command);
                }
            }
            command.ReleaseTemporaryRT(DepthNormalsTexture_ID);

            // Add command buffer to camera events
            m_MainCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, command);

            // Cached into to dictionary
            commandBufferDictionary.Add(DepthTextureMode.DepthNormals, command);
        }
    }
    private void RemoveCommandBuffer()
    {
        if (m_MainCamera == null)
            return;
        foreach (var item in commandBufferDictionary)
        {
            var command = item.Value;
            m_MainCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, command);
            command.Clear();
            command.Dispose();
        }
        commandBufferDictionary.Clear();
    }
    private void RefreshCommandBuffer()
    {
        RemoveCommandBuffer();
        InitializeCommandBuffer();
    }
    // *NOTE: This method can't handle Mesh with multiple submesh. Modify if you need handle with multiple submesh
    private void DrawDepthRenderer(Renderer renderer, ref CommandBuffer command)
    {
        int shaderPass = m_DepthMaterial.FindPass(ShadowCasterPassName);
        if (shaderPass == -1)
            return;
        command.DrawRenderer(renderer, m_DepthMaterial, 0, shaderPass);
    }
    // *NOTE: This method can't handle Mesh with multiple submesh. Modify if you need handle with multiple submesh
    private void DrawDepthNormalsRenderer(Renderer renderer, ref CommandBuffer command)
    {
        command.DrawRenderer(renderer, m_DepthNormalsMaterial, 0);
    }
    private void AddRenderer(Renderer renderer)
    {
        if (renderer.sharedMaterial.renderQueue >= 2500)
            return;
        var renderQueue = renderer.sharedMaterial.renderQueue;
        if (!renderersDictionary.ContainsKey(renderQueue))
        {
            renderersDictionary.Add(renderQueue, new List<Renderer>());
        }
        var renderers = renderersDictionary[renderQueue];
        renderers.Add(renderer);
        renderers.Sort((Renderer x, Renderer y) => x.transform.position.z.CompareTo(y.transform.position.z));
    }

    public void OnAddRenderer(Renderer renderer)
    {
        AddRenderer(renderer);
        RefreshCommandBuffer();
    }
}
