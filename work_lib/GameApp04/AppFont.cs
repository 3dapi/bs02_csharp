using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;

class AppFont : IDisposable
{
	private readonly record struct FontKey(
		string FamilyName,
		float FontSize,
		FontWeight Weight,
		FontStyle Style,
		TextAlignment TextAlign,
		ParagraphAlignment ParaAlign);

	private class FontData
	{
		public IDWriteTextFormat TextFormat = null!;
		public int Count;
	}

	private static readonly Dictionary<FontKey, FontData> FontList = new();

	private readonly FontKey _fontKey;
	private readonly FontData _fontData;
	private readonly ID2D1SolidColorBrush _textBrush;

	public AppFont(string fontFamilyName
				, float fontSize
				, FontWeight fontWeight = FontWeight.Heavy
				, FontStyle fontStyle = FontStyle.Normal
				, TextAlignment textAlignment = TextAlignment.Leading
				, ParagraphAlignment paragraphAlignment = ParagraphAlignment.Near)
	{
		var app = AppBase.Instance
			?? throw new InvalidOperationException("AppBase instance is not initialized.");

		_textBrush = app.RenderTarget.CreateSolidColorBrush(new Color4(1.0f, 1.0f, 1.0f, 1.0f));
		_fontKey = new FontKey(fontFamilyName, fontSize, fontWeight, fontStyle, textAlignment, paragraphAlignment);

		if (FontList.TryGetValue(_fontKey, out FontData? fontData))
		{
			fontData.Count++;
			_fontData = fontData;
			return;
		}

		IDWriteTextFormat textFormat = app.DWriteFactory.CreateTextFormat(fontFamilyName, fontWeight, fontStyle, fontSize);
		textFormat.TextAlignment = textAlignment;
		textFormat.ParagraphAlignment = paragraphAlignment;
		_fontData = new FontData
		{
			TextFormat = textFormat,
			Count = 1
		};
		FontList.Add(_fontKey, _fontData);
	}

	public void DrawText(string text, Rect layoutRect, Color4 color)
	{
		var renderTarget = AppBase.Instance?.RenderTarget
			?? throw new InvalidOperationException("AppBase instance is not initialized.");
		_textBrush.Color = color;
		renderTarget.DrawText(text, _fontData.TextFormat, layoutRect, _textBrush);
	}

	public void Dispose()
	{
		_textBrush.Dispose();
		_fontData.Count--;
		if (_fontData.Count == 0)
		{
			_fontData.TextFormat.Dispose();
			FontList.Remove(_fontKey);
		}
	}
}