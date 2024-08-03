using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace VirtualGridSample.VirtualControl
{
    public class DataAdapter : IDisposable
    {
        public class AdapterItem(object data)
        {
            public object Data { get; } = data;
        }
        protected VirtualGrid Control { get; set; }
        protected List<AdapterItem> InternalItems { get; set; } = [];

        public IReadOnlyList<AdapterItem> Items => InternalItems;

        public virtual int ItemsCount => InternalItems?.Count ?? 0;

        public event EventHandler DataSetChanged;
        public event EventHandler<(int startingIndex, int totalCount)> ItemRangeInserted;
        public event EventHandler<(int startingIndex, int totalCount)> ItemRangeRemoved;
        public event EventHandler<(int startingIndex, int oldCount, int newCount)> ItemRangeChanged;
        public event EventHandler<(int oldIndex, int newIndex)> ItemMoved;

        public DataAdapter(VirtualGrid listView)
        {
            Control = listView;

            Control.PropertyChanged += Control_PropertyChanged;
        }

        private void Control_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public virtual void InitCollection(IEnumerable? itemsSource)
        {
            if (itemsSource is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += ItemsSourceCollectionChanged;
            }

            OnCollectionChangedReset(itemsSource);
        }

        public void ReloadData()
        {
            var itemsSource = Control?.GridItemSource;

            RemoveListenerCollection(itemsSource);

            InitCollection(itemsSource);
        }

        protected virtual void OnCollectionChangedReset(IEnumerable? itemsSource)
        {
            List<AdapterItem> items = itemsSource is null ? [] : new(itemsSource.Cast<object>().Select(d => new AdapterItem(d)));

            InternalItems = items;

            NotifyDataSetChanged();
        }
        public virtual void NotifyDataSetChanged()
        {
            NotifyWrapper(() =>
            {
                DataSetChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        private void NotifyWrapper(Action notifyAction)
        {
            if (!MainThread.IsMainThread)
            {
                MainThread.BeginInvokeOnMainThread(Notify);
            }
            else Notify();

            void Notify()
            {
                if (IsDisposed)
                {
                    RemoveListenerCollection(Control.GridItemSource);
                }
                else
                {
                    notifyAction.Invoke();
                }
            }
        }
        protected virtual void RemoveListenerCollection(IEnumerable? itemsSource)
        {
            if (itemsSource is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= ItemsSourceCollectionChanged;
            }
        }

        private void ItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var itemsSource = sender as IEnumerable;

            if (IsDisposed)
            {
                RemoveListenerCollection(itemsSource);
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnCollectionChangedAdd(e);
                    break;
                //case NotifyCollectionChangedAction.Remove:
                //    OnCollectionChangedRemove(e);
                //    break;
                //case NotifyCollectionChangedAction.Replace:
                //    OnCollectionChangedReplace(e);
                //    break;
                //case NotifyCollectionChangedAction.Move:
                //    OnCollectionChangedMove(e);
                //    break;
                case NotifyCollectionChangedAction.Reset:
                    OnCollectionChangedReset(itemsSource);
                    break;
            }
        }

        protected virtual void OnCollectionChangedAdd(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems?.Count is null or 0) return;

            var index = e.NewStartingIndex;

            InternalItems.InsertRange(index, e.NewItems.Cast<object>().Select(d => new AdapterItem(d)));
            NotifyItemRangeInserted(index, e.NewItems.Count);
        }

        public virtual void NotifyItemRangeInserted(int startingIndex, int totalCount)
        {
            NotifyWrapper(() =>
            {
                ItemRangeInserted?.Invoke(this, (startingIndex, totalCount));
            });
        }

        #region IDisposable
        public bool IsDisposed { get; protected set; }
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed) return;

            Control.PropertyChanged -= Control_PropertyChanged;

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DataAdapter()
        {
            Dispose(false);
        }
        #endregion

        public virtual List<DataTemplate> GetTemplate(int position)
        {

            return GetItemTemplate(InternalItems[position]);
        }

        protected virtual List<DataTemplate> GetItemTemplate(object item)
        {
            return Control.Columns.Select(x=> x.ColumnCellTemplate).ToList();
        }
        public virtual double GetColumnsItemTemplate()
        {
            return Control.Columns.Sum(x=> x.Width.Value);
        }

        public virtual CellHolder OnCreateCell(DataTemplate template, int position)
        {
            var holder = CreateEmptyCellForTemplate(template);
            var content = holder[0];

            return holder;
        }

        protected virtual CellHolder CreateEmptyCellForTemplate(DataTemplate template)
        {
            var content = template.CreateContent() as Microsoft.Maui.Controls.View;
            var holder = new CellHolder()
        {
            /*Content =*/ content
        };
            return holder;
        }

        public virtual void OnBindCell(ObservableCollection<CellHolder> holders, int position)
        {
            var data = Items[position];

            for (int i = 0; i < holders.Count; i++)
            {
                holders[i].BindingContext = data;
            }
        }
    }
}
