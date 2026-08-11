// -------------------------------------------------------------------------------------------------------------------------------------------------------------
// Author: 3dapi (https://github.com/3dapi)
// -------------------------------------------------------------------------------------------------------------------------------------------------------------

using Vortice.Direct2D1;
using Vortice.Mathematics;

class G2Texture : IDisposable
{
	// 텍스처 데이터를 저장하는 내부 클래스.
	private class TextureData
	{
		public ID2D1Bitmap Bitmap = null!;
		public int Count;
	}
	private static readonly Dictionary<string, TextureData> TextureList = new();

	public string FilePath { get; }

	// 텍스처 데이터와 관련된 필드.
	private TextureData? _textureData;

	public G2Texture(string filePath)
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
		ID2D1Bitmap? bitmap = G2TextureLoader.LoadBitmap(FilePath);
		if (bitmap == null)
		{
			throw new FileNotFoundException($"G2Texture::{FilePath} 이미지를 찾을 수 없습니다.");
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
		ID2D1RenderTarget renderTarget = G2AppBase.Instance?.RenderTarget?? throw new InvalidOperationException("G2Texture::G2AppBase instance is not initialized.");
		renderTarget.DrawBitmap(_textureData!.Bitmap, opacity, interpolationMode);
	}

	public void Draw(Rect destination, Rect sourceRectangle, float opacity = 1.0f, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.Linear)
	{
		ID2D1RenderTarget renderTarget = G2AppBase.Instance?.RenderTarget?? throw new InvalidOperationException("G2Texture::G2AppBase instance is not initialized.");
		renderTarget.DrawBitmap(_textureData!.Bitmap, destination, opacity, interpolationMode, sourceRectangle);
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
