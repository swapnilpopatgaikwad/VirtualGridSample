using System.Collections.ObjectModel;

namespace VirtualGridSample.VirtualControl
{
    public class VirtualizeListViewItem
    {
        protected VirtualizeItemsLayoutManger LayoutManager { get; set; }

        public VirtualizeListViewItem(VirtualizeItemsLayoutManger layoutManager)
        {
            LayoutManager = layoutManager;
        }

        private void Cell_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(var item in _cell)
            {
                if (item.Item==null)
                {
                    item.Item = this;
                }
            }
        }

        private ObservableCollection<CellHolder> _cell;

        public int Position { get; set; } = -1;
        public virtual bool IsOnScreen => IntersectsWithScrollVisibleRect();
        public bool IsAttached { get; set; }
        public bool IsCached { get; set; }
        public bool PendingSizeChange { get; set; }
        public List<DataTemplate> Template { get; set; }
        public double ColumnItemTemplateWidth { get; set; }
        public object BindingContext { get; set; }
        public ObservableCollection<CellHolder>? Cell
        {
            get => _cell;
            set
            {

                _cell = value;
                if(_cell!=null)
                {
                    _cell.CollectionChanged -= Cell_CollectionChanged;
                    _cell.CollectionChanged += Cell_CollectionChanged;
                }

            }
        }
        public Rect CellBounds { get; set; }
        public Rect Bounds { get; set; }
        public Thickness Margin { get; set; }

        protected virtual bool IntersectsWithScrollVisibleRect()
        {
            var control = LayoutManager.Control;

            var itemBoundsWithCollectionPadding = new Rect(
                CellBounds.X + control.Padding.Left,
                CellBounds.Y + control.Padding.Top,
                CellBounds.Width,
                CellBounds.Height);

            var visibleRect = new Rect(control.ScrollX, control.ScrollY, control.Width, control.Height);

            return itemBoundsWithCollectionPadding.IntersectsWith(visibleRect);
        }
    }
}
