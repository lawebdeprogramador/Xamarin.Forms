namespace Xamarin.Forms.Controls.GalleryPages.BindableMap
{
	public partial class BindableMapPage : ContentPage
	{
		public BindableMapPage ()
		{
			InitializeComponent ();

			BindingContext = new BindableMapViewModel();
		}
	}
}