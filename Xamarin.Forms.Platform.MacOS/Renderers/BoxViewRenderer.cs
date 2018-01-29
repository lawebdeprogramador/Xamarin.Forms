using System.ComponentModel;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class BoxViewRenderer : ViewRenderer<BoxView, NSView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new NSView());
				}

				SetBackgroundColor(Element.Color);
				SetCornerRadius();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == BoxView.ColorProperty.PropertyName)
				SetBackgroundColor(Element.Color);
			else if (e.PropertyName == BoxView.CornerRadiusProperty.PropertyName)
				SetCornerRadius();
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && Element.IsVisible)
				SetNeedsDisplayInRect(Bounds);
		}

		protected override void SetBackgroundColor(Color color)
		{
			if (Element == null || Control == null)
				return;
			Control.WantsLayer = true;
			Control.Layer.BackgroundColor = color.ToCGColor();
		}

		void SetCornerRadius()
		{
			if (Element == null)
				return;

			var elementCornerRadius = Element.CornerRadius;

			Layer.MasksToBounds = true;
			Layer.CornerRadius = (float)elementCornerRadius.TopLeft;
		}
	}
}