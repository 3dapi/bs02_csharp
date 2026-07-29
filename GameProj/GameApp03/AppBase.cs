using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;
using Vortice.WinForms;

namespace glc
{
	internal class AppBase : IDisposable
	{
		public static AppBase? Instance { get; private set; }
		public ID2D1HwndRenderTarget RenderTarget => _graphics.RenderTarget;
		public IDWriteFactory DWriteFactory => _graphics.DWriteFactory;

		private RenderForm _mainForm;
		private AppD2DContext _graphics;
		protected AppMouseEvent _mouseApp; // 자식 클래스에서 마우스 상태를 읽기 위해 protected로 변경
		private ID2D1Bitmap? _bitmap;
		private Stopwatch _stopwatch = new();
		private Color4 _clearColor = new();

		public AppBase()
		{
			Instance = this;
			_mainForm = new RenderForm { Text = "Vortice Direct2D", ClientSize = new(800, 480) };
			_mainForm.Resize += (sender, e) => { _graphics?.Resize(_mainForm.ClientSize.Width, _mainForm.ClientSize.Height); };

			_graphics = new AppD2DContext(_mainForm.Handle, _mainForm.ClientSize.Width, _mainForm.ClientSize.Height);
			_mouseApp = new AppMouseEvent(_mainForm);

			_bitmap = AppUtil.LoadBitmap(_graphics.RenderTarget, Path.Combine(AppContext.BaseDirectory, "resource", "res_checker.png"));
		}

		public virtual void Dispose()
		{
			_bitmap?.Dispose();
			_mouseApp.Dispose();
			_graphics.Dispose();
			_mainForm.Dispose();
			AppUtil.Release();
		}

		public void Run()
		{
			try
			{
				this._stopwatch = Stopwatch.StartNew();
				RenderLoop.Run(_mainForm, MainRender);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void MainRender()
		{
			Update2D();
			Render2D();
		}

		private void Update2D()
		{
			double elapsed = _stopwatch.Elapsed.TotalSeconds;
			_clearColor = new(
				red: (float)(Math.Sin(elapsed) * 0.5 + 0.5),
				green: (float)(Math.Sin(elapsed + Math.PI / 2.0) * 0.5 + 0.5),
				blue: (float)(Math.Sin(elapsed + Math.PI) * 0.5 + 0.5),
				alpha: 1.0f
			);

			Update(); // 파생 클래스의 Update 호출
		}

		protected void Render2D()
		{
			var renderTarget = _graphics.RenderTarget;
			renderTarget.BeginDraw();
			renderTarget.Clear(_clearColor);

			if (_bitmap != null)
			{
				renderTarget.DrawBitmap(_bitmap, 1.0f, BitmapInterpolationMode.Linear);
				Rect destination = new Rect(400, 300, 300, 200);
				Rect sourceRect = new Rect(200, 100, 300, 200);
				renderTarget.DrawBitmap(_bitmap, destination, 1.0f, BitmapInterpolationMode.Linear, sourceRect);
			}

			Render(); // 파생 클래스의 Render 호출 (세미콜론 누락 수정됨)

			renderTarget.EndDraw();
		}

		virtual protected void Update() { }
		virtual protected void Render() { }
	}
}