using System;
using System.Collections.Generic;
using Xamarin.Forms.Core;

namespace Xamarin.Forms.Controls
{
	internal class RepeaterGalleryPage : ContentPage
    {
		public RepeaterGalleryPage()
		{
			var stackLayout = new StackLayout();
			Content = stackLayout;

			var buttonChangeList = new Button() { Text = "Reorder Items" };
			buttonChangeList.Clicked += ButtonChangeList_Clicked;
			stackLayout.Children.Add(buttonChangeList);

			_items = new ObservableList<RepeaterValue>
				{ new RepeaterValue(1), new RepeaterValue2(2), new RepeaterValue(3) };

			var noTemplateLabel = new Label() { Text = "No Template", FontAttributes = FontAttributes.Bold };
			stackLayout.Children.Add(noTemplateLabel);
			var noTemplateRepeater = new Repeater();
			stackLayout.Children.Add(noTemplateRepeater);		
			noTemplateRepeater.ItemsSource = _items;

			var templatedLabel = new Label() { Text = "Templated", FontAttributes = FontAttributes.Bold };
			stackLayout.Children.Add(templatedLabel);
			var templatedRepeater = new Repeater();
			stackLayout.Children.Add(templatedRepeater);
			templatedRepeater.ItemTemplate = new DataTemplate(typeof(RepeaterTemplate));
			templatedRepeater.ItemsSource = _items;

			var selectorLabel = new Label() { Text = "Selector", FontAttributes = FontAttributes.Bold };
			stackLayout.Children.Add(selectorLabel);
			var selectorRepeater = new Repeater();
			stackLayout.Children.Add(selectorRepeater);
			selectorRepeater.ItemTemplate = new RepeaterDataTemplateSelector();
			selectorRepeater.ItemsSource = _items;
		}

		ObservableList<RepeaterValue> _items;

		private void ButtonChangeList_Clicked(object sender, EventArgs e)
		{
			var middleItem = _items[1];
			_items.Remove(middleItem);
			_items.Add(middleItem);
		}

		class RepeaterValue
		{
			public RepeaterValue(int value)
			{
				Text = $"Item {value}";
				Value = value;
			}

			public string Text { get; set; }
			public int Value { get; set; }

			public override string ToString()
			{
				return Text;
			}
		}

		class RepeaterValue2 : RepeaterValue
		{
			public RepeaterValue2(int value) : base(value)
			{
			}
		}

		class RepeaterTemplate : Label
		{
			public RepeaterTemplate()
			{
				TextColor = Color.Red;
				this.SetBinding(TextProperty, "Text");
			}
		}

		class RepeaterTemplate2 : Label
		{
			public RepeaterTemplate2()
			{
				TextColor = Color.Blue;
				this.SetBinding(TextProperty, "Text");
			}
		}

		class RepeaterDataTemplateSelector : DataTemplateSelector
		{
			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			{
				switch(item)
				{
					case RepeaterValue2 v2:
						return new DataTemplate(typeof(RepeaterTemplate2));
					case RepeaterValue v1:
						return new DataTemplate(typeof(RepeaterTemplate));
					default:
						throw new Exception();
				}
			}
		}
	}
}
