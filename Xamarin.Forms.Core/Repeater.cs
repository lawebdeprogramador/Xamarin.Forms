using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Xamarin.Forms.Core
{
	public class Repeater : StackLayout
    {
		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(Repeater), null, propertyChanged: SourceChanged);

		public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(Repeater), null, propertyChanged: ItemTemplateChanged);

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}
		
		public DataTemplate ItemTemplate
		{
			get => (DataTemplate)GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}
		
		static void ItemTemplateChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			var list = bindable as Repeater;
			list?.BuildItems();
		}

		static void SourceChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (!(bindable is Repeater list)) throw new ArgumentNullException(nameof(list));

			if (oldvalue is INotifyCollectionChanged oldCollectionChanged)
				oldCollectionChanged.CollectionChanged -= list.ItemsSource_CollectionChanged;

			if (newvalue is INotifyCollectionChanged newCollectionChanged)
				newCollectionChanged.CollectionChanged += list.ItemsSource_CollectionChanged;

			list.BuildItems();
		}

		void BuildItems()
		{
			if (ItemsSource == null) return;

			//Clear any existing items
			Children.Clear();

			foreach (var item in ItemsSource)
				Children.Add(CreateTemplateForItem(item, ItemTemplate));
		}
		void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			//Insert new views
			var startingInsertIndex = e.NewStartingIndex;
			if (e.NewItems != null)
			{
				foreach (var newItem in e.NewItems)
				{
					var view = CreateTemplateForItem(newItem, ItemTemplate);
					Children.Insert(startingInsertIndex, view);
					startingInsertIndex++;
				}
			}

			if (e.OldItems == null) return;

			//Remove any views no longer needed
			foreach (var oldItem in e.OldItems)
			{
				var viewsToRemove = Children.Where(c => c.BindingContext == oldItem).ToList();
				foreach (var viewToRemove in viewsToRemove)
					Children.Remove(viewToRemove);
			}
		}

		View CreateTemplateForItem(object item, DataTemplate itemTemplate, bool createDefaultIfNoTemplate = true)
		{
			DataTemplate templateToUse;

			//Check to see if we have a template selector or just a template
			var templateSelector = itemTemplate as DataTemplateSelector;
			templateToUse = templateSelector != null ? templateSelector.SelectTemplate(item, null) : itemTemplate;

			//If we still don't have a template, create a label
			if (templateToUse == null)
				return createDefaultIfNoTemplate ? new Label() { Text = item.ToString(), BindingContext = item } : null;

			//Create the content
			var view = templateToUse.CreateContent() as View;

			//If a view wasn't created, we can't use it, exit
			if (view == null) return new Label() { Text = item.ToString() }; ;

			//Set the binding
			view.BindingContext = item;

			return view;
		}
	}
}
