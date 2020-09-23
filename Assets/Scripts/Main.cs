using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{
    [SerializeField]
    Shader pclShader;

    Material pclMaterial;

    [SerializeField]
    Shader postShader;

    private RenderTexture _colorTex;
    private RenderTexture _pointTex;

    private ComputeBuffer _argsBuffer;
    private ComputeBuffer _vtxBuffer;
    private readonly int[] _args = { 1, 1, 0, 0 };
    private readonly int[] _vtxs = { 0, 1, 2, 3 };
    private static readonly int VertexBuffer = Shader.PropertyToID("_VertexBuffer");

    private CommandBuffer commandBuffer;

    void Start()
    {


        _argsBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
        _argsBuffer.SetData(_args);
        
        _vtxBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
        _vtxBuffer.SetData(_vtxs);

        pclMaterial = new Material(pclShader);
        pclMaterial.hideFlags = HideFlags.HideAndDontSave;
        pclMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        pclMaterial.SetBuffer(VertexBuffer, _vtxBuffer);


        //render target
        _colorTex = new RenderTexture(Screen.width, Screen.height, 24);
        _colorTex.Create();

        _pointTex = new RenderTexture(Screen.width, Screen.height, 1, RenderTextureFormat.ARGBFloat);
        _pointTex.Create();
        _pointTex.wrapMode = TextureWrapMode.Clamp;
        _pointTex.filterMode = FilterMode.Point;

        commandBuffer = new CommandBuffer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            int w = _pointTex.width;
            int h = _pointTex.height;

            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBAFloat, false);
            tex.filterMode = FilterMode.Point;
            RenderTexture.active = _pointTex;
            Rect rect = new Rect(0, 0, w, h);
            tex.ReadPixels(rect, 0, 0);
            var pixel_x = (int)Input.mousePosition.x;
            var pixel_y = (int)Input.mousePosition.y;
            Color c = tex.GetPixel(pixel_x, pixel_y);
            if (c.r > 0.1 || c.g > 0.1 || c.b > 0.1 || c.a > 0.1)
            {
                Debug.LogFormat("pixel[{0},{1}] = {2}", pixel_x, pixel_y, c);
            } else
            {
                Debug.LogFormat("pixel[{0},{1}] = {2}", pixel_x, pixel_y, 0 );
            }
        }
    }

    void OnRenderObject()
    {

        RenderBuffer[] rbufs = { };
        Camera camera = Camera.main;

        camera.SetTargetBuffers(new[]
        {
            _colorTex.colorBuffer, _pointTex.colorBuffer
        }, _colorTex.depthBuffer);


        commandBuffer.Clear();
        GL.Clear(true, true, Color.clear);
        pclMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, 4);
        commandBuffer.Blit(_colorTex, -1);
        camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);


    }

    private void OnDestroy()
    {
        //T.B.D
    }

}   