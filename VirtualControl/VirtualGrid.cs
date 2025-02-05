﻿using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using VirtualGridSample.Extension;

namespace VirtualGridSample.VirtualControl
{
    public interface IItemsLayout
    {
        int PoolSize { get; set; }
    }

    public class ItemsLayout : IItemsLayout
    {
        public int PoolSize { get; set; } = 4;
    }

    public class LinearLayout : ItemsLayout
    {
        public double ItemSpacing { get; set; }
    }
    public class VirtualGrid : ScrollView
    {
        public PropertyInfo[] Properties { get; set; }
        public ScrollOrientation PrevScrollOrientation { get; protected set; }
        public VirtualGrid()
        {
            PrevScrollOrientation = Orientation != ScrollOrientation.Neither ? Orientation : ScrollOrientation.Vertical;
            Orientation = ScrollOrientation.Both;
            VerticalScrollBarVisibility = ScrollBarVisibility.Always;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Always;
            Adapter = new DataAdapter(this);

            OnItemsLayoutSet();
        }
        protected virtual void OnItemsLayoutSet()
        {
            if (ItemsLayout is LinearLayout linearLayout)
            {
                LayoutManager = new LinearItemsLayoutManager()
                {
                    BindingContext = null
                };
            }
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
                typeof(VirtualGrid), new ObservableRangeCollection<DataGridColumn>(), propertyChanged: ColumnsPropertyChanged);

        private static void ColumnsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
        }

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
        public IItemsLayout ItemsLayout
        {
            get { return (IItemsLayout)GetValue(ItemsLayoutProperty); }
            set { SetValue(ItemsLayoutProperty, value); }
        }

        public static readonly BindableProperty ItemsLayoutProperty =
            BindableProperty.Create(
                nameof(ItemsLayout),
                typeof(IItemsLayout),
                typeof(VirtualGrid),
                new LinearLayout());

        public VirtualizeItemsLayoutManger LayoutManager
        {
            get { return (VirtualizeItemsLayoutManger)GetValue(LayoutManagerProperty); }
            protected set { SetValue(LayoutManagerProperty, value); }
        }

        public static readonly BindableProperty LayoutManagerProperty =
            BindableProperty.Create(
                nameof(LayoutManager),
                typeof(VirtualizeItemsLayoutManger),
                typeof(VirtualGrid));


        private static void OnGridItemSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (VirtualGrid)bindable;
            control.HandleGridItemSourceChanged();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == ColumnsProperty.PropertyName)
            {
                LayoutManager?.InvalidateLayout();
            }
            else if (propertyName == LayoutManagerProperty.PropertyName)
            {
                this.Content = LayoutManager;
            }
            else if (propertyName == GridItemSourceProperty.PropertyName)
            {
                Adapter?.ReloadData();
            }
        }

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
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
