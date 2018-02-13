using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[ContentProperty("Text")]
	[RenderWith(typeof(_LabelRenderer))]
	public class Label : View, IFontElement, ITextElement, ITextAlignmentElement, IElementConfiguration<Label>
	{
		public static readonly BindableProperty HorizontalTextAlignmentProperty = TextAlignmentElement.HorizontalTextAlignmentProperty;

		[Obsolete("XAlignProperty is obsolete as of version 2.0.0. Please use HorizontalTextAlignmentProperty instead.")]
		public static readonly BindableProperty XAlignProperty = HorizontalTextAlignmentProperty;

		public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create("VerticalTextAlignment", typeof(TextAlignment), typeof(Label), TextAlignment.Start,
			propertyChanged: OnVerticalTextAlignmentPropertyChanged);

		[Obsolete("YAlignProperty is obsolete as of version 2.0.0. Please use VerticalTextAlignmentProperty instead.")]
		public static readonly BindableProperty YAlignProperty = VerticalTextAlignmentProperty;

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public static readonly BindableProperty FontProperty = FontElement.FontProperty;

		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(Label), default(string), propertyChanged: OnTextPropertyChanged);

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		public static readonly BindableProperty FormattedTextProperty = BindableProperty.Create("FormattedText", typeof(FormattedString), typeof(Label), default(FormattedString),
			propertyChanging: (bindable, oldvalue, newvalue) =>
			{
				if (oldvalue != null)
				{
					var formattedString = ((FormattedString)oldvalue);
					formattedString.Parent = null;
					formattedString.PropertyChanged -= ((Label)bindable).OnFormattedTextChanged;
					SetInheritedBindingContext(formattedString, null);
					((ObservableCollection<Span>)formattedString.Spans).CollectionChanged -= ((Label)bindable).Span_CollectionChanged;
				}
			}, propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				if (newvalue != null)
				{
					var label = ((Label)bindable);
					var formattedString = (FormattedString)newvalue;
					formattedString.Parent = label;
					formattedString.PropertyChanged += label.OnFormattedTextChanged;
					SetInheritedBindingContext(formattedString, bindable.BindingContext);
					((ObservableCollection<Span>)formattedString.Spans).CollectionChanged += label.Span_CollectionChanged;

					// Initial Load of FormattedText could come preloaded with spans
					foreach (var span in formattedString.Spans)
						foreach (var recognizer in span.GestureRecognizers)
							((IGestureElement)label).CompositeGestureRecognizers.Add(new SpanGestureRecognizer() { GestureRecognizer = recognizer });
				}

				((Label)bindable).InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
				if (newvalue != null)
					((Label)bindable).Text = null;
			});

		public static readonly BindableProperty LineBreakModeProperty = BindableProperty.Create("LineBreakMode", typeof(LineBreakMode), typeof(Label), LineBreakMode.WordWrap,
			propertyChanged: (bindable, oldvalue, newvalue) => ((Label)bindable).InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged));

		readonly Lazy<PlatformConfigurationRegistry<Label>> _platformConfigurationRegistry;

		public Label()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Label>>(() => new PlatformConfigurationRegistry<Label>(this));
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			if (FormattedText != null)
				SetInheritedBindingContext(FormattedText, this.BindingContext);
		}

		[Obsolete("Font is obsolete as of version 1.3.0. Please use the Font attributes which are on the class itself.")]
		public Font Font
		{
			get { return (Font)GetValue(FontProperty); }
			set { SetValue(FontProperty, value); }
		}

		public FormattedString FormattedText
		{
			get { return (FormattedString)GetValue(FormattedTextProperty); }
			set { SetValue(FormattedTextProperty, value); }
		}

		public TextAlignment HorizontalTextAlignment
		{
			get { return (TextAlignment)GetValue(TextAlignmentElement.HorizontalTextAlignmentProperty); }
			set { SetValue(TextAlignmentElement.HorizontalTextAlignmentProperty, value); }
		}

		public LineBreakMode LineBreakMode
		{
			get { return (LineBreakMode)GetValue(LineBreakModeProperty); }
			set { SetValue(LineBreakModeProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		public TextAlignment VerticalTextAlignment
		{
			get { return (TextAlignment)GetValue(VerticalTextAlignmentProperty); }
			set { SetValue(VerticalTextAlignmentProperty, value); }
		}

		[Obsolete("XAlign is obsolete as of version 2.0.0. Please use HorizontalTextAlignment instead.")]
		public TextAlignment XAlign
		{
			get { return (TextAlignment)GetValue(XAlignProperty); }
			set { SetValue(XAlignProperty, value); }
		}

		[Obsolete("YAlign is obsolete as of version 2.0.0. Please use VerticalTextAlignment instead.")]
		public TextAlignment YAlign
		{
			get { return (TextAlignment)GetValue(YAlignProperty); }
			set { SetValue(YAlignProperty, value); }
		}

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, (Label)this);

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontChanged(Font oldValue, Font newValue) =>
			 InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void OnFormattedTextChanged(object sender, PropertyChangedEventArgs e)
		{
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
			OnPropertyChanged("FormattedText");
		}

		void Span_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{			
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (IGestureChildElement span in e.NewItems)
					{
						((ObservableCollection<IGestureRecognizer>)span.GestureRecognizers).CollectionChanged += Span_GestureRecognizer_CollectionChanged;
						// Span could be preloaded with GestureRecognizers
						foreach (var recognizer in ((ObservableCollection<IGestureRecognizer>)span.GestureRecognizers))
							((IGestureElement)this).CompositeGestureRecognizers.Add(new SpanGestureRecognizer() { GestureRecognizer = recognizer });
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (IGestureChildElement span in e.OldItems)
						((ObservableCollection<IGestureRecognizer>)span.GestureRecognizers).CollectionChanged -= Span_GestureRecognizer_CollectionChanged;
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (IGestureChildElement span in e.NewItems)
					{
						((ObservableCollection<IGestureRecognizer>)span.GestureRecognizers).CollectionChanged += Span_GestureRecognizer_CollectionChanged;
						// Span could be preloaded with GestureRecognizers
						foreach (var recognizer in ((ObservableCollection<IGestureRecognizer>)span.GestureRecognizers))
							((IGestureElement)this).CompositeGestureRecognizers.Add(new SpanGestureRecognizer() { GestureRecognizer = recognizer });
					}

					foreach (IGestureChildElement span in e.OldItems)
						((ObservableCollection<IGestureRecognizer>)span.GestureRecognizers).CollectionChanged -= Span_GestureRecognizer_CollectionChanged;
					break;
				case NotifyCollectionChangedAction.Reset:
					//TODO: How to remove all existing elements from this span only
					break;
			}
		}

		private void Span_GestureRecognizer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (IGestureRecognizer recognizer in e.NewItems)
						((IGestureElement)this).CompositeGestureRecognizers.Add(new SpanGestureRecognizer() { GestureRecognizer = recognizer });
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (IGestureRecognizer recognizer in e.OldItems)
						foreach (SpanGestureRecognizer spanRecognizer in ((IGestureElement)this).CompositeGestureRecognizers.ToList())
							if (spanRecognizer == recognizer)
								((IGestureElement)this).CompositeGestureRecognizers.Remove(spanRecognizer);
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (IGestureRecognizer recognizer in e.NewItems)
						((IGestureElement)this).CompositeGestureRecognizers.Add(new SpanGestureRecognizer() { GestureRecognizer = recognizer });

					foreach (IGestureRecognizer recognizer in e.OldItems)
						foreach (SpanGestureRecognizer spanRecognizer in ((IGestureElement)this).CompositeGestureRecognizers.ToList())
							if (spanRecognizer == recognizer)
								((IGestureElement)this).CompositeGestureRecognizers.Remove(spanRecognizer);
					break;
				case NotifyCollectionChangedAction.Reset:
					//TODO: How to remove all existing elements from this span only
					break;
			}
			
		}

		void ITextAlignmentElement.OnHorizontalTextAlignmentPropertyChanged(TextAlignment oldValue, TextAlignment newValue)
		{
#pragma warning disable 0618 // retain until XAlign removed
			OnPropertyChanged(nameof(XAlign));
#pragma warning restore
		}

		static void OnTextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			var label = (Label)bindable;
			LineBreakMode breakMode = label.LineBreakMode;
			bool isVerticallyFixed = (label.Constraint & LayoutConstraint.VerticallyFixed) != 0;
			bool isSingleLine = !(breakMode == LineBreakMode.CharacterWrap || breakMode == LineBreakMode.WordWrap);
			if (!isVerticallyFixed || !isSingleLine)
				((Label)bindable).InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);
			if (newvalue != null)
				((Label)bindable).FormattedText = null;
		}

		static void OnVerticalTextAlignmentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var label = (Label)bindable;
#pragma warning disable 0618 // retain until YAlign removed
			label.OnPropertyChanged(nameof(YAlign));
#pragma warning restore 0618
		}

		public IPlatformElementConfiguration<T, Label> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		public override IList<IGestureChildElement> ChildElementOverrides(Point point)
		{
			var elements = new List<IGestureChildElement>();

			if (FormattedText?.Spans == null || FormattedText?.Spans.Count == 0)
				return elements;

			foreach (var span in FormattedText.Spans)
				for (int i = 0; i < span.Positions.Count; i++)
					if (span.Positions[i].Contains(point.X, point.Y))
						elements.Add(span);

			return elements;
		}
	}
}