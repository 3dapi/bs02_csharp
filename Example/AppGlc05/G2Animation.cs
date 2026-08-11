using System.Diagnostics;
using Vortice.Direct2D1;
using Vortice.Mathematics;

class G2Animation : IDisposable
{
	private readonly G2Texture _texture;
	private readonly Stopwatch _timer = new();

	private readonly int _frameWidth;
	private readonly int _frameHeight;
	private readonly int _frameCount;
	private readonly double _frameTime;

	private int _frameIndex;

	public int FrameIndex => _frameIndex;

	public bool IsPlaying { get; private set; }

	public bool Loop { get; set; } = true;

	public G2Animation(string filePath, int frameWidth, int frameHeight, int frameCount, double frameTime)
	{
		_texture = new G2Texture(filePath);
		_frameWidth = frameWidth;
		_frameHeight = frameHeight;
		_frameCount = frameCount;
		_frameTime = frameTime;

		Play();
	}

	public void Play()
	{
		IsPlaying = true;
		_timer.Restart();
	}

	public void Stop()
	{
		IsPlaying = false;
		_timer.Stop();
	}

	public void Reset()
	{
		_frameIndex = 0;
		_timer.Restart();
	}

	public void Update()
	{
		if (!IsPlaying)
		{
			return;
		}
		int frameIndex = (int)(_timer.Elapsed.TotalMilliseconds / _frameTime);
		if (Loop)
		{
			_frameIndex = frameIndex % _frameCount;
		}
		else
		{
			if (frameIndex >= _frameCount)
			{
				_frameIndex = _frameCount - 1;
				IsPlaying = false;
				_timer.Stop();
			}
			else
			{
				_frameIndex = frameIndex;
			}
		}
	}

	public void Draw(float x, float y, float scale = 1.0f)
	{
		Rect source = new(_frameIndex * _frameWidth, 0, _frameWidth, _frameHeight);
		Rect destination = new(x, y, _frameWidth * scale, _frameHeight * scale);
		_texture.Draw(destination, source, 1.0f, BitmapInterpolationMode.NearestNeighbor);
	}

	public void Dispose()
	{
		_timer.Stop();
		_texture.Dispose();
	}
}