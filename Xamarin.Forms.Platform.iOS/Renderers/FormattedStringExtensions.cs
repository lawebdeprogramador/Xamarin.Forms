using Foundation;
using Xamarin.Forms.Internals;
#if __MOBILE__
using UIKit;

namespace Xamarin.Forms.Platform.iOS
#else
using AppKit;
using UIColor = AppKit.NSColor;

namespace Xamarin.Forms.Platform.MacOS
#endif
{
	public static class FormattedStringExtensions
	{
		public static NSAttributedString ToAttributed(this Span span, Font defaultFont, Color defaultForegroundColor)
		{
			if (span == null)
				return null;

#pragma warning disable 0618 //retaining legacy call to obsolete code
			var font = span.Font != Font.Default ? span.Font : defaultFont;
#pragma warning restore 0618
			var fgcolor = span.ForegroundColor;
			if (fgcolor.IsDefault)
				fgcolor = defaultForegroundColor;
			if (fgcolor.IsDefault)
				fgcolor = Color.Black; // as defined by apple docs		

#if __MOBILE__
			return new NSAttributedString(span.Text, font == Font.Default ? null : font.ToUIFont(), fgcolor.ToUIColor(), span.BackgroundColor.ToUIColor());
#else
			return new NSAttributedString(span.Text, font == Font.Default ? null : font.ToNSFont(), fgcolor.ToNSColor(),
				span.BackgroundColor.ToNSColor());
#endif
		}

		public static NSAttributedString ToAttributed(this FormattedString formattedString, Font defaultFont,
			Color defaultForegroundColor)
		{
			if (formattedString == null)
				return null;
			var attributed = new NSMutableAttributedString();
			foreach (var span in formattedString.Spans)
			{
				if (span.Text == null)
					continue;

				attributed.Append(span.ToAttributed(defaultFont, defaultForegroundColor));
			}

			return attributed;
		}

		internal static NSAttributedString ToAttributed(this Span span, Element owner, Color defaultForegroundColor)
		{
			if (span == null)
				return null;

#if __MOBILE__
			UIFont targetFont;
			IFontElement fontElement = (IFontElement)owner;
			if (span.IsDefault())
				targetFont = fontElement.ToUIFont();
			else
				targetFont = span.ToUIFont();

			var fgcolor = span.ForegroundColor;
			if (fgcolor.IsDefault)
				fgcolor = defaultForegroundColor;
			if (fgcolor.IsDefault)
				fgcolor = Color.Black; // as defined by apple docs

			NSUnderlineStyle underlineStyle;
			NSUnderlineStyle strikeThroughStyle;

			underlineStyle = span.FontAttributes.HasFlag(FontAttributes.Underline) ? NSUnderlineStyle.Single : NSUnderlineStyle.None;
			strikeThroughStyle = span.FontAttributes.HasFlag(FontAttributes.Strike) ? NSUnderlineStyle.Single : NSUnderlineStyle.None;

			var result = new NSAttributedString(span.Text, targetFont, fgcolor.ToUIColor(), span.BackgroundColor.ToUIColor(), underlineStyle: underlineStyle, strikethroughStyle: strikeThroughStyle);

			return result;
#else
			NSFont targetFont;
			if (span.IsDefault())
				targetFont = ((IFontElement)owner).ToNSFont();
			else
				targetFont = span.ToNSFont();

			var fgcolor = span.ForegroundColor;
			if (fgcolor.IsDefault)
				fgcolor = defaultForegroundColor;
			if (fgcolor.IsDefault)
				fgcolor = Color.Black; // as defined by apple docs

			return new NSAttributedString(span.Text, targetFont, fgcolor.ToNSColor(), span.BackgroundColor.ToNSColor());
#endif
		}

		internal static NSAttributedString ToAttributed(this FormattedString formattedString, Element owner,
			Color defaultForegroundColor)
		{
			if (formattedString == null)
				return null;
			var attributed = new NSMutableAttributedString();
			foreach (var span in formattedString.Spans)
			{
				if (span.Text == null)
					continue;

				attributed.Append(span.ToAttributed(owner, defaultForegroundColor));
			}

			return attributed;
		}
	}
}