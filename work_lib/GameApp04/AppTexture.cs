using System;
using System.Collections.Generic;
using System.IO;
using Vortice.Direct2D1;
using Vortice.Mathematics;

class AppTexture : IDisposable
{
	private class TextureData
	{
		public ID2D1Bitmap Bitmap = null!;
		public int Count;
	}
	private static readonly Dictionary<string, TextureData> TextureList = new();
	public string FilePath { get; }
	private TextureData? _textureData;

	public AppTexture(string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath))
		{
			throw new ArgumentException("파일 경로가 비어 있습니다.", nameof(filePath));
		}

		FilePath = Path.GetFullPath(Path.IsPathRooted(filePath)? filePath : Path.Combine(AppContext.BaseDirectory, filePath));

		if (TextureList.TryGetValue(FilePath, out TextureData? texture))
		{
			texture.Count++;
			_textureData = texture;
			return;
		}

		ID2D1Bitmap? bitmap = AppUtil.LoadBitmap(FilePath);
		if (bitmap == null)
		{
			throw new FileNotFoundException("이미지 파일을 찾을 수 없습니다.", FilePath);
		}
		_textureData = new TextureData
		{
			Bitmap = bitmap,
			Count = 1
		};
		TextureList.Add(FilePath, _textureData);
	}

	public void Draw(float opacity = 1.0f, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.Linear)
	{
		AppBase app = AppBase.Instance?? throw new InvalidOperationException("AppBase instance is not initialized.");
		app.RenderTarget.DrawBitmap(_textureData!.Bitmap, opacity, interpolationMode);
	}

	public void Draw(Rect destination, Rect sourceRectangle, float opacity = 1.0f, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.Linear)
	{
		AppBase app = AppBase.Instance?? throw new InvalidOperationException("AppBase instance is not initialized.");
		app.RenderTarget.DrawBitmap(_textureData!.Bitmap, destination, opacity, interpolationMode, sourceRectangle);
	}

	public void Dispose()
	{
		_textureData!.Count--;

		if (_textureData.Count == 0)
		{
			_textureData.Bitmap.Dispose();
			TextureList.Remove(FilePath);
		}

		_textureData = null;
	}
}
