using System.Diagnostics;
using Vortice;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using Vortice.Multimedia;
using Vortice.WIC;
using Vortice.WinForms;
using Vortice.XAudio2;
using VorticeBitmapInterpolationMode = Vortice.Direct2D1.BitmapInterpolationMode;

namespace GameFrameworkTest;

internal static class Program
{
	public static uint counter = 0;
	[STAThread]
	private static void Main()
	{
		ApplicationConfiguration.Initialize();

		using RenderForm _MainForm = new()
		{
			Text = "Vortice Direct2D - PNG Render",
			ClientSize = new System.Drawing.Size(800, 480)
		};

		_MainForm.MouseDown += (sender, e) =>
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
				Hwnd = _MainForm.Handle,
				PixelSize = new SizeI(_MainForm.ClientSize.Width, _MainForm.ClientSize.Height)
			});

		// WIC 팩토리 생성 (이미지 파일 디코딩용)
		using IWICImagingFactory wicFactory = new IWICImagingFactory();

		// PNG 파일을 읽어서 Direct2D 비트맵으로 변환
		// 주의: "res_checker.png" 파일이 실행 파일(.exe)과 동일한 경로에 있어야 합니다.
		using ID2D1Bitmap? _bitmap = LoadBitmap(renderTarget, wicFactory,
			Path.Combine( AppContext.BaseDirectory, "resource", "res_checker.png"));

		Stopwatch stopwatch = Stopwatch.StartNew();

		RenderLoop.Run(_MainForm, () =>
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

	/// <summary>
	/// WIC를 사용하여 이미지 파일을 로드하고 ID2D1Bitmap으로 변환하는 메서드입니다.
	/// </summary>
	private static ID2D1Bitmap? LoadBitmap(ID2D1RenderTarget renderTarget, IWICImagingFactory wicFactory, string filePath)
	{
		if (!System.IO.File.Exists(filePath))
		{
			return null;
		}

		// 1. 파일로부터 디코더 생성
		using IWICBitmapDecoder decoder = wicFactory.CreateDecoderFromFileName(filePath);

		// 2. 첫 번째 프레임(보통 인덱스 0) 가져오기
		using IWICBitmapFrameDecode frame = decoder.GetFrame(0);

		// 3. 포맷 컨버터 생성
		using IWICFormatConverter converter = wicFactory.CreateFormatConverter();

		// 4. Direct2D와 호환되는 픽셀 포맷(32bpp PBGRA)으로 변환 초기화
		converter.Initialize(frame, Vortice.WIC.PixelFormat.Format32bppPBGRA);

		// 5. WIC 비트맵 데이터를 기반으로 Direct2D 비트맵 생성
		return renderTarget.CreateBitmapFromWicBitmap(converter);
	}
}