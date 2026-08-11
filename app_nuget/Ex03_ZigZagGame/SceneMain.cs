// -------------------------------------------------------------------------------------------------------------------------------------------------------------
// Author: 3dapi (https://github.com/3dapi)
// -------------------------------------------------------------------------------------------------------------------------------------------------------------

using System.Windows.Forms;
using Vortice.Direct2D1;
using Vortice.Mathematics;

class SceneMain : IDisposable
{
	private const int RowCount = 3;
	private const int ColumnCount = 4;

	private const int ImageWidth = 640;
	private const int ImageHeight = 480;

	private const int ShuffleCount = 500;
	private const int Gap = 2;

	private enum MoveDirection
	{
		None,
		Left,
		Right,
		Up,
		Down
	}

	private readonly int[,] _map = new int[RowCount, ColumnCount];

	private readonly int _sourceBlockWidth = ImageWidth / ColumnCount;
	private readonly int _sourceBlockHeight = ImageHeight / RowCount;

	private int DestBlockWidth => GameGlobal.ScreenSize.Width / ColumnCount;
	private int DestBlockHeight => GameGlobal.ScreenSize.Height / RowCount;

	private readonly int _blankIndex = RowCount * ColumnCount - 1;

	private int _blankRow;
	private int _blankColumn;
	private int _moveCount;
	private bool _isSuccess;

	private G2Texture? _texture;
	private G2Font? _font;
	private G2AudioSound? _soundMove;
	private G2AudioSound? _soundSuccess;

	public void Initialize()
	{
		var app = G2AppBase.Instance ?? throw new InvalidOperationException("G2AppBase instance is not initialized.");
		app.ClearColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
		_texture = new G2Texture("resource/Texture/img1.bmp");
		_font = new G2Font("Arial", 50);
		_soundSuccess = new G2AudioSound("resource/sound/trample.wav");
		_soundMove = new G2AudioSound("resource/sound/move3.wav");
		ResetPuzzle();
	}

	public void Update()
	{
		var input = G2AppBase.Instance?.Input ?? throw new InvalidOperationException("G2AppBase instance is not initialized.");
		if (_isSuccess)
		{
			if (input.IsKeyDown(Keys.Home))
			{
				ResetPuzzle();
			}
			return;
		}

		if (input.IsKeyDown(Keys.Insert))
		{
			SetSolved();
			CheckSuccess();
			return;
		}

		if (input.IsKeyDown(Keys.Right))
		{
			if (MoveTile(MoveDirection.Right, true))
			{
				_soundMove?.Play();
			}
		}
		else if (input.IsKeyDown(Keys.Left))
		{
			if (MoveTile(MoveDirection.Left, true))
			{
				_soundMove?.Play();
			}
		}
		else if (input.IsKeyDown(Keys.Down))
		{
			if (MoveTile(MoveDirection.Down, true))
			{
				_soundMove?.Play();
			}
		}
		else if (input.IsKeyDown(Keys.Up))
		{
			if (MoveTile(MoveDirection.Up, true))
			{
				_soundMove?.Play();
			}
		}

		CheckSuccess();
	}

	public void Render()
	{
		if (_texture == null)
		{
			return;
		}

		if (_isSuccess)
		{
			Rect destination = new(0, 0, GameGlobal.ScreenSize.Width, GameGlobal.ScreenSize.Height);
			Rect source = new(0, 0, ImageWidth, ImageHeight);
			_texture.Draw(destination, source, 1.0f, BitmapInterpolationMode.NearestNeighbor);
			_font?.DrawText( "Complete!!!"
				, new Rect( GameGlobal.ScreenSize.Width / 2 - 200, GameGlobal.ScreenSize.Height - 180, 400, 70)
				, new Color4(1.0f, 0.73f, 0.47f, 1.0f));
			_font?.DrawText($"Move : {_moveCount}"
				, new Rect( GameGlobal.ScreenSize.Width / 2 - 160, GameGlobal.ScreenSize.Height - 100, 320, 60)
				, new Color4(1.0f, 1.0f, 1.0f, 1.0f));
			return;
		}

		DrawPuzzle();
	}

	private void ResetPuzzle()
	{
		SetSolved();
		Shuffle();
		_moveCount = 0;
		_isSuccess = false;
	}

