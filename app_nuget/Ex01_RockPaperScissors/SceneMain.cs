// -------------------------------------------------------------------------------------------------------------------------------------------------------------
// Author: 3dapi (https://github.com/3dapi)
// -------------------------------------------------------------------------------------------------------------------------------------------------------------

using System.Drawing;
using System.Windows.Forms;
using Vortice.Mathematics;

class SceneMain : IDisposable
{
	private const int GameWinCount = 3;
	private const int HandMax = 3;

	private enum GameState
	{
		Ready,
		Select,
		Result,
		GameOver
	}

	private enum Hand
	{
		Rock,
		Scissor,
		Paper
	}

	private sealed class GameButton : IDisposable
	{
		private readonly G2Texture _textureNormal;
		private readonly G2Texture _textureOver;

		public RectangleF Rect { get; }

		public GameButton(
			string normalFile,
			string overFile,
			RectangleF rect)
		{
			_textureNormal = new G2Texture(normalFile);
			_textureOver = new G2Texture(overFile);
			Rect = rect;
		}

		public bool IsMouseOver(float mouseX, float mouseY)
		{
			return Rect.Contains(mouseX, mouseY);
		}

		public void Draw(float mouseX, float mouseY)
		{
			G2Texture texture = _textureNormal;

			if(IsMouseOver(mouseX, mouseY))
			{
				texture = _textureOver;
			}

			texture.Draw(Rect.X, Rect.Y);
		}

		public void Dispose()
		{
			_textureOver.Dispose();
			_textureNormal.Dispose();
		}
	}

	private sealed class HandData : IDisposable
	{
		public G2Texture TextureLeft { get; }
		public G2Texture TextureRight { get; }
		public GameButton Button { get; }

		public HandData(
			string leftFile,
			string rightFile,
			string buttonFile,
			string buttonOverFile,
			RectangleF buttonRect)
		{
			TextureLeft = new G2Texture(leftFile);
			TextureRight = new G2Texture(rightFile);
			Button = new GameButton(
				buttonFile,
				buttonOverFile,
				buttonRect);
		}

		public void Dispose()
		{
			Button.Dispose();
			TextureRight.Dispose();
			TextureLeft.Dispose();
		}
	}

	private GameState _gameState = GameState.Ready;

	private int _userWin;
	private int _comWin;

	private Hand _handUser = Hand.Rock;
	private Hand _handCom = Hand.Rock;
	private bool _hasResult;

	private G2Texture? _textureBg;
	private G2Texture? _textureScore;
	private G2Texture? _textureStart;

	private readonly HandData?[] _hands = new HandData?[HandMax];

	private GameButton? _buttonContinue;
	private GameButton? _buttonEnd;

	private G2Font? _fontScore;
	private G2Font? _fontMsg;

	private readonly RectangleF _rectStart = new(256, 200, 128, 64);

	public void Initialize()
	{
		var app = G2AppBase.Instance
			?? throw new InvalidOperationException(
				"G2AppBase instance is not initialized.");

		app.ClearColor = new Color4(
			0.0f,
			0.4f,
			0.6f,
			1.0f);

		_textureBg = new G2Texture("resource/tex_ui/ui_bg.png");
		_textureScore = new G2Texture("resource/tex_ui/ui_score.png");
		_textureStart = new G2Texture("resource/tex_ui/ui_start.png");

		_hands[(int)Hand.Rock] = new HandData(
			"resource/tex_play/img_l_r.png",
			"resource/tex_play/img_r_r.png",
			"resource/tex_ui/ui_rock.png",
			"resource/tex_ui/ui_rock_o.png",
			new RectangleF(20, 330, 128, 128));

		_hands[(int)Hand.Scissor] = new HandData(
			"resource/tex_play/img_l_s.png",
			"resource/tex_play/img_r_s.png",
			"resource/tex_ui/ui_scissor.png",
			"resource/tex_ui/ui_scissor_o.png",
			new RectangleF(150, 330, 128, 128));

		_hands[(int)Hand.Paper] = new HandData(
			"resource/tex_play/img_l_p.png",
			"resource/tex_play/img_r_p.png",
			"resource/tex_ui/ui_paper.png",
			"resource/tex_ui/ui_paper_o.png",
			new RectangleF(280, 330, 128, 128));

		_buttonContinue = new GameButton(
			"resource/tex_ui/ui_continue.png",
			"resource/tex_ui/ui_continue_o.png",
			new RectangleF(455, 365, 128, 64));

		_buttonEnd = new GameButton(
			"resource/tex_ui/ui_end.png",
			"resource/tex_ui/ui_end_o.png",
			new RectangleF(565, 400, 64, 64));

		_fontScore = new G2Font("Arial", 40);
		_fontMsg = new G2Font("Arial", 24);

		_gameState = GameState.Ready;
	}

