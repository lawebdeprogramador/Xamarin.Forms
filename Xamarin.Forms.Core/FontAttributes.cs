using System;

namespace Xamarin.Forms
{
	[Flags]
	public enum FontAttributes
	{
		None = 0,
		Bold = 1 << 0,
		Italic = 1 << 1,
		Underline = 1 << 2,
		Strike = 1 << 3
	}
}