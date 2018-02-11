using System;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;

namespace Xamarin.Forms.Maps.UWP
{
	internal class PushPin : ContentControl
	{
		readonly CustomPin _pin;

		internal PushPin(Pin pin, DataTemplate pinTemplate = null)
		{
			if (pin == null)
				throw new ArgumentNullException();

			if (pinTemplate != null)
				ContentTemplate = Windows.UI.Xaml.Application.Current.Resources["ViewCell"] as Windows.UI.Xaml.DataTemplate;
			else
				ContentTemplate = Windows.UI.Xaml.Application.Current.Resources["PushPinTemplate"] as Windows.UI.Xaml.DataTemplate;

			DataContext = Content = _pin = new CustomPin(pin, pinTemplate);

			UpdateLocation();

			Loaded += PushPinLoaded;
			Unloaded += PushPinUnloaded;
			Tapped += PushPinTapped;
		}

		void PushPinLoaded(object sender, RoutedEventArgs e)
		{
			_pin.PropertyChanged += PinPropertyChanged;
		}

		void PushPinUnloaded(object sender, RoutedEventArgs e)
		{
			_pin.PropertyChanged -= PinPropertyChanged;
			Tapped -= PushPinTapped;
		}

		void PinPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Pin.PositionProperty.PropertyName)
				UpdateLocation();
		}

		void PushPinTapped(object sender, TappedRoutedEventArgs e)
		{
			_pin.SendTap();
		}

		void UpdateLocation()
		{
			var anchor = new Windows.Foundation.Point(0.65, 1);
			var location = new Geopoint(new BasicGeoposition
			{
				Latitude = _pin.Position.Latitude,
				Longitude = _pin.Position.Longitude
			});
			MapControl.SetLocation(this, location);
			MapControl.SetNormalizedAnchorPoint(this, anchor);
		}
	}

	internal class CustomPin : Pin
	{
		internal CustomPin(Pin pin, DataTemplate pinTemplate = null)
		{
			Address = pin.Address;
			Label = pin.Label;
			Position = pin.Position;
			Type = pin.Type;

			if (pinTemplate != null)
			{
				Cell = pinTemplate.CreateContent() as ViewCell;
			}
		}

		public Cell Cell { get; set; }
	}
}