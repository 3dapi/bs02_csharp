using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;

namespace glc
{
	public class AppFont : IDisposable
	{
		private record struct FontKey(
			string FamilyName
			, float FontSize
			, FontWeight Weight
			, FontStyle Style
			, TextAlignment TextAlign
			, ParagraphAlignment ParaAlign
			);
		private static Dictionary<FontKey, IDWriteTextFormat> FontList = new();

		public ID2D1HwndRenderTarget? _renderTarget;
		private IDWriteTextFormat _textFormat;
		private ID2D1SolidColorBrush _textBrush;

		public AppFont(string fontFamilyName
					, float fontSize
					, FontWeight fontWeight = FontWeight.Heavy
					, FontStyle fontStyle   = FontStyle.Normal
					, TextAlignment textAlignment = TextAlignment.Leading
					, ParagraphAlignment paragraphAlignment = ParagraphAlignment.Near
			)
		{
			// 1. AppBase.Instance를 지역 변수에 담아 null 검사를 안전하게 수행합니다.
			var appInstance = AppBase.Instance ?? throw new InvalidOperationException("AppBase instance is not initialized.");

			// 2. 안전해진 지역 변수를 통해 RenderTarget과 DWriteFactory를 가져옵니다.
			_renderTarget = appInstance.RenderTarget ?? throw new ArgumentNullException(nameof(appInstance.RenderTarget));
			var writeFactory = appInstance.DWriteFactory ?? throw new ArgumentNullException(nameof(appInstance.DWriteFactory));



			// 1. 텍스트 색상 브러시 생성
			_textBrush = _renderTarget.CreateSolidColorBrush(new Color4(1.0F, 1.0F, 1.0F, 1.0F));

			var key = new FontKey(fontFamilyName, fontSize, fontWeight, fontStyle, textAlignment, paragraphAlignment);

			// Dictionary에 해당 속성의 폰트가 없다면 새로 생성해서 캐싱
			if (!FontList.TryGetValue(key, out IDWriteTextFormat? font))
			{
				font = writeFactory.CreateTextFormat(
					fontFamilyName
					, fontWeight
					, fontStyle
					, fontSize
				);
				font.TextAlignment = textAlignment;
				font.ParagraphAlignment = paragraphAlignment;
				FontList.Add(key, font);
			}

			// 2. 텍스트 포맷 생성
			_textFormat = font;
		}

		public void DrawText(string text, Rect layoutRect, Color4 color)
		{
			_textBrush.Color = color;
			_renderTarget?.DrawText(text, _textFormat, layoutRect, _textBrush);
		}

		public void Dispose()
		{
			// Native 리소스 안전 해제
			_textBrush?.Dispose();
		}

		public static void ClearFontCache()
		{
			foreach (var font in FontList.Values)
			{
				font.Dispose();
			}
			FontList.Clear();
		}
	}
}