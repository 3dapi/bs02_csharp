using System;
using Vortice.Direct2D1;
using Vortice.DirectWrite; // 추가
using Vortice.Mathematics;

namespace glc
{
    internal class AppD2DContext : IDisposable
    {
        public ID2D1HwndRenderTarget RenderTarget { get; private set; }
        public ID2D1Factory1 Factory { get; private set; }
        
        // 폰트 처리를 위한 DirectWrite 팩토리 추가
        public IDWriteFactory DWriteFactory { get; private set; }

        public AppD2DContext(IntPtr hwnd, int width, int height)
        {
            Factory = D2D1.D2D1CreateFactory<ID2D1Factory1>();
            
            // DirectWrite 팩토리 생성 로직 추가
            DWriteFactory = DWrite.DWriteCreateFactory<IDWriteFactory>();

            RenderTarget = Factory.CreateHwndRenderTarget(
                new RenderTargetProperties(), 
                new HwndRenderTargetProperties
                {
                    Hwnd = hwnd,
                    PixelSize = new SizeI(width, height)
                });
        }

        public void Resize(int width, int height)
        {
            RenderTarget?.Resize(new SizeI(width, height));
        }

        public void Dispose()
        {
            RenderTarget?.Dispose();
            DWriteFactory?.Dispose(); // 해제 추가
            Factory?.Dispose();
        }
    }
}