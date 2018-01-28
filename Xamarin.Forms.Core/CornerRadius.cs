using System;

namespace Xamarin.Forms.Core
{
	public struct CornerRadius
	{
		public double TopLeft { get; set; }
		public double TopRight { get; set; }
		public double BottomLeft { get; set; }
		public double BottomRight { get; set; }

		public CornerRadius(double uniformRadius) : this(uniformRadius, uniformRadius, uniformRadius, uniformRadius)
		{
		}

		public CornerRadius(double topLeft, double topRight, double bottomLeft, double bottomRight)
		{
			if (topLeft < 0 || double.IsNaN(topLeft))
				throw new ArgumentException("TopLeft is less than 0 or is not a number", "topLeft");
			if (topRight < 0 || double.IsNaN(topRight))
				throw new ArgumentException("TopRight is less than 0 or is not a number", "topRight");
			if (bottomLeft < 0 || double.IsNaN(bottomLeft))
				throw new ArgumentException("BottomLeft is less than 0 or is not a number", "bottomLeft");
			if (bottomRight < 0 || double.IsNaN(bottomRight))
				throw new ArgumentException("BottomRight is less than 0 or is not a number", "bottomRight");

			TopLeft = topLeft;
			TopRight = topRight;
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
		}

		public static implicit operator CornerRadius(double uniformRadius)
		{
			return new CornerRadius(uniformRadius);
		}
	}
}
