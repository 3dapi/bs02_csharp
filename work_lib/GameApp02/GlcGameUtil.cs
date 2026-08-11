using Vortice.Direct2D1;
using Vortice.WIC;

namespace Glc2D
{
	static class GlcGameUtil
	{
		private static IWICImagingFactory? wicFactory;

		private static IWICImagingFactory WicFactory
		{
			get
			{
				wicFactory ??= new IWICImagingFactory();
				return wicFactory;
			}
		}

		public static ID2D1Bitmap? LoadBitmap( ID2D1RenderTarget renderTarget, string filePath)
		{
			ArgumentNullException.ThrowIfNull(renderTarget);
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException(
					"파일 경로가 비어 있습니다.",
					nameof(filePath));
			}

			if (!File.Exists(filePath))
			{
				return null;
			}

			using IWICBitmapDecoder decoder = WicFactory.CreateDecoderFromFileName(filePath);
			using IWICBitmapFrameDecode frame = decoder.GetFrame(0);
			using IWICFormatConverter converter = WicFactory.CreateFormatConverter();
			converter.Initialize( frame, PixelFormat.Format32bppPBGRA);
			return renderTarget.CreateBitmapFromWicBitmap(converter);
		}

		public static void Release()
		{
			wicFactory?.Dispose();
			wicFactory = null;
		}
	}
}
 