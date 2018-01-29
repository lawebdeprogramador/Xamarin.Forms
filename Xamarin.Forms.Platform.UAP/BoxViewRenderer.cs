using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UWP
{
	public class BoxViewRenderer : ViewRenderer<BoxView, Border>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var rect = new Border
					{
						DataContext = Element
					};

					rect.SetBinding(Border.BackgroundProperty, new Windows.UI.Xaml.Data.Binding { Converter = new ColorConverter(), Path = new PropertyPath("Color") });

					SetNativeControl(rect);
				}

				SetCornerRadius(Element.CornerRadius);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == BoxView.CornerRadiusProperty.PropertyName)
				SetCornerRadius(Element.CornerRadius);
		}

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			// We need an automation peer so we can interact with this in automated tests
			if (Control == null)
			{
				return new FrameworkElementAutomationPeer(this);
			}

			return new FrameworkElementAutomationPeer(Control);
		}

		private void SetCornerRadius(Core.CornerRadius cornerRadius)
		{
			Control.CornerRadius = new CornerRadius(
				cornerRadius.TopLeft,
				cornerRadius.TopRight,
				cornerRadius.BottomRight,
				cornerRadius.BottomLeft);
		}
	}
}