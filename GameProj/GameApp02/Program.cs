using System.Diagnostics;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using Vortice.Multimedia;
using Vortice.WIC;
using Vortice.XAudio2;
using VorticeBitmapInterpolationMode = Vortice.Direct2D1.BitmapInterpolationMode;
using Glc2D;

internal static class Program
{
	public static uint counter = 0;
	[STAThread]
	private static void Main()
	{
		ApplicationConfiguration.Initialize();

		using Vortice.WinForms.RenderForm _mainForm = new()
		{
			Text = "Vortice Direct2D - PNG Render",
			ClientSize = new System.Drawing.Size(800, 480)
		};

		_mainForm.MouseDown += (sender, e) =>
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				counter++;
				Console.WriteLine($"Left mouse button clicked. Counter: {counter}");
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				counter++;
				Console.WriteLine($"Right mouse button clicked. Counter: {counter}");
			}
		};

		using ID2D1Factory1 factory = D2D1.D2D1CreateFactory<ID2D1Factory1>();

		using ID2D1HwndRenderTarget renderTarget = factory.CreateHwndRenderTarget(
			default,
			new HwndRenderTargetProperties
			{
				Hwnd = _mainForm.Handle,
				PixelSize = new SizeI(_mainForm.ClientSize.Width, _mainForm.ClientSize.Height)
			});

		
		// PNG 파일을 읽어서 Direct2D 비트맵으로 변환
		using ID2D1Bitmap? _bitmap = GlcGameUtil.LoadBitmap(renderTarget, Path.Combine( AppContext.BaseDirectory, "resource", "res_checker.png"));

		Stopwatch stopwatch = Stopwatch.StartNew();

		Vortice.WinForms.RenderLoop.Run(_mainForm, () =>
		{
			double elapsed = stopwatch.Elapsed.TotalSeconds;

			Color4 clearColor = new(
				red: (float)(Math.Sin(elapsed) * 0.5 + 0.5),
				green: (float)(Math.Sin(elapsed + Math.PI / 2.0) * 0.5 + 0.5),
				blue: (float)(Math.Sin(elapsed + Math.PI) * 0.5 + 0.5),
				alpha: 1.0f);

			renderTarget.BeginDraw();
			renderTarget.Clear(clearColor);

			// 비트맵이 성공적으로 로드되었다면 렌더링
			if (_bitmap != null)
			{
				renderTarget.DrawBitmap(_bitmap, 1.0f, Vortice.Direct2D1.BitmapInterpolationMode.Linear);

				Rect destination = new Rect(400, 300, 300, 200);
				Rect sourceRect = new Rect(200,100, 300, 200);
				renderTarget.DrawBitmap(
				_bitmap,
				destination,
				1.0f,
				VorticeBitmapInterpolationMode.Linear, sourceRect);
			}

			//Console.WriteLine($"FPS: {1.0 / (stopwatch.Elapsed.TotalSeconds - elapsed):F2}");
			renderTarget.EndDraw();
		});
	}
}
