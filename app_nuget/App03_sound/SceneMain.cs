using System.Windows.Forms;
using Vortice.Mathematics;

namespace AppGlc
{
	class SceneMain : IDisposable
	{
		private G2Font? _systemFont;
		private G2Font? _cursorFont;
		private string _mouseInfoText = string.Empty;
		private string _cursorText = string.Empty;
		private G2AudioSound? _soundEffect;
		private G2AudioMp3? _backgroundMusic;
		// -------------------------------------------------------------------------------------------------------------------------------------------------------------
		// -------------------------------------------------------------------------------------------------------------------------------------------------------------
		public void Initialize()
		{
			_systemFont = new G2Font("Arial", 32);
			_cursorFont = new G2Font("Arial", 18);
			_soundEffect = new G2AudioSound("resource/audio/effect/move3.wav");
			_backgroundMusic = new G2AudioMp3("resource/audio/bgm/background.mp3");
			_backgroundMusic.Play(true);
		}
		public void Update()
		{
			var Input = G2AppBase.Instance?.Input ?? throw new InvalidOperationException("G2AppBase instance is not initialized.");
			var mousePos = Input.MousePosition;
			_mouseInfoText = $"실시간 마우스 좌표: (X: {(int)mousePos.X}, Y: {(int)mousePos.Y})";
			_cursorText = $"({(int)mousePos.X}, {(int)mousePos.Y})";

			// 마우스 좌클릭 시 효과음 재생 체크
			if (Input.IsButtonDown(MouseButtons.Left)) // 또는 윈도우 폼 이벤트/입력 상태에 맞게 체크
			{
				_soundEffect?.Play();
			}
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
			_backgroundMusic?.Dispose();
			_soundEffect?.Dispose();
			_systemFont?.Dispose();
			_cursorFont?.Dispose();
		}
	}
}
