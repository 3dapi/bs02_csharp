// -------------------------------------------------------------------------------------------------------------------------------------------------------------
// Author: 3dapi (https://github.com/3dapi)
// -------------------------------------------------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Vortice.Mathematics;

class SceneMain : IDisposable
{
	private const int GameMaxStage = 3;
	private const int GameTime = 200;
	private const int ImageWidth = 400;

	private enum GamePhase
	{
		Init,
		Play,
		End
	}

	private sealed class GameButton
	{
		public Rect NormalRect { get; }
		public Rect OverRect { get; }
		public PointF Position { get; }

		public GameButton(Rect normalRect, Rect overRect, PointF position)
		{
			NormalRect = normalRect;
			OverRect = overRect;
			Position = position;
		}

		public bool IsMouseOver(float mouseX, float mouseY)
		{
			float width = NormalRect.Right - NormalRect.Left;
			float height = NormalRect.Bottom - NormalRect.Top;

			return
				mouseX >= Position.X &&
				mouseX < Position.X + width &&
				mouseY >= Position.Y &&
				mouseY < Position.Y + height;
		}
	}

	private sealed class GameStage : IDisposable
	{
		public G2Texture TextureLeft { get; }
		public G2Texture TextureRight { get; }
		public RectangleF[] CheckRects { get; }

		public GameStage(string leftFile, string rightFile, RectangleF[] checkRects)
		{
			TextureLeft = new G2Texture(leftFile);
			TextureRight = new G2Texture(rightFile);
			CheckRects = checkRects;
		}

		public void Dispose()
		{
			TextureRight.Dispose();
			TextureLeft.Dispose();
		}
	}

	private readonly RectangleF[][] _checkRects =
	[
		[
			new RectangleF(  0, 112, 32,  32),
			new RectangleF(224,  62, 32,  32),
			new RectangleF( 91,   2, 37, 102),
			new RectangleF(280, 212, 30,  30),
			new RectangleF(194, 267, 14,  67),
			new RectangleF(107, 165, 17,  17),
			new RectangleF(  0, 306, 40,  40),
			new RectangleF( 50, 261, 20,  24),
			new RectangleF(243, 336, 28,  20),
			new RectangleF(338, 305, 18,  18),
		],
		[
			new RectangleF( 27, 122, 116, 43),
			new RectangleF(228,  74,  20, 20),
			new RectangleF(374, 110,  20, 20),
			new RectangleF(204, 117,  37, 23),
			new RectangleF( 11, 181,  32, 52),
			new RectangleF( 50, 330,  42, 28),
			new RectangleF(162, 262,  43, 23),
			new RectangleF(231, 314,  41, 12),
			new RectangleF(323, 252,  16, 14),
			new RectangleF(364, 215,  21, 19),
		],
		[
			new RectangleF( 50,  38, 24, 28),
			new RectangleF(378,  10, 20, 14),
			new RectangleF( 18,  96, 52, 98),
			new RectangleF( 83, 176, 23, 14),
			new RectangleF( 63, 254, 24, 24),
			new RectangleF( 94, 300, 20, 27),
			new RectangleF(  1, 332, 24, 42),
			new RectangleF(222, 238, 15, 39),
			new RectangleF(240, 347, 34, 26),
			new RectangleF(353, 332, 28, 18),
		],
	];

	private readonly string[][] _textureNames =
	[
		["resource/texture/smurf11.png", "resource/texture/smurf12.png"],
		["resource/texture/smurf21.png", "resource/texture/smurf22.png"],
		["resource/texture/smurf31.png", "resource/texture/smurf32.png"],
	];

	private readonly GameButton _buttonNext = new(
		new Rect(128, 0, 90, 86),
		new Rect(218, 0, 90, 86),
		new PointF(550.0f, 385.0f));

	private readonly GameButton _buttonExit = new(
		new Rect(312, 0, 96, 94),
		new Rect(408, 0, 96, 94),
		new PointF(670.0f, 385.0f));

	private readonly List<int> _checked = [];
	private readonly GameStage?[] _gameStages = new GameStage?[GameMaxStage];
	private readonly Stopwatch _timer = new();

	private GamePhase _gamePhase = GamePhase.Init;
	private bool _isVictory;
	private int _currentStage;
	private int _timeRemain = GameTime;

	private G2Texture? _textureUi;
	private G2Font? _fontMain;
	private G2Font? _fontTimer;

	public void Initialize()
	{
		var app = G2AppBase.Instance ?? throw new InvalidOperationException("G2AppBase instance is not initialized.");
		app.ClearColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
		_textureUi = new G2Texture("resource/texture/ui.png");
		_fontMain = new G2Font("Pretendard ExtraBold", 64);
		_fontTimer = new G2Font("Arial", 32);
		for(int i = 0; i < GameMaxStage; ++i)
		{
			_gameStages[i] = new GameStage(_textureNames[i][0], _textureNames[i][1], _checkRects[i]);
		}
		_currentStage = 0;
		StartStage();
	}

	public void Update()
	{
		var input = G2AppBase.Instance?.Input ?? throw new InvalidOperationException("G2AppBase instance is not initialized.");
		PointF mouse = input.MousePosition;
		if(input.IsButtonDown(MouseButtons.Left))
		{
			if(_buttonNext.IsMouseOver(mouse.X, mouse.Y) && _isVictory)
			{
				NextStage();
				return;
			}
			if(_buttonExit.IsMouseOver(mouse.X, mouse.Y))
			{
				ExitGame();
				return;
			}
		}
		if(_gamePhase != GamePhase.Play)
		{
			return;
		}
		if(input.IsButtonDown(MouseButtons.Left))
		{
			CheckAnswer(mouse.X, mouse.Y);
		}
		UpdateTimer();
		CheckVictory();
	}

