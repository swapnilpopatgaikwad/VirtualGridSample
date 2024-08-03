using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using VirtualGridSample.Model;
using VirtualGridSample.Extension;
using VirtualGridSample.VirtualControl;

namespace VirtualGridSample.ViewModel
{
    public partial class VirtualGridViewModel:ObservableObject
    {
        [ObservableProperty]
        public ObservableRangeCollection<VirtualGridModel> dataItems = [];
        [ObservableProperty]
        public ObservableRangeCollection<DataGridColumn> columns = []; 
        private static Random random = new Random();
        public VirtualGridViewModel()
        {
            BuildColumn();
            BuildDataItems();

        }

        private void BuildColumn()
        {
            var col=  new ObservableCollection<DataGridColumn>
        {
            new DataGridColumn
            {
                Title = "Logo1",
                PropertyName = "Logo1",
                Width = 100,
                ColumnCellTemplate = Application.Current.Resources["Logo1Template"] as DataTemplate
            },
            new DataGridColumn
            {
                Title = "Logo2",
                PropertyName = "Logo2",
                Width = 100,
                ColumnCellTemplate = Application.Current.Resources["Logo2Template"] as DataTemplate
            },
            new DataGridColumn
            {
                Title = "Logo3",
                PropertyName = "Logo3",
                Width = 100,
                ColumnCellTemplate = Application.Current.Resources["Logo3Template"] as DataTemplate
            },
            new DataGridColumn
            {
                Title = "Logo4",
                PropertyName = "Logo4",
                Width = 100,
                ColumnCellTemplate = Application.Current.Resources["Logo4Template"] as DataTemplate
            },
            new DataGridColumn
            {
                Title = "Logo5",
                PropertyName = "Logo5",
                Width = 100,
                ColumnCellTemplate = Application.Current.Resources["Logo5Template"] as DataTemplate
            }
        };

            Columns.AddRange (col) ;
        }

        public void BuildDataItems()
        {
            var list = new List<VirtualGridModel>();
            for (int i = 0; i < 50; i++)
            {
                list.Add(new VirtualGridModel
                {
                    Index = i,
                    Name = $"Item Value {i}",
                    Name1 = "Name1",
                    Name2 = "Name2",
                    Name3 = "Name3",
                    Name4 = "Name4",
                    Name5 = "Name5",
                    Name6 = "Name6",
                    Name7 = "Name7",
                });
            }

            DataItems.AddRange(list);
        }
    }
}
