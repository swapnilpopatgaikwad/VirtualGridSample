using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using VirtualGridSample.Extension;

namespace VirtualGridSample.VirtualControl
{
    public class VirtualGrid : Grid
    {
        public PropertyInfo[] Properties { get; set; }
        public VirtualGrid()
        {
            Adapter = new DataAdapter(this);
            Render();
        }

        private void Render()
        {
            Adapter?.OnAdapterSet();
        }

        public Type PropertyEnum
        {
            get { return (Type)GetValue(PropertyEnumProperty); }
            set { SetValue(PropertyEnumProperty, value); }
        }

        public static readonly BindableProperty PropertyEnumProperty =
            BindableProperty.Create(
                nameof(PropertyEnum),
                typeof(Type),
                typeof(VirtualGrid),
                default(Type),
                propertyChanged: OnPropertyEnumChanged);

        private static void OnPropertyEnumChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (VirtualGrid)bindable;
            Enum newEnumValue = (Enum)newValue;
        }

        public IEnumerable GridItemSource
        {
            get { return (IEnumerable)GetValue(GridItemSourceProperty); }
            set { SetValue(GridItemSourceProperty, value); }
        }

        public static readonly BindableProperty GridItemSourceProperty =
            BindableProperty.Create(
                nameof(GridItemSource),
                typeof(IEnumerable),
                typeof(VirtualGrid),
                null,
            propertyChanged: OnGridItemSourceChanged);


        public ObservableRangeCollection<DataGridColumn> Columns
        {
            get => (ObservableRangeCollection<DataGridColumn>)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public static readonly BindableProperty ColumnsProperty =
        BindableProperty.Create(
                nameof(Columns),
                typeof(ObservableRangeCollection<DataGridColumn>),
                typeof(VirtualGrid), new ObservableRangeCollection<DataGridColumn>());

        public DataAdapter Adapter
        {
            get { return (DataAdapter)GetValue(AdapterProperty); }
            protected set { SetValue(AdapterProperty, value); }
        }

        public static readonly BindableProperty AdapterProperty =
            BindableProperty.Create(
                nameof(Adapter),
                typeof(DataAdapter),
                typeof(VirtualGrid));


        private static void OnGridItemSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (VirtualGrid)bindable;
            control.HandleGridItemSourceChanged();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == GridItemSourceProperty.PropertyName)
            {
                Adapter?.ReloadData();
            }
            else if (propertyName == ColumnProperty.PropertyName)
            {
                Adapter?.BuildColumns();
            }
        }

        public void HandleGridItemSourceChanged()
        {
            if (GridItemSource == null)
            {
                return;
            }

            // Get the enumerator of the IEnumerable
            var enumerator = GridItemSource.GetEnumerator();

            // Move to the first element
            if (enumerator.MoveNext())
            {
                var firstItem = enumerator.Current;

                if (firstItem != null)
                {
                    Type type = firstItem.GetType();
                    Properties = type.GetProperties();

                    // Process the properties as needed
                    foreach (var property in Properties)
                    {
                        // For example, print the property names
                        System.Diagnostics.Debug.WriteLine(property.Name);
                    }
                }
            }
        }
    }
}
