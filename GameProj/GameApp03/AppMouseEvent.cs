using System;
using System.Drawing;
using System.Windows.Forms;

namespace glc
{
	internal class AppMouseEvent : IDisposable
	{
		private readonly Form _targetForm;

		public AppMouseEvent(Form form)
		{
			_targetForm = form ?? throw new ArgumentNullException(nameof(form));

			_targetForm.MouseDown += MouseDown;
			_targetForm.MouseUp += MouseUp;
			_targetForm.MouseWheel += MouseWheel;
			_targetForm.MouseMove += MouseMove;
			_targetForm.MouseEnter += MouseEnter;
			_targetForm.MouseLeave += MouseLeave;
		}
		public void Dispose()
		{
			_targetForm.MouseDown -= MouseDown;
			_targetForm.MouseUp -= MouseUp;
			_targetForm.MouseWheel -= MouseWheel;
			_targetForm.MouseMove -= MouseMove;
			_targetForm.MouseEnter -= MouseEnter;
			_targetForm.MouseLeave -= MouseLeave;
		}


		// 실시간 마우스 좌표 프로퍼티
		public Point MousePosition => _targetForm.PointToClient(Cursor.Position); // Form 내부 상대 좌표
		public Point ScreenMousePosition => Cursor.Position;                      // Screen 전체 절대 좌표

		public void MouseDown(object? sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Console.WriteLine($"MouseDown: Left mouse at ({e.X}, {e.Y})");
			}
			else if (e.Button == MouseButtons.Right)
			{
				Console.WriteLine($"MouseDown: Right mouse at ({e.X}, {e.Y})");
			}
			else if (e.Button == MouseButtons.Middle)
			{
				Console.WriteLine($"MouseDown: Middle (Wheel) mouse at ({e.X}, {e.Y})");
			}
		}

		public void MouseUp(object? sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Console.WriteLine($"MouseUp: Left mouse at ({e.X}, {e.Y})");
			}
			else if (e.Button == MouseButtons.Right)
			{
				Console.WriteLine($"MouseUp: Right mouse at ({e.X}, {e.Y})");
			}
			else if (e.Button == MouseButtons.Middle)
			{
				Console.WriteLine($"MouseUp: Middle (Wheel) mouse at ({e.X}, {e.Y})");
			}
		}

		public void MouseWheel(object? sender, MouseEventArgs e)
		{
			Console.WriteLine($"MouseWheel: Delta={e.Delta} at ({e.X}, {e.Y})");
		}

		public void MouseMove(object? sender, MouseEventArgs e)
		{
			var pos = this.MousePosition;

			if (e.Button == MouseButtons.None)
			{
				Console.WriteLine($"MouseMove: EventPos=({e.X}, {e.Y}), RealtimePos=({pos.X}, {pos.Y})");
			}
			else if (e.Button == MouseButtons.Middle)
			{
				Console.WriteLine($"MouseMove: Dragging Middle mouse at EventPos=({e.X}, {e.Y}), RealtimePos=({pos.X}, {pos.Y})");
			}
		}

		public void MouseEnter(object? sender, EventArgs e)
		{
			Console.WriteLine("MouseEnter: Mouse entered the form.");
		}

		public void MouseLeave(object? sender, EventArgs e)
		{
			Console.WriteLine("MouseLeave: Mouse left the form.");
		}
	}
}