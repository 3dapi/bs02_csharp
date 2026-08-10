using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;
using Vortice.WinForms;

abstract class AppBase : IDisposable
{
	public static AppBase? Instance { get; private set; }

	public ID2D1HwndRenderTarget RenderTarget => _graphics.RenderTarget;
	public IDWriteFactory DWriteFactory => _graphics.DWriteFactory;

	protected AppMouseEvent Mouse => _mouseApp;
	protected double DeltaTime { get; private set; }
	protected double TotalTime { get; private set; }
	protected Color4 ClearColor { get; set; } = new(0.0f, 0.0f, 0.0f, 1.0f);

	private readonly RenderForm _mainForm;
	private readonly AppD2DContext _graphics;
	private readonly AppMouseEvent _mouseApp;
	private readonly Stopwatch _stopwatch = new();

	private double _previousTime;

	private bool _isFullscreen = false;
	private FormBorderStyle _originalFormBorderStyle = FormBorderStyle.Sizable;
	private Rectangle _originalWindowBounds;

	protected AppBase()
	{
		if (Instance != null)
		{
			throw new InvalidOperationException("AppBase instance already exists.");
		}
		Instance = this;
		_mainForm = new RenderForm
		{
			Text = AppGlobal.GameName,
			ClientSize = AppGlobal.ScreenSize
		};
		AppD2DContext? graphics = null;
		AppMouseEvent? mouse = null;
		try
		{
			graphics = new AppD2DContext(_mainForm.Handle, _mainForm.ClientSize.Width, _mainForm.ClientSize.Height);
			mouse = new AppMouseEvent(_mainForm);
			_graphics = graphics;
			_mouseApp = mouse;
			_mainForm.Resize += MainFormResize;
			_mainForm.KeyDown += FormKeyDown;
		}
		catch
		{
			mouse?.Dispose();
			graphics?.Dispose();
			_mainForm.Dispose();
			throw;
		}
	}

	public void Run()
	{
		try
		{
			Initialize();
			DeltaTime = 0.0;
			TotalTime = 0.0;
			_previousTime = 0.0;
			_stopwatch.Restart();
			RenderLoop.Run(_mainForm, MainRender);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		_stopwatch.Stop();
	}

	private void MainRender()
	{
		UpdateTime();
		if (_mainForm.ClientSize.Width <= 0 || _mainForm.ClientSize.Height <= 0)
		{
			_mouseApp.EndFrame();
			return;
		}
		Update2D();
		Render2D();
		_mouseApp.EndFrame();
	}

	private void UpdateTime()
	{
		double currentTime = _stopwatch.Elapsed.TotalSeconds;
		DeltaTime = currentTime - _previousTime;
		TotalTime = currentTime;
		_previousTime = currentTime;
	}

	private void Update2D()
	{
		Update();
	}

	private void Render2D()
	{
		ID2D1HwndRenderTarget renderTarget = _graphics.RenderTarget;
		renderTarget.Transform = System.Numerics.Matrix3x2.CreateScale(ScreenScaleX, ScreenScaleY);
		renderTarget.BeginDraw();
		renderTarget.Clear(ClearColor);

		Render();

		renderTarget.EndDraw();
	}

	private void MainFormResize(object? sender, EventArgs e)
	{
		if (_mainForm.ClientSize.Width <= 0 || _mainForm.ClientSize.Height <= 0)
		{
			return;
		}
		_graphics.Resize(_mainForm.ClientSize.Width, _mainForm.ClientSize.Height);
	}

	private void FormKeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Alt && e.KeyCode == Keys.Enter)
		{
			ToggleFullscreen();
			e.Handled = true;
		}
	}

	protected abstract void Initialize();
	protected abstract void Update();
	protected abstract void Render();

	public virtual void Dispose()
	{
		_stopwatch.Stop();
		_mainForm.Resize -= MainFormResize;
		_mainForm.KeyDown -= FormKeyDown;
		_mouseApp.Dispose();
		_graphics.Dispose();
		_mainForm.Dispose();
		AppUtil.Release();
		Instance = null;
	}

	public static float ScreenScaleX
	{
		get
		{
			var size = Instance?._mainForm.ClientSize
				?? throw new InvalidOperationException("AppBase instance is not initialized.");
			return (float)size.Width / AppGlobal.ScreenSize.Width;
		}
	}

	public static float ScreenScaleY
	{
		get
		{
			var size = Instance?._mainForm.ClientSize
				?? throw new InvalidOperationException("AppBase instance is not initialized.");
			return (float)size.Height / AppGlobal.ScreenSize.Height;
		}
	}

	public void ToggleFullscreen()
	{
		if (!_isFullscreen)
		{
			_originalFormBorderStyle = _mainForm.FormBorderStyle;
			_originalWindowBounds = _mainForm.Bounds;
			_mainForm.FormBorderStyle = FormBorderStyle.None;
			_mainForm.WindowState = FormWindowState.Maximized;

			_isFullscreen = true;
		}
		else
		{
			_mainForm.FormBorderStyle = _originalFormBorderStyle;
			_mainForm.WindowState = FormWindowState.Normal;
			_mainForm.Bounds = _originalWindowBounds;

			_isFullscreen = false;
		}
	}
}
