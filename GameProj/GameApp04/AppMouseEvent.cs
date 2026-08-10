using System;
using System.Drawing;
using System.Windows.Forms;

class AppMouseEvent : IDisposable
{
	private readonly Form _targetForm;

	private bool _leftButtonDown;
	private bool _rightButtonDown;
	private bool _middleButtonDown;

	private bool _leftButtonPressed;
	private bool _rightButtonPressed;
	private bool _middleButtonPressed;

	private bool _leftButtonReleased;
	private bool _rightButtonReleased;
	private bool _middleButtonReleased;

	public PointF MousePosition
	{
		get
		{
			Point clientPos = _targetForm.PointToClient(Cursor.Position);
			return new PointF((int)(clientPos.X / AppBase.ScreenScaleX), (int)(clientPos.Y / AppBase.ScreenScaleY));
		}
	}

	public PointF ScreenMousePosition => Cursor.Position;

	public bool IsInside { get; private set; }

	public bool LeftButtonDown => _leftButtonDown;
	public bool RightButtonDown => _rightButtonDown;
	public bool MiddleButtonDown => _middleButtonDown;

	public bool LeftButtonPressed => _leftButtonPressed;
	public bool RightButtonPressed => _rightButtonPressed;
	public bool MiddleButtonPressed => _middleButtonPressed;

	public bool LeftButtonReleased => _leftButtonReleased;
	public bool RightButtonReleased => _rightButtonReleased;
	public bool MiddleButtonReleased => _middleButtonReleased;

	public int WheelDelta { get; private set; }

	public AppMouseEvent(Form form)
	{
		_targetForm = form?? throw new ArgumentNullException(nameof(form));
		_targetForm.MouseDown += MouseDown;
		_targetForm.MouseUp += MouseUp;
		_targetForm.MouseWheel += MouseWheel;
		_targetForm.MouseEnter += MouseEnter;
		_targetForm.MouseLeave += MouseLeave;
	}

	private void MouseDown(object? sender, MouseEventArgs e)
	{
		switch (e.Button)
		{
			case MouseButtons.Left:
				if (!_leftButtonDown)
				{
					_leftButtonPressed = true;
				}

				_leftButtonDown = true;
				break;

			case MouseButtons.Right:
				if (!_rightButtonDown)
				{
					_rightButtonPressed = true;
				}

				_rightButtonDown = true;
				break;

			case MouseButtons.Middle:
				if (!_middleButtonDown)
				{
					_middleButtonPressed = true;
				}

				_middleButtonDown = true;
				break;
		}
	}

	private void MouseUp(object? sender, MouseEventArgs e)
	{
		switch (e.Button)
		{
			case MouseButtons.Left:
				_leftButtonDown = false;
				_leftButtonReleased = true;
				break;

			case MouseButtons.Right:
				_rightButtonDown = false;
				_rightButtonReleased = true;
				break;

			case MouseButtons.Middle:
				_middleButtonDown = false;
				_middleButtonReleased = true;
				break;
		}
	}

	private void MouseWheel(object? sender, MouseEventArgs e)
	{
		WheelDelta += e.Delta;
	}

	private void MouseEnter(object? sender, EventArgs e)
	{
		IsInside = true;
	}

	private void MouseLeave(object? sender, EventArgs e)
	{
		IsInside = false;
	}

	internal void EndFrame()
	{
		_leftButtonPressed = false;
		_rightButtonPressed = false;
		_middleButtonPressed = false;

		_leftButtonReleased = false;
		_rightButtonReleased = false;
		_middleButtonReleased = false;

		WheelDelta = 0;
	}

	public void Dispose()
	{
		_targetForm.MouseDown -= MouseDown;
		_targetForm.MouseUp -= MouseUp;
		_targetForm.MouseWheel -= MouseWheel;
		_targetForm.MouseEnter -= MouseEnter;
		_targetForm.MouseLeave -= MouseLeave;
	}
}
