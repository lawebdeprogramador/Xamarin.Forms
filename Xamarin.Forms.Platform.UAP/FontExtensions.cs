using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Internals;
using WApplication = Windows.UI.Xaml.Application;

namespace Xamarin.Forms.Platform.UWP
{
	public static class FontExtensions
	{
		public static void ApplyFont(this Control self, Font font)
		{
			self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = !string.IsNullOrEmpty(font.FontFamily) ? new FontFamily(font.FontFamily) : (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		public static void ApplyFont(this TextBlock self, Font font)
		{
			self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = !string.IsNullOrEmpty(font.FontFamily) ? new FontFamily(font.FontFamily) : (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		public static void ApplyFont(this Windows.UI.Xaml.Documents.TextElement self, Font font)
		{
			self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = !string.IsNullOrEmpty(font.FontFamily) ? new FontFamily(font.FontFamily) : (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		internal static void ApplyFont(this Control self, IFontElement element)
		{
			self.FontSize = element.FontSize;
			self.FontFamily = !string.IsNullOrEmpty(element.FontFamily) ? new FontFamily(element.FontFamily) : (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];
			self.FontStyle = element.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = element.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		internal static double GetFontSize(this NamedSize size)
		{
			// These are values pulled from the mapped sizes on Windows Phone, WinRT has no equivalent sizes, only intents.
			switch (size)
			{
				case NamedSize.Default:
					return (double)WApplication.Current.Resources["ControlContentThemeFontSize"];
				case NamedSize.Micro:
					return 18.667 - 3;
				case NamedSize.Small:
					return 18.667;
				case NamedSize.Medium:
					return 22.667;
				case NamedSize.Large:
					return 32;
				default:
					throw new ArgumentOutOfRangeException("size");
			}
		}

		internal static bool IsDefault(this IFontElement self)
		{
			return self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) && self.FontAttributes == FontAttributes.None;
		}

		public static void SetTextDecoration(this TextBlock self, FontAttributes fontAttributes)
		{
			if (HasTextDecorationChanges(self, fontAttributes))
			{
				var addUnderline = self.TextDecorations | TextDecorations.Underline;
				var removeUnderline = self.TextDecorations & ~TextDecorations.Underline;

				self.TextDecorations = fontAttributes.HasFlag(FontAttributes.Underline) ? addUnderline : removeUnderline;

				var addStrike = self.TextDecorations | TextDecorations.Strikethrough;
				var removeStrike = self.TextDecorations & ~TextDecorations.Strikethrough;

				self.TextDecorations = fontAttributes.HasFlag(FontAttributes.Strike) ? addStrike : removeStrike;
				//text decorations do not update in the UI until the text is also changed...
				self.Text += "";
			}
		}

		public static void SetTextDecoration(this Windows.UI.Xaml.Documents.TextElement self, FontAttributes fontAttributes)
		{
			if (HasTextDecorationChanges(self, fontAttributes))
			{
				var addUnderline = self.TextDecorations | TextDecorations.Underline;
				var removeUnderline = self.TextDecorations & ~TextDecorations.Underline;

				self.TextDecorations = fontAttributes.HasFlag(FontAttributes.Underline) ? addUnderline : removeUnderline;

				var addStrike = self.TextDecorations | TextDecorations.Strikethrough;
				var removeStrike = self.TextDecorations & ~TextDecorations.Strikethrough;

				self.TextDecorations = fontAttributes.HasFlag(FontAttributes.Strike) ? addStrike : removeStrike;
			}
		}

		static bool HasTextDecorationChanges(TextBlock textBlock, FontAttributes fontAttributes)
		{
			var nativeFlags = textBlock.TextDecorations;
			var sameStrikeValue = nativeFlags.HasFlag(TextDecorations.Strikethrough) == fontAttributes.HasFlag(FontAttributes.Strike);
			var sameUnderlineValue = nativeFlags.HasFlag(TextDecorations.Underline) == fontAttributes.HasFlag(FontAttributes.Underline);

			return (!sameStrikeValue) || (!sameUnderlineValue);
		}

		static bool HasTextDecorationChanges(Windows.UI.Xaml.Documents.TextElement textElement, FontAttributes fontAttributes)
		{
			var nativeFlags = textElement.TextDecorations;
			var sameStrikeValue = nativeFlags.HasFlag(TextDecorations.Strikethrough) == fontAttributes.HasFlag(FontAttributes.Strike);
			var sameUnderlineValue = nativeFlags.HasFlag(TextDecorations.Underline) == fontAttributes.HasFlag(FontAttributes.Underline);

			return (!sameStrikeValue) || (!sameUnderlineValue);
		}
	}
}