	public void Update()
	{
		var input = G2AppBase.Instance?.Input
			?? throw new InvalidOperationException(
				"G2AppBase instance is not initialized.");

		if(!input.IsButtonDown(MouseButtons.Left))
		{
			return;
		}

		PointF mouse = input.MousePosition;
		float mouseX = mouse.X;
		float mouseY = mouse.Y;

		if(_gameState == GameState.Ready)
		{
			if(_rectStart.Contains(mouseX, mouseY))
			{
				ResetGame();
			}

			return;
		}

		if(_gameState == GameState.Select)
		{
			for(int i = 0; i < HandMax; ++i)
			{
				HandData hand = GetHandData((Hand)i);

				if(hand.Button.IsMouseOver(mouseX, mouseY))
				{
					PlayHand((Hand)i);
					return;
				}
			}

			if(_buttonEnd?.IsMouseOver(mouseX, mouseY) == true)
			{
				ExitGame();
				return;
			}
		}

		if(_gameState == GameState.Result)
		{
			if(_buttonContinue?.IsMouseOver(mouseX, mouseY) == true)
			{
				ContinueGame();
				return;
			}

			if(_buttonEnd?.IsMouseOver(mouseX, mouseY) == true)
			{
				ExitGame();
				return;
			}
		}

		if(_gameState == GameState.GameOver)
		{
			if(_buttonContinue?.IsMouseOver(mouseX, mouseY) == true)
			{
				ResetGame();
				return;
			}

			if(_buttonEnd?.IsMouseOver(mouseX, mouseY) == true)
			{
				ExitGame();
			}
		}
	}

	public void Render()
	{
		var input = G2AppBase.Instance?.Input
			?? throw new InvalidOperationException(
				"G2AppBase instance is not initialized.");

		PointF mouse = input.MousePosition;

		DrawBackground();
		DrawScore();

		if(_gameState == GameState.Ready)
		{
			DrawReady();
			return;
		}

		DrawHands();

		if(_gameState == GameState.Select)
		{
			DrawSelectButtons(mouse.X, mouse.Y);
		}
		else if(_gameState == GameState.Result)
		{
			DrawResultMessage();
			_buttonContinue?.Draw(mouse.X, mouse.Y);
			_buttonEnd?.Draw(mouse.X, mouse.Y);
		}
		else if(_gameState == GameState.GameOver)
		{
			DrawResultMessage();
			DrawGameOverMessage();
			_buttonContinue?.Draw(mouse.X, mouse.Y);
			_buttonEnd?.Draw(mouse.X, mouse.Y);
		}
	}

	private static bool IsUserWin(Hand user, Hand com)
	{
		if(user == Hand.Rock && com == Hand.Scissor)
		{
			return true;
		}

		if(user == Hand.Scissor && com == Hand.Paper)
		{
			return true;
		}

		if(user == Hand.Paper && com == Hand.Rock)
		{
			return true;
		}

		return false;
	}

	private static Hand GetRandomHand()
	{
		return (Hand)Random.Shared.Next(HandMax);
	}

	private void ResetGame()
	{
		_userWin = 0;
		_comWin = 0;

		_handUser = Hand.Rock;
		_handCom = GetRandomHand();
		_hasResult = false;

		_gameState = GameState.Select;
	}

