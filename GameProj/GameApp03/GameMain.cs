using Vortice.Mathematics;

namespace glc
{
	internal class GameMain : AppBase
	{
		private AppFont _systemFont;
		private AppFont _cursorFont;
		private string _mouseInfoText = string.Empty;
		private string _cursorText = string.Empty;

		public GameMain() : base()
		{
			_systemFont = new AppFont("고도 B", 32 /* 폰트 크기 */);
			// 마우스 따라다니는 용도의 작은 폰트 추가 (예: 크기 18)
			_cursorFont = new AppFont("고도 B", 18);
		}

		protected override void Update()
		{
			// 게임 로직 업데이트: 매 프레임 실시간 마우스 좌표 획득
			// _mouseApp은 Application 클래스에서 protected로 제공되므로 직접 접근 가능합니다.
			var mousePos = _mouseApp.MousePosition;

			// 출력할 문자열 포맷팅
			_mouseInfoText = $"실시간 마우스 좌표: (X: {mousePos.X}, Y: {mousePos.Y})";

			// 커서 근처에 띄울 텍스트 갱신
			_cursorText = $"({mousePos.X}, {mousePos.Y})";
		}

		protected override void Render()
		{
			// 렌더링 파이프라인: 포맷팅된 텍스트를 화면에 출력
			_systemFont.DrawText(_mouseInfoText, new Rect(20, 20, 600, 100), new Color4(0.0f, 1.0f, 1.0f, 1.5f));

			// 2. 마우스 커서 위치를 따라다니는 텍스트 출력
			// 마우스 좌표(mousePos)를 기준으로 살짝 우측 하단(예: +15, +15)에 텍스트 박스 영역을 잡습니다.
			var mousePos = _mouseApp.MousePosition;
			Rect cursorRect = new Rect(mousePos.X + 15, mousePos.Y + 15, 200, 50);

			_cursorFont.DrawText(_cursorText, cursorRect, new Color4(1.0f, 1.0f, 0.0f, 1.0f)); // 노란색
		}

		// 객체 소멸 시 Native 리소스 해제
		public override void Dispose()
		{
			_systemFont?.Dispose();
			_cursorFont?.Dispose();
			AppFont.ClearFontCache();
			base.Dispose(); // 부모 클래스의 Dispose 호출하여 나머지 리소스 해제
		}
	}
}