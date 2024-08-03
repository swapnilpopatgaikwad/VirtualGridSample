using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGridSample.VirtualControl
{
    public class CellHolder : Grid
    {
        public VirtualizeListViewItem? Item { get; set; }

        public CellHolder()
        {
            VerticalOptions = HorizontalOptions = LayoutOptions.Start;

            this.SizeChanged += CellHolder_SizeChanged;
        }

        private void CellHolder_SizeChanged(object? sender, EventArgs e)
        {
            //Item?.OnCellSizeChanged();
        }
    }
}