	private void PlayHand(Hand hand)
	{
		_handUser = hand;
		_hasResult = true;

		if(_handUser != _handCom)
		{
			if(IsUserWin(_handUser, _handCom))
			{
				++_userWin;
			}
			else
			{
				++_comWin;
			}
		}

		if(_userWin >= GameWinCount ||
			_comWin >= GameWinCount)
		{
			_gameState = GameState.GameOver;
		}
		else
		{
			_gameState = GameState.Result;
		}
	}

	private void ContinueGame()
	{
		_handCom = GetRandomHand();
		_hasResult = false;
		_gameState = GameState.Select;
	}

	private static void ExitGame()
	{
		G2AppBase.Instance?.Close();
	}

	private HandData GetHandData(Hand hand)
	{
		return _hands[(int)hand]
			?? throw new InvalidOperationException(
				"Hand resource is not initialized.");
	}

	private void DrawBackground()
	{
		_textureBg?.Draw(0.0f, 0.0f);
		_textureScore?.Draw(10.0f, 10.0f);
	}

	private void DrawScore()
	{
		_fontScore?.DrawText(
			$"{_userWin} : {_comWin}  COM:{(int)_handCom}",
			new Rect(120, 5, 350, 55),
			new Color4(1.0f, 0.0f, 0.0f, 1.0f));
	}

	private void DrawHands()
	{
		if(!_hasResult)
		{
			return;
		}

		HandData user = GetHandData(_handUser);
		HandData com = GetHandData(_handCom);

		user.TextureLeft.Draw(20.0f, 60.0f);
		com.TextureRight.Draw(350.0f, 60.0f);
	}

	private void DrawSelectButtons(float mouseX, float mouseY)
	{
		for(int i = 0; i < HandMax; ++i)
		{
			GetHandData((Hand)i).Button.Draw(mouseX, mouseY);
		}

		_buttonEnd?.Draw(mouseX, mouseY);
	}

	private void DrawResultMessage()
	{
		if(!_hasResult)
		{
			return;
		}

		if(_handUser == _handCom)
		{
			_fontMsg?.DrawText(
				"Draw",
				new Rect(240, 290, 190, 40),
				new Color4(1.0f, 1.0f, 0.0f, 1.0f));

			return;
		}

		if(IsUserWin(_handUser, _handCom))
		{
			_fontMsg?.DrawText(
				"You Win",
				new Rect(240, 290, 190, 40),
				new Color4(0.0f, 1.0f, 0.0f, 1.0f));
		}
		else
		{
			_fontMsg?.DrawText(
				"You Lose",
				new Rect(240, 290, 190, 40),
				new Color4(1.0f, 0.0f, 0.0f, 1.0f));
		}
	}

	private void DrawReady()
	{
		_textureStart?.Draw(
			_rectStart.X,
			_rectStart.Y);
	}

	private void DrawGameOverMessage()
	{
		if(_userWin >= GameWinCount)
		{
			_fontScore?.DrawText(
				"YOU WIN!",
				new Rect(180, 270, 320, 55),
				new Color4(0.0f, 1.0f, 0.0f, 1.0f));
		}
		else
		{
			_fontScore?.DrawText(
				"YOU LOSE",
				new Rect(180, 270, 320, 55),
				new Color4(1.0f, 0.0f, 0.0f, 1.0f));
		}
	}

	public void Dispose()
	{
		_fontMsg?.Dispose();
		_fontMsg = null;

		_fontScore?.Dispose();
		_fontScore = null;

		_buttonEnd?.Dispose();
		_buttonEnd = null;

		_buttonContinue?.Dispose();
		_buttonContinue = null;

		for(int i = 0; i < _hands.Length; ++i)
		{
			_hands[i]?.Dispose();
			_hands[i] = null;
		}

		_textureStart?.Dispose();
		_textureStart = null;

		_textureScore?.Dispose();
		_textureScore = null;

		_textureBg?.Dispose();
		_textureBg = null;
	}
}
