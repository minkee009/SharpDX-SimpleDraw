using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.D3DCompiler;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

public class SimpleDirectX : IDisposable
{

    public DriverType driverType = DriverType.Null;

    public FeatureLevel[] featureLevels =
        {
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0
        };

    public Device device = null;
    public DeviceContext deviceContext = null;
    public RenderTargetView renderTargetView = null;
    public SwapChain swapChain = null;

    public Factory factory = null;
    public Texture2D backBuffer = null;
    public CompilationResult vertexShaderByteCode = null;
    public VertexShader vertexShader = null;
    public CompilationResult pixelShaderByteCode = null;
    public PixelShader pixelShader = null;
    public InputLayout layout = null;
    public Buffer vertices = null;

    public void Dispose()
    {
        deviceContext?.ClearState();
        deviceContext?.Flush();
        deviceContext?.Dispose();

        device?.Dispose();

        renderTargetView?.Dispose();

        swapChain?.Dispose();

        factory?.Dispose();

        backBuffer?.Dispose();

        vertexShaderByteCode?.Dispose();

        vertexShader?.Dispose();

        pixelShaderByteCode?.Dispose();

        pixelShader?.Dispose();

        layout?.Dispose();

        vertices?.Dispose();
    }

    public void InitDevice(Form mainForm)
    {
        SwapChainDescription sd = new SwapChainDescription()
        {
            BufferCount = 1,
            ModeDescription = new ModeDescription(mainForm.ClientSize.Width, mainForm.ClientSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
            Usage = Usage.RenderTargetOutput,
            SampleDescription = new SampleDescription(1, 0),
            IsWindowed = true,
            OutputHandle = mainForm.Handle
        };

        Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, featureLevels, sd, out device, out swapChain);
        deviceContext = device.ImmediateContext;

        factory = swapChain.GetParent<Factory>();
        factory.MakeWindowAssociation(mainForm.Handle, WindowAssociationFlags.IgnoreAll);

        backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
        renderTargetView = new RenderTargetView(device, backBuffer);

        vertexShaderByteCode = ShaderBytecode.CompileFromFile("Shader/VertexColor.hlsl", "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
        vertexShader = new VertexShader(device, vertexShaderByteCode);

        pixelShaderByteCode = ShaderBytecode.CompileFromFile("Shader/VertexColor.hlsl", "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
        pixelShader = new PixelShader(device, pixelShaderByteCode);

        layout = new InputLayout(
            device,
            ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                });

        vertices = Buffer.Create(device, BindFlags.VertexBuffer,
            new[]
            {
                    new RawVector4(0.0f, 0.5f, 0.5f, 1.0f), new RawVector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new RawVector4(0.5f, -0.5f, 0.5f, 1.0f), new RawVector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new RawVector4(-0.5f, -0.5f, 0.5f, 1.0f), new RawVector4(0.0f, 0.0f, 1.0f, 1.0f)
            });

        deviceContext.InputAssembler.InputLayout = layout;
        deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, 32, 0));
        deviceContext.VertexShader.Set(vertexShader);
        deviceContext.Rasterizer.SetViewport(new Viewport(0, 0, mainForm.ClientSize.Width, mainForm.ClientSize.Height, 0.0f, 1.0f));
        deviceContext.PixelShader.Set(pixelShader);
        deviceContext.OutputMerger.SetTargets(renderTargetView);
    }
}