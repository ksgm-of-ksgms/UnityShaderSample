//#define READ_RENDER_TARGET
//#define USE_INDIRECT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    RenderTexture debugTexture;

    [SerializeField]
    Shader pclShader;
    Material pclMaterial;
    
    private ComputeBuffer _argsBuffer;
    //private ComputeBuffer _vtxBuffer;
    private readonly int[] _args = { 1, 1, 0, 0 };
    //private static readonly int VertexBuffer = Shader.PropertyToID("_VertexBuffer");

    void Start()
    {
        debugTexture = new RenderTexture(1024, 1024, 1, RenderTextureFormat.ARGBFloat);
        debugTexture.Create();
        debugTexture.wrapMode = TextureWrapMode.Clamp;
        debugTexture.filterMode = FilterMode.Point;
        
        _argsBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
        _argsBuffer.SetData(_args);
        pclMaterial = new Material(pclShader);

        //_vtxBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
        //_vtxBuffer.SetData(_args);
        //pclMaterial.SetBuffer(VertexBuffer, _vtxBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        //cshader.Dispatch(kernel_Lorenz, particleNum / 64, 1, 1);
    }

    void OnRenderObject()
    {


#if READ_RENDER_TARGET
        Graphics.SetRenderTarget(debugTexture);
#endif
#if USE_INDIRECT
        var buffer = new CommandBuffer();
        var cam = Camera.current;
        cam.RemoveCommandBuffers(CameraEvent.AfterSkybox);
        buffer.DrawProceduralIndirect(Matrix4x4.identity, pclMaterial, 0, MeshTopology.Points, _argsBuffer);
        cam.AddCommandBuffer(CameraEvent.AfterSkybox, buffer);
#else
        GL.Clear(true, true, Color.clear);
        pclMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, 4);
#endif

#if READ_RENDER_TARGET
        int w = debugTexture.width;
        int h = debugTexture.height;

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBAFloat, false);
        tex.filterMode = FilterMode.Point;
        RenderTexture.active = debugTexture;
        Rect rect = new Rect(0, 0, w, h);
        tex.ReadPixels(rect, 0, 0);
        for (int pixel_y = 0; pixel_y < h; ++pixel_y)
        {
            for (int pixel_x = 0; pixel_x < w; ++pixel_x)
            {
                Color c = tex.GetPixel(pixel_x, pixel_y);
                if (c.r > 0.1 || c.g > 0.1 || c.b > 0.1 || c.a > 0.1)
                {
                    Debug.LogFormat("pixel[{0},{1}] = {2}", pixel_x, pixel_y, c);
                }
            }
        }
#endif

    }

    private void OnDestroy()
    {
        //T.B.D
    }
}   