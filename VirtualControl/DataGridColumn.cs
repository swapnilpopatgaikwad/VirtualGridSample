using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using VirtualGridSample.Extension;

namespace VirtualGridSample.VirtualControl
{
    public sealed class DataGridColumn : BindableObject, IDefinition
    {

        public static readonly BindableProperty WidthProperty = 
            BindableProperty.Create(nameof(Width),
                typeof(GridLength) ,
                typeof(DataGridColumn), 
                GridLength.Star,
                propertyChanged: WidthPropertyChanged);

        private static void WidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!oldValue.Equals(newValue) && bindable is DataGridColumn self)
            {
                if (self.ColumnDefinition == null)
                {
                    self.ColumnDefinition = new((GridLength)newValue);
                }
                else
                {
                    self.ColumnDefinition.Width = (GridLength)newValue;
                }

                self.OnSizeChanged();
            }
        }

        [TypeConverter(typeof(GridLengthTypeConverter))]
        public GridLength Width
        {
            get => (GridLength)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string) , typeof(DataGridColumn), string.Empty);
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty PropertyNameProperty = BindableProperty.Create(nameof(PropertyName), typeof(string) , typeof(DataGridColumn), string.Empty);
        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        public static readonly BindableProperty ColumnCellTemplateProperty = BindableProperty.Create(nameof(ColumnCellTemplate), typeof(DataTemplate) , typeof(DataGridColumn), default);
        public DataTemplate ColumnCellTemplate
        {
            get => (DataTemplate)GetValue(ColumnCellTemplateProperty);
            set => SetValue(ColumnCellTemplateProperty, value);
        }


        private readonly WeakEventManager _sizeChangedEventManager = new();

        private ColumnDefinition? _columnDefinition;

        public DataGridColumn()
        {
        }


        public event EventHandler SizeChanged
        {
            add => _sizeChangedEventManager.AddEventHandler(value);
            remove => _sizeChangedEventManager.RemoveEventHandler(value);
        }

        private void OnSizeChanged()
        {
            _sizeChangedEventManager.HandleEvent(this, EventArgs.Empty, nameof(SizeChanged));
        }


        internal Label HeaderLabel { get; } = new();

        internal Type? DataType { get; private set; }

        internal VirtualGrid? VirtualGrid { get; set; }

        internal ColumnDefinition? ColumnDefinition
        {
            get => _columnDefinition;
            set => _columnDefinition = value;
        }

        //internal DataGridCell? HeaderCell { get; set; }



        internal void InitializeDataType()
        {
            if (DataType != null || string.IsNullOrEmpty(PropertyName))
            {
                return;
            }

            ArgumentNullException.ThrowIfNull(VirtualGrid);

            if (VirtualGrid.GridItemSource == null)
            {
                return;
            }

            try
            {
                Type? rowDataType = null;

                var genericArguments = VirtualGrid.GridItemSource.GetType().GetGenericArguments();

                if (genericArguments.Length == 1)
                {
                    rowDataType = genericArguments[0];
                }
                else
                {
                    var firstItem = VirtualGrid.GridItemSource.OfType<object>().FirstOrDefault(i => i != null);
                    if (firstItem != default)
                    {
                        rowDataType = firstItem.GetType();
                    }
                }

                DataType = rowDataType?.GetPropertyTypeByPath(PropertyName);
            }
            catch (Exception ex)
                when (ex is NotSupportedException or ArgumentNullException or InvalidCastException)
            {
                Debug.WriteLine($"Attempting to obtain the data type for the column '{Title}' resulted in the following error: {ex.Message}");
            }
        }
    }
}
