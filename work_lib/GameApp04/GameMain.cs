using Vortice.Mathematics;

class GameMain : AppBase
{
	private AppFont? _systemFont;
	private AppFont? _cursorFont;
	private AppTexture? _checkerTexture;

	private string _mouseInfoText = string.Empty;
	private string _cursorText = string.Empty;

	protected override void Initialize()
	{
		_systemFont = new AppFont("Arial", 32);
		_cursorFont = new AppFont("Arial", 18);
		_checkerTexture = new AppTexture("resource/texture/res_checker.png");
	}

	protected override void Update()
	{
		double elapsed = TotalTime;

		ClearColor = new Color4(
			red: (float)(Math.Sin(elapsed) * 0.5 + 0.5),
			green: (float)(Math.Sin(elapsed + Math.PI / 2.0) * 0.5 + 0.5),
			blue: (float)(Math.Sin(elapsed + Math.PI) * 0.5 + 0.5),
			alpha: 1.0f);

		var mousePos = Mouse.MousePosition;
		_mouseInfoText = $"실시간 마우스 좌표: (X: {mousePos.X}, Y: {mousePos.Y})";
		_cursorText = $"({mousePos.X}, {mousePos.Y})";
	}

	protected override void Render()
	{
		_checkerTexture?.Draw();
		_checkerTexture?.Draw( new Rect(400, 300, 300, 200), new Rect(200, 100, 300, 200));
		_systemFont?.DrawText( _mouseInfoText, new Rect(20, 20, 600, 100), new Color4(0.0f, 1.0f, 1.0f, 1.0f));

		var mousePos = Mouse.MousePosition;
		Rect cursorRect = new(mousePos.X + 5, mousePos.Y + 5, 200, 50);
		_cursorFont?.DrawText(_cursorText, cursorRect, new Color4(1.0f, 1.0f, 0.0f, 1.0f));
	}

	public override void Dispose()
	{
		_checkerTexture?.Dispose();
		_cursorFont?.Dispose();
		_systemFont?.Dispose();

		base.Dispose();
	}
}