	private void SetSolved()
	{
		int index = 0;
		for (int row = 0; row < RowCount; ++row)
		{
			for (int column = 0; column < ColumnCount; ++column)
			{
				_map[row, column] = index;
				++index;
			}
		}
		_blankRow = RowCount - 1;
		_blankColumn = ColumnCount - 1;
	}

	private void Shuffle()
	{
		MoveDirection previous = MoveDirection.None;
		for (int i = 0; i < ShuffleCount; ++i)
		{
			List<MoveDirection> directions = GetMovableDirections(previous);
			MoveDirection direction = directions[Random.Shared.Next(directions.Count)];
			MoveTile(direction, false);
			previous = direction;
		}
		if (IsSolved())
		{
			List<MoveDirection> directions =
				GetMovableDirections(MoveDirection.None);
			MoveTile(directions[Random.Shared.Next(directions.Count)], false);
		}
	}

	private List<MoveDirection> GetMovableDirections(MoveDirection previous)
	{
		List<MoveDirection> directions = [];
		if (_blankColumn < ColumnCount - 1 && previous != MoveDirection.Right)
		{
			directions.Add(MoveDirection.Left);
		}

		if (_blankColumn > 0 && previous != MoveDirection.Left)
		{
			directions.Add(MoveDirection.Right);
		}

		if (_blankRow < RowCount - 1 && previous != MoveDirection.Down)
		{
			directions.Add(MoveDirection.Up);
		}

		if (_blankRow > 0 && previous != MoveDirection.Up)
		{
			directions.Add(MoveDirection.Down);
		}

		return directions;
	}

	private bool MoveTile(
		MoveDirection direction,
		bool countMove)
	{
		int tileRow = _blankRow;
		int tileColumn = _blankColumn;

		switch (direction)
		{
			case MoveDirection.Right:
				tileColumn = _blankColumn - 1;
				break;

			case MoveDirection.Left:
				tileColumn = _blankColumn + 1;
				break;

			case MoveDirection.Down:
				tileRow = _blankRow - 1;
				break;

			case MoveDirection.Up:
				tileRow = _blankRow + 1;
				break;

			default:
				return false;
		}

		if (tileRow < 0 ||
			tileRow >= RowCount ||
			tileColumn < 0 ||
			tileColumn >= ColumnCount)
		{
			return false;
		}

		_map[_blankRow, _blankColumn] = _map[tileRow, tileColumn];
		_map[tileRow, tileColumn] = _blankIndex;
		_blankRow = tileRow;
		_blankColumn = tileColumn;
		if (countMove)
		{
			++_moveCount;
		}

		return true;
	}

	private void CheckSuccess()
	{
		if (!IsSolved())
		{
			return;
		}
		_isSuccess = true;
		_soundSuccess?.Play();
	}

	private bool IsSolved()
	{
		int index = 0;
		for (int row = 0; row < RowCount; ++row)
		{
			for (int column = 0; column < ColumnCount; ++column)
			{
				if (_map[row, column] != index)
				{
					return false;
				}
				++index;
			}
		}
		return true;
	}

	private void DrawPuzzle()
	{
		if (_texture == null)
		{
			return;
		}
		for (int row = 0; row < RowCount; ++row)
		{
			for (int column = 0; column < ColumnCount; ++column)
			{
				int imageIndex = _map[row, column];

				if (imageIndex == _blankIndex)
				{
					continue;
				}

				int sourceColumn = imageIndex % ColumnCount;
				int sourceRow = imageIndex / ColumnCount;

				Rect source = new(sourceColumn * _sourceBlockWidth, sourceRow * _sourceBlockHeight, _sourceBlockWidth, _sourceBlockHeight);
				Rect destination = new(column * DestBlockWidth + Gap / 2, row * DestBlockHeight + Gap / 2, DestBlockWidth - Gap, DestBlockHeight - Gap);
				_texture.Draw(destination, source, 1.0f, BitmapInterpolationMode.NearestNeighbor);
			}
		}
	}

	public void Dispose()
	{
		_soundMove?.Dispose();
		_soundMove = null;
		_soundSuccess?.Dispose();
		_soundSuccess = null;
		_font?.Dispose();
		_font = null;
		_texture?.Dispose();
		_texture = null;
	}
}