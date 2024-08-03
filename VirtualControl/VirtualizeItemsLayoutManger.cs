using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace VirtualGridSample.VirtualControl
{
    public abstract class VirtualizeItemsLayoutManger : Grid, IDisposable
    {
        public VirtualGrid? Control { get; protected set; }

        public bool IsDisposed { get; protected set; }
        protected List<VirtualizeListViewItem> LaidOutItems { get; } = [];
        protected List<VirtualizeListViewItem> CachedItems { get; } = [];

        //public List<(object Data, int Position)> VisibleItems => LaidOutItems.FindAll(i => i.IsOnScreen && i.IsAttached && i.Cell?.Children[0] is VirtualizeListViewCell).Select(i => (i.BindingContext, i.Position)).ToList();

        protected virtual Size AvailableSpace => GetAvailableSpace();
        protected Size PrevAvailableSpace { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed) return;

            UnsubscribeFromEvents();

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~VirtualizeItemsLayoutManger()
        {
            Dispose(false);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return new ItemsLayoutManager(this);
        }

        protected virtual bool IsOrientation(ScrollOrientation orientation)
        {
            return Control!.Orientation == orientation
                || (Control.Orientation == ScrollOrientation.Neither && Control.PrevScrollOrientation == orientation);
        }

        protected override void OnParentChanging(ParentChangingEventArgs args)
        {
            base.OnParentChanging(args);

            UnsubscribeFromEvents();

            Control = null;
        }

        protected override void OnParentChanged()
        {
            base.OnParentChanged();

            if (this.Parent is null) return;

            if (this.Parent is not VirtualGrid listView)
            {
                throw new InvalidOperationException("ItemsLayoutManager can be used only within VirtualizeListView");
            }

            Control = listView;

            SubscribeToEvents();
        }

        protected virtual void UnsubscribeFromEvents()
        {
            if (Control is null) return;

            OnAdapterReset();

            Control.PropertyChanging -= Control_PropertyChanging;
            Control.PropertyChanged -= Control_PropertyChanged;
            Control.SizeChanged -= Control_SizeChanged;
            Control.Scrolled -= Control_Scrolled;
        }

        protected virtual void SubscribeToEvents()
        {
            if (Control is null) return;

            Control.PropertyChanging += Control_PropertyChanging;
            Control.PropertyChanged += Control_PropertyChanged;
            Control.SizeChanged += Control_SizeChanged;
            Control.Scrolled += Control_Scrolled;

            OnAdapterSet();
        }

        private void Control_Scrolled(object? sender, ScrolledEventArgs e)
        {
        }

        private void Control_SizeChanged(object? sender, EventArgs e)
        {
            var newSpace = GetAvailableSpace();

            if (newSpace == PrevAvailableSpace) return;

            PrevAvailableSpace = GetAvailableSpace();

            if (LaidOutItems.Count == 0)
            {
                InvalidateLayout();
                return;
            }
            else
            {
                //RelayoutItems();
                //PendingAdjustScroll = true;
            }
        }

        private void Control_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void Control_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
        }

        protected virtual bool DoesScrollHaveSize()
        {
            return Control is not null && !double.IsNaN(Control.Width) && !double.IsNaN(Control.Height) && Control.Width >= 0d && Control.Height >= 0d;
        }

        protected virtual void OnAdapterSet()
        {
            if (Control?.Adapter is null) return;

            Control.Adapter.DataSetChanged += AdapterDataSetChanged;
            Control.Adapter.ItemMoved += AdapterItemMoved;
            Control.Adapter.ItemRangeChanged += AdapterItemRangeChanged;
            Control.Adapter.ItemRangeInserted += AdapterItemRangeInserted;
            Control.Adapter.ItemRangeRemoved += AdapterItemRangeRemoved;

            InvalidateLayout();
        }

        private void AdapterItemRangeRemoved(object? sender, (int startingIndex, int totalCount) e)
        {
        }

        private void AdapterItemRangeInserted(object? sender, (int startingIndex, int totalCount) e)
        {
        }

        private void AdapterItemRangeChanged(object? sender, (int startingIndex, int oldCount, int newCount) e)
        {
        }

        private void AdapterItemMoved(object? sender, (int oldIndex, int newIndex) e)
        {
        }

        private void AdapterDataSetChanged(object? sender, EventArgs e)
        {
            if (Control!.Adapter.ItemsCount == 0 && LaidOutItems.Count == 0)
            {
                return;
            }
            else if (LaidOutItems.Count == 0 && CachedItems.Count == 0)
            {
                InvalidateLayout();
            }
            else if (Control!.Adapter.ItemsCount == 0)
            {
                AdapterItemRangeRemoved(this, (0, LaidOutItems.Count));
            }
            else if (LaidOutItems.Count > 0)
            {
                AdapterItemRangeChanged(this, (0, LaidOutItems.Count, Control.Adapter.ItemsCount));
                if (Control.ScrollX == 0d && Control.ScrollY == 0d) return;
                Control.ScrollToAsync(0, 0, false);
            }
            else if (LaidOutItems.Count == 0)
            {
                AdapterItemRangeInserted(this, (0, Control.Adapter.ItemsCount));
            }
        }

        protected virtual void OnAdapterReset()
        {
            if (Control?.Adapter is null) return;

            Control.Adapter.DataSetChanged -= AdapterDataSetChanged;
            Control.Adapter.ItemMoved -= AdapterItemMoved;
            Control.Adapter.ItemRangeChanged -= AdapterItemRangeChanged;
            Control.Adapter.ItemRangeInserted -= AdapterItemRangeInserted;
            Control.Adapter.ItemRangeRemoved -= AdapterItemRangeRemoved;
        }

        protected virtual Size GetAvailableSpace()
        {
            var widthCell = Control.Columns.Sum(x => x.Width.Value);
            return new Size(widthCell - Control.Padding.HorizontalThickness, Control.Height - Control.Padding.VerticalThickness);
        }

        protected const double AutoSize = -1d;
        protected virtual void ResizeLayout()
        {
            if (LaidOutItems.Count == 0)
            {
                this.WidthRequest = AutoSize;
                this.HeightRequest = AutoSize;
                return;
            }

            var control = Control!;

            var lastItem = LaidOutItems[^1];

            var widthCell = lastItem.ColumnItemTemplateWidth;

            Size newSize = new(widthCell, lastItem.Bounds.Bottom);
            if (this.WidthRequest == newSize.Width && this.HeightRequest == newSize.Height) return;

            this.WidthRequest = newSize.Width;
            this.HeightRequest = newSize.Height;
        }
        public virtual void InvalidateLayout()
        {
            LaidOutItems.Clear();
            CachedItems.Clear();
            this.Clear();
            ResizeLayout();


            if (!DoesScrollHaveSize()) return;

            if (Control?.Adapter is null || Control.Adapter.Items.Count == 0) return;

            var dataItems = Control.Adapter.Items;
            var count = dataItems.Count;

            for (int i = 0; i < count; i++)
            {
                var item = CreateItemForPosition(dataItems, i);

                LaidOutItems.Add(item);

                var estimatedSize = GetEstimatedItemSize(item);
                item.CellBounds = item.Bounds = new(0d, 0d, estimatedSize.Width, estimatedSize.Height);

                ShiftAllItems(LaidOutItems, i, LaidOutItems.Count);

               // if (!item.IsOnScreen) continue;
                for (int j = 0; j < item.Template.Count; j++)
                {
                    var template = item.Template[j];
                    var _cell = Control.Adapter.OnCreateCell(template, item.Position);
                    if (item.Cell == null)
                    {
                        item.Cell = new ObservableCollection<CellHolder>();
                    }
                    item.Cell.Add(_cell);
                    this.Add(_cell, j, i);
                }

                Control.Adapter.OnBindCell(item.Cell, item.Position);

                ArrangeItem(LaidOutItems, item, AvailableSpace);

                item.IsAttached = true;
            }

            DrawAndResize();
        }

        protected virtual void DrawAndResize()
        {
//#if !ANDROID
//            foreach (var item in LaidOutItems.FindAll(i => i.Cell is not null))
//            {
//                DrawItem(LaidOutItems, item);
//            }
//#endif

            ResizeLayout();
        }

        protected void ArrangeItem(IReadOnlyList<VirtualizeListViewItem> items, VirtualizeListViewItem item, Size availableSpace)
        {
            var count = items.Count;

            if (IsOrientation(ScrollOrientation.Both)
                || count == 0 || item.Position == -1) return;

            var prevIndex = item.Position - 1;
            var prevItemBounds = prevIndex == -1 ? new() : items[prevIndex].Bounds;

            var margin = GetItemMargin(items, item);

            item.Margin = margin;

            if (IsOrientation(ScrollOrientation.Vertical))
            {
                var bottom = prevItemBounds.Bottom;

                var newAvailableSpace = new Size(availableSpace.Width - margin.HorizontalThickness, availableSpace.Height);

                var request = MeasureItem(items, item, newAvailableSpace);

                //if (item.Cell.Sum(x=> x.WidthRequest) != newAvailableSpace.Width
                //    || item.Cell.Sum(x => x.HeightRequest) != AutoSize)
                //{
                //    item.Cell!.WidthRequest = newAvailableSpace.Width;
                //    item.Cell.HeightRequest = AutoSize;
                //}

                item.CellBounds = new Rect(margin.Left, bottom + margin.Top, request.Width, request.Height);
                item.Bounds = new Rect(0d, bottom, request.Width, request.Height + margin.VerticalThickness);
            }
            else
            {
                var right = prevItemBounds.Right;

                var newAvailableSpace = new Size(availableSpace.Width, availableSpace.Height - margin.VerticalThickness);

                var request = MeasureItem(items, item, newAvailableSpace);

                //if (item.Cell!.WidthRequest != AutoSize
                //    || item.Cell.HeightRequest != newAvailableSpace.Height)
                //{
                //    item.Cell!.HeightRequest = newAvailableSpace.Height;
                //    item.Cell.WidthRequest = AutoSize;
                //}

                item.CellBounds = new Rect(right + margin.Left, margin.Top, request.Width, request.Height);
                item.Bounds = new Rect(right, 0d, request.Width + margin.HorizontalThickness, request.Height);
            }
        }


        protected  Size MeasureItem(IReadOnlyList<VirtualizeListViewItem> items, VirtualizeListViewItem item, Size availableSpace)
        {
            double _width = 0d;
            double _height = 0d;
            List<Size> cellSizes = new List<Size>();
            for (int i = 0; i < item.Cell.Count; i++)
            {
                var iview = (item.Cell[i] as IView)!;
                var size1 = iview.Measure(availableSpace.Width, double.PositiveInfinity);
                cellSizes.Add(size1);
            }
            _width = cellSizes.Sum(x => x.Width);
            _height = cellSizes.Max(x => x.Height);
            return new Size(_width, _height); 
        }
        protected Thickness GetItemMargin(IReadOnlyList<VirtualizeListViewItem> items, VirtualizeListViewItem item)
        {
            return new Thickness();
        }

        protected  Size GetEstimatedItemSize(VirtualizeListViewItem item)
        {
            if (IsOrientation(ScrollOrientation.Both) || item.Position < 0) return new();

            if (IsOrientation(ScrollOrientation.Vertical))
            {
                return new Size(AvailableSpace.Width, 200d);
            }
            else
            {
                return new Size(200d, AvailableSpace.Height);
            }
        }

        protected  void ShiftAllItems(IReadOnlyList<VirtualizeListViewItem> items, int start, int exclusiveEnd)
        {
            var count = items.Count;

            if (IsOrientation(ScrollOrientation.Both) || start < 0
                || start >= count || exclusiveEnd <= 0 || exclusiveEnd > count) return;

            var item = items[start];
            var prevIndex = start - 1;
            var prevBounds = prevIndex == -1 ? new() : items[prevIndex].Bounds;

            if (IsOrientation(ScrollOrientation.Vertical))
            {
                var dy = prevBounds.Bottom - item.Bounds.Y;
                if (dy == 0d) return;

                for (int i = start; i < exclusiveEnd; i++)
                {
                    var currentItem = items[i];

                    currentItem.CellBounds = new Rect(currentItem.CellBounds.X, currentItem.CellBounds.Y + dy, currentItem.CellBounds.Width, currentItem.CellBounds.Height);
                    currentItem.Bounds = new Rect(currentItem.Bounds.X, currentItem.Bounds.Y + dy, currentItem.Bounds.Width, currentItem.Bounds.Height);
                }
            }
            else
            {
                var dx = prevBounds.Right - item.Bounds.X;
                if (dx == 0d) return;

                for (int i = start; i < exclusiveEnd; i++)
                {
                    var currentItem = items[i];
                    currentItem.CellBounds = new Rect(currentItem.CellBounds.X + dx, currentItem.CellBounds.Y, currentItem.CellBounds.Width, currentItem.CellBounds.Height);
                    currentItem.Bounds = new Rect(currentItem.Bounds.X + dx, currentItem.Bounds.Y, currentItem.Bounds.Width, currentItem.Bounds.Height);
                }
            }
        }

        protected virtual VirtualizeListViewItem CreateItemForPosition(IReadOnlyList<object> dataItems, int position)
        {
            return new VirtualizeListViewItem(this)
            {
                BindingContext = dataItems[position],
                Template = Control!.Adapter.GetTemplate(position),
                ColumnItemTemplateWidth = Control!.Adapter.GetColumnsItemTemplate(),
                Position = position
            };
        }
    }
}
