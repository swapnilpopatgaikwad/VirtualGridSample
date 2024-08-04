namespace VirtualGridSample.VirtualControl
{
    public class VirtualizeListViewItem
    {
        public VirtualizeListViewItem() 
        {
        }

        private CellHolder _cell;
        public CellHolder? Cell
        {
            get => _cell;
            set
            {
                if (value is null && _cell is not null)
                {
                    _cell.Item = null;
                }

                _cell = value;

                if (_cell is not null)
                {
                    _cell.Item = this;
                }
            }
        }
    }
}
