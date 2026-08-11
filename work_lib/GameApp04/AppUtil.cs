using Vortice.Direct2D1;
using Vortice.WIC;

static class AppUtil
{
	private static IWICImagingFactory? _wicFactory;
	private static IWICImagingFactory WicFactory
	{
		get
		{
			_wicFactory ??= new IWICImagingFactory();
			return _wicFactory;
		}
	}

	public static ID2D1Bitmap? LoadBitmap(string filePath)
	{
		var renderTarget = AppBase.Instance?.RenderTarget
			?? throw new InvalidOperationException("AppBase instance is not initialized.");

		ArgumentNullException.ThrowIfNull(renderTarget);
		if (string.IsNullOrWhiteSpace(filePath))
		{
			throw new ArgumentException("파일 경로가 비어 있습니다.", nameof(filePath));
		}
		if (!File.Exists(filePath))
		{
			return null;
		}
		using IWICBitmapDecoder decoder = WicFactory.CreateDecoderFromFileName(filePath);
		using IWICBitmapFrameDecode frame = decoder.GetFrame(0);
		using IWICFormatConverter converter = WicFactory.CreateFormatConverter();
		converter.Initialize(frame, PixelFormat.Format32bppPBGRA);
		return renderTarget.CreateBitmapFromWicBitmap(converter);
	}

	public static void Release()
	{
		_wicFactory?.Dispose();
		_wicFactory = null;
	}
}
