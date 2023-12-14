using SharpDXSimple;
using SharpDX.Windows;

internal static class Program
{
    static SimpleDirectX mainDirectX = null;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var mainForm = new Form1();
        mainDirectX = new SimpleDirectX();
        mainDirectX.InitDevice(mainForm);

        RenderLoop.Run(mainForm, () =>
        {
            mainDirectX.deviceContext.ClearRenderTargetView(mainDirectX.renderTargetView, new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1));
            mainDirectX.deviceContext.Draw(3, 0);
            mainDirectX.swapChain.Present(0, SharpDX.DXGI.PresentFlags.None);
        });
    }
}