	public void Render()
	{
		DrawBackground();
		DrawChecked();
		//DrawCheckList();
		DrawGameUi();
		if(_gamePhase == GamePhase.End)
		{
			DrawGameResult();
		}
	}

	private GameStage GetCurrentStage()
	{
		return _gameStages[_currentStage] ?? throw new InvalidOperationException("Game stage is not initialized.");
	}

	private void StartStage()
	{
		_checked.Clear();
		_timeRemain = GameTime;
		_timer.Restart();
		_isVictory = false;
		_gamePhase = GamePhase.Play;
	}

	private void NextStage()
	{
		++_currentStage;
		if(_currentStage >= GameMaxStage)
		{
			_currentStage = 0;
		}
		StartStage();
	}

	private static void ExitGame()
	{
		G2AppBase.Instance?.Close();
	}

	private void AddCheck(int index)
	{
		if(_checked.Contains(index))
		{
			return;
		}
		_checked.Add(index);
	}

	private void CheckAnswer(float mouseX, float mouseY)
	{
		GameStage currentStage = GetCurrentStage();
		float imageX = mouseX;
		if(mouseX >= ImageWidth)
		{
			imageX -= ImageWidth;
		}
		for(int i = 0; i < currentStage.CheckRects.Length; ++i)
		{
			if(currentStage.CheckRects[i].Contains(imageX, mouseY))
			{
				AddCheck(i);
				break;
			}
		}
	}

	private void UpdateTimer()
	{
		if(_timer.ElapsedMilliseconds < 1000)
		{
			return;
		}
		long seconds = _timer.ElapsedMilliseconds / 1000;
		_timer.Restart();
		_timeRemain -= (int)seconds;
		if(_timeRemain <= 0)
		{
			_timeRemain = 0;
			_isVictory = false;
			_gamePhase = GamePhase.End;
			_timer.Stop();
		}
	}

	private void CheckVictory()
	{
		GameStage currentStage = GetCurrentStage();
		if(_checked.Count == 1)///currentStage.CheckRects.Length)
		{
			_isVictory = true;
			_gamePhase = GamePhase.End;
			_timer.Stop();
		}
	}

	private void DrawBackground()
	{
		GameStage currentStage = GetCurrentStage();
		currentStage.TextureLeft.Draw(0.0f, 0.0f);
		currentStage.TextureRight.Draw(ImageWidth, 0.0f);
	}

	private void DrawChecked()
	{
		GameStage currentStage = GetCurrentStage();
		Rect source = new(0, 0, 64, 64);
		foreach(int index in _checked)
		{
			RectangleF rect = currentStage.CheckRects[index];
			float x = rect.X + rect.Width * 0.5f - 32.0f;
			float y = rect.Y + rect.Height * 0.5f - 32.0f;
			DrawUiImage(source, x, y);
			DrawUiImage(source, x + ImageWidth, y);
		}
	}

	private void DrawCheckList()
	{
		GameStage currentStage = GetCurrentStage();
		Rect source = new(0, 0, 64, 64);
		foreach (var rect in currentStage.CheckRects)
		{
			float x = rect.X + rect.Width * 0.5f - 32.0f;
			float y = rect.Y + rect.Height * 0.5f - 32.0f;
			DrawUiImage(source, x, y);
			DrawUiImage(source, x + ImageWidth, y);
		}
	}

	private void DrawButton(GameButton button, float mouseX, float mouseY)
	{
		Rect source = button.NormalRect;
		if(button.IsMouseOver(mouseX, mouseY))
		{
			source = button.OverRect;
		}
		DrawUiImage(source, button.Position.X, button.Position.Y);
	}

	private void DrawUiImage(Rect source, float x, float y)
	{
		if(_textureUi == null)
		{
			return;
		}
		float width = source.Right - source.Left;
		float height = source.Bottom - source.Top;
		Rect destination = new((int)x, (int)y, (int)(source.Width), (int)(source.Height));
		_textureUi.Draw(destination, source);
	}

	private void DrawGameUi()
	{
		var input = G2AppBase.Instance?.Input ?? throw new InvalidOperationException("G2AppBase instance is not initialized.");

		PointF mouse = input.MousePosition;
		if(_isVictory)
		{
			DrawButton(_buttonNext, mouse.X, mouse.Y);
		}
		DrawButton(_buttonExit, mouse.X, mouse.Y);
		_fontTimer?.DrawText($"Time: {_timeRemain}", new Rect(10, 440, 300, 470), new Color4(1.0f, 1.0f, 1.0f, 1.0f));
	}

	private void DrawGameResult()
	{
		if(_isVictory)
		{
			_fontMain?.DrawText("You Win!!!", new Rect(250, 200, 800, 400), new Color4(1.0f, 0.0f, 1.0f, 1.0f));
		}
		else
		{
			_fontMain?.DrawText("Game Over", new Rect(250, 200, 800, 400), new Color4(1.0f, 0.0f, 0.0f, 1.0f));
		}
	}

	public void Dispose()
	{
		_timer.Stop();
		_fontTimer?.Dispose();
		_fontTimer = null;
		_fontMain?.Dispose();
		_fontMain = null;
		for(int i = 0; i < _gameStages.Length; ++i)
		{
			_gameStages[i]?.Dispose();
			_gameStages[i] = null;
		}
		_textureUi?.Dispose();
		_textureUi = null;
	}
}
