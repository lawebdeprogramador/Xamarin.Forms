﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Xamarin.Forms.Platform.UWP
{
	public static class FormattedStringExtensions
	{
		public static Run ToRun(this Span span)
		{
			var run = new Run { Text = span.Text ?? string.Empty };

			if (span.TextColor != Color.Default)
				run.Foreground = span.TextColor.ToBrush();

			if (!span.IsDefault())
#pragma warning disable 618
				run.ApplyFont(span.Font);
#pragma warning restore 618

			return run;
		}
	}

	public class LabelRenderer : ViewRenderer<Label, TextBlock>
	{
		bool _fontApplied;
		bool _isInitiallyDefault;
		SizeRequest _perfectSize;
		bool _perfectSizeValid;
		IList<double> _inlineHeights = new List<double>();

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			// We need an automation peer so we can interact with this in automated tests
			if (Control == null)
			{
				return new FrameworkElementAutomationPeer(this);
			}

			return new FrameworkElementAutomationPeer(Control);
		}

		protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
		{
			if (Element == null)
				return finalSize;

			double childHeight = Math.Max(0, Math.Min(Element.Height, Control.DesiredSize.Height));
			var rect = new Rect();

			switch (Element.VerticalTextAlignment)
			{
				case TextAlignment.Start:
					break;
				default:
				case TextAlignment.Center:
					rect.Y = (int)((finalSize.Height - childHeight) / 2);
					break;
				case TextAlignment.End:
					rect.Y = finalSize.Height - childHeight;
					break;
			}
			rect.Height = childHeight;
			rect.Width = finalSize.Width;
			Control.Arrange(rect);
			RecalculatePositions(finalSize);
			return finalSize;
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (!_perfectSizeValid)
			{
				_perfectSize = base.GetDesiredSize(double.PositiveInfinity, double.PositiveInfinity);
				_perfectSize.Minimum = new Size(Math.Min(10, _perfectSize.Request.Width), _perfectSize.Request.Height);
				_perfectSizeValid = true;
			}

			var widthFits = widthConstraint >= _perfectSize.Request.Width;
			var heightFits = heightConstraint >= _perfectSize.Request.Height;

			if (widthFits && heightFits)
				return _perfectSize;

			var result = base.GetDesiredSize(widthConstraint, heightConstraint);
			var tinyWidth = Math.Min(10, result.Request.Width);
			result.Minimum = new Size(tinyWidth, result.Request.Height);

			if (widthFits || Element.LineBreakMode == LineBreakMode.NoWrap)
				return result;

			bool containerIsNotInfinitelyWide = !double.IsInfinity(widthConstraint);

			if (containerIsNotInfinitelyWide)
			{
				bool textCouldHaveWrapped = Element.LineBreakMode == LineBreakMode.WordWrap || Element.LineBreakMode == LineBreakMode.CharacterWrap;
				bool textExceedsContainer = result.Request.Width > widthConstraint;

				if (textExceedsContainer || textCouldHaveWrapped)
				{
					var expandedWidth = Math.Max(tinyWidth, widthConstraint);
					result.Request = new Size(expandedWidth, result.Request.Height);
				}
			}

			return result;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new TextBlock());
				}

				_isInitiallyDefault = Element.IsDefault();

				UpdateText(Control);
				UpdateColor(Control);
				UpdateAlign(Control);
				UpdateFont(Control);
				UpdateLineBreakMode(Control);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Label.TextProperty.PropertyName || e.PropertyName == Label.FormattedTextProperty.PropertyName)
				UpdateText(Control);
			else if (e.PropertyName == Label.TextColorProperty.PropertyName)
				UpdateColor(Control);
			else if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName || e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
				UpdateAlign(Control);
			else if (e.PropertyName == Label.FontProperty.PropertyName)
				UpdateFont(Control);
			else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
				UpdateLineBreakMode(Control);
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateAlign(Control);

			base.OnElementPropertyChanged(sender, e);
		}

		void UpdateAlign(TextBlock textBlock)
		{
			_perfectSizeValid = false;

			if (textBlock == null)
				return;

			Label label = Element;
			if (label == null)
				return;

			textBlock.TextAlignment = label.HorizontalTextAlignment.ToNativeTextAlignment(((IVisualElementController)Element).EffectiveFlowDirection);
			textBlock.VerticalAlignment = label.VerticalTextAlignment.ToNativeVerticalAlignment();
		}

		void UpdateColor(TextBlock textBlock)
		{
			if (textBlock == null)
				return;

			Label label = Element;
			if (label != null && label.TextColor != Color.Default)
			{
				textBlock.Foreground = label.TextColor.ToBrush();
			}
			else
			{
				textBlock.ClearValue(TextBlock.ForegroundProperty);
			}
		}

		void UpdateFont(TextBlock textBlock)
		{
			_perfectSizeValid = false;

			if (textBlock == null)
				return;

			Label label = Element;
			if (label == null || (label.IsDefault() && !_fontApplied))
				return;

#pragma warning disable 618
			Font fontToApply = label.IsDefault() && _isInitiallyDefault ? Font.SystemFontOfSize(NamedSize.Medium) : label.Font;
#pragma warning restore 618

			textBlock.ApplyFont(fontToApply);
			_fontApplied = true;
		}

		void UpdateLineBreakMode(TextBlock textBlock)
		{
			_perfectSizeValid = false;

			if (textBlock == null)
				return;

			switch (Element.LineBreakMode)
			{
				case LineBreakMode.NoWrap:
					textBlock.TextTrimming = TextTrimming.Clip;
					textBlock.TextWrapping = TextWrapping.NoWrap;
					break;
				case LineBreakMode.WordWrap:
					textBlock.TextTrimming = TextTrimming.None;
					textBlock.TextWrapping = TextWrapping.Wrap;
					break;
				case LineBreakMode.CharacterWrap:
					textBlock.TextTrimming = TextTrimming.WordEllipsis;
					textBlock.TextWrapping = TextWrapping.Wrap;
					break;
				case LineBreakMode.HeadTruncation:
					// TODO: This truncates at the end.
					textBlock.TextTrimming = TextTrimming.WordEllipsis;
					textBlock.TextWrapping = TextWrapping.NoWrap;
					break;
				case LineBreakMode.TailTruncation:
					textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
					textBlock.TextWrapping = TextWrapping.NoWrap;
					break;
				case LineBreakMode.MiddleTruncation:
					// TODO: This truncates at the end.
					textBlock.TextTrimming = TextTrimming.WordEllipsis;
					textBlock.TextWrapping = TextWrapping.NoWrap;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		double FindDefaultLineHeight(Inline inline)
		{
			var control = new TextBlock();
			control.Inlines.Add(inline);

			control.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));

			var height = control.DesiredSize.Height;

			control.Inlines.Remove(inline);
			control = null;

			return height;
		}

		void RecalculatePositions(Windows.Foundation.Size finalSize)
		{
			if (Element?.FormattedText?.Spans == null
				|| Element.FormattedText.Spans.Count == 0)
				return;

			for (int i = 0; i < Element.FormattedText.Spans.Count; i++)
			{
				var span = Element.FormattedText.Spans[i];

				var inline = Control.Inlines[i];
				var rect = inline.ContentStart.GetCharacterRect(LogicalDirection.Forward);
				var endRect = inline.ContentEnd.GetCharacterRect(LogicalDirection.Forward);

				var positions = new List<Rectangle>();
				var labelWidth = Control.ActualWidth;

				var defaultLineHeight = _inlineHeights[i];
								
				var yaxis = rect.Top;
				var lineHeights = new List<double>();
				while (yaxis < endRect.Bottom)
				{
					double lineHeight;
					if (yaxis == rect.Top) // First Line
					{
						lineHeight = rect.Bottom - rect.Top;
					}
					else if (yaxis != endRect.Top) // Middle Line(s)
					{
						lineHeight = defaultLineHeight;
					}
					else // Bottom Line
					{
						lineHeight = endRect.Bottom - endRect.Top;
					}
					lineHeights.Add(lineHeight);
					yaxis += lineHeight;
				}

				span.CalculatePositions(lineHeights.ToArray(), finalSize.Width, rect.X, endRect.X + endRect.Width, rect.Top);

			}
		}
				
		void UpdateText(TextBlock textBlock)
		{
			_perfectSizeValid = false;

			if (textBlock == null)
				return;

			Label label = Element;
			if (label != null)
			{
				FormattedString formatted = label.FormattedText;

				if (formatted == null)
				{
					textBlock.Text = label.Text ?? string.Empty;
				}
				else
				{
					textBlock.Inlines.Clear();
					// Have to implement a measure here, otherwise inline.ContentStart and ContentEnd will be null, when used in RecalculatePositions
					textBlock.Measure(new Windows.Foundation.Size(double.MaxValue, double.MaxValue));

					var heights = new List<double>();
					for (var i = 0; i < formatted.Spans.Count; i++)
					{
						var span = formatted.Spans[i];
						if (span.Text != null)
						{
							var run = span.ToRun();
							heights.Add(FindDefaultLineHeight(run));
							textBlock.Inlines.Add(run);
						}
					}
					_inlineHeights = heights;
				}
			}
		}
	}
}
