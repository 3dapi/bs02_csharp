using Vortice.Mathematics;

namespace AppGlc
{
	class SceneMain : IDisposable
	{
		private G2Font? _systemFont;
		private G2Font? _cursorFont;
		private string _mouseInfoText = string.Empty;
		private string _cursorText = string.Empty;
		// -------------------------------------------------------------------------------------------------------------------------------------------------------------
		// -------------------------------------------------------------------------------------------------------------------------------------------------------------
		public void Initialize()
		{
			_systemFont = new G2Font("Arial", 32);
			_cursorFont = new G2Font("Arial", 18);
		}
		public void Update()
		{
			var mousePos = G2AppBase.Instance?.Input?.MousePosition
								?? throw new InvalidOperationException("G2AppBase instance is not initialized.");
			_mouseInfoText = $"실시간 마우스 좌표: (X: {(int)mousePos.X}, Y: {(int)mousePos.Y})";
			_cursorText = $"({(int)mousePos.X}, {(int)mousePos.Y})";
		}
		public void Render()
		{
			_systemFont?.DrawText(_mouseInfoText, new Rect(20, 20, 600, 100), new Color4(0.0f, 1.0f, 1.0f, 1.0f));

			var mousePos = G2AppBase.Instance?.Input?.MousePosition
								?? throw new InvalidOperationException("G2AppBase instance is not initialized.");

			Rect cursorRect = new(mousePos.X + 5, mousePos.Y + 5, 200, 50);
			_cursorFont?.DrawText(_cursorText, cursorRect, new Color4(1.0f, 1.0f, 0.0f, 1.0f));
		}

		public void Dispose()
		{
			_systemFont?.Dispose();
			_cursorFont?.Dispose();
		}
	}
}
