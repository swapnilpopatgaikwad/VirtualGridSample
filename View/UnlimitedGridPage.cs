namespace VirtualGridSample.View
{
    internal class UnlimitedGridPage : ContentPage
    {
        private ScrollView _headerScrollView;
        private ScrollView _contentScrollView;
        private Grid _headerGrid;
        private Grid _contentGrid;

        public UnlimitedGridPage()
        {
            // Create the main layout
            var mainLayout = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            // Initialize header and content grids
            _headerGrid = new Grid();
            _contentGrid = new Grid();

            // Create ScrollView for header (horizontal scrolling only)
            _headerScrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                Content = _headerGrid,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never
            };
            _headerScrollView.Scrolled += OnContentHeaderScrolled;

            // Create ScrollView for content (both horizontal and vertical scrolling)
            _contentScrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Both,
                Content = _contentGrid
            };
            _contentScrollView.Scrolled += OnContentScrolled;

            // Add ScrollViews to the main layout
            mainLayout.Add(_headerScrollView, 0, 0);
            mainLayout.Add(_contentScrollView, 0, 1);

            Content = mainLayout;

            // Initialize data (example data, can be replaced with dynamic data)
            var headers = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                headers.Add("Header " + i);
            }

            var rows = new List<List<string>>();

            for (int i = 0; i < 40; i++)
            {
                var cols = new List<string>();
                for (int j = 0; j < 10; j++)
                {
                    cols.Add("Row" + i + ", " + "Col " + j);
                }
                rows.Add(cols);
            }

            // Populate the grids
            PopulateHeaderGrid(headers);
            PopulateContentGrid(rows);
        }

        private void OnContentHeaderScrolled(object? sender, ScrolledEventArgs e)
        {
            //if ((e.ScrollX > 0 && _contentScrollView.ScrollX != e.ScrollX) || (e.ScrollY > 0 && _contentScrollView.ScrollY != e.ScrollY))
            //   _contentScrollView.ScrollToAsync(e.ScrollX, e.ScrollY, false);
            if ((e.ScrollX > 0 && _contentScrollView.ScrollX != e.ScrollX) || (e.ScrollY > 0 && _contentScrollView.ScrollY != e.ScrollY))
                _contentScrollView.ScrollToAsync(e.ScrollX, _contentScrollView.ScrollY, false);
        }

        private void OnContentScrolled(object sender, ScrolledEventArgs e)
        {
            //if (e.ScrollX > 0 && e.ScrollY == 0) 
            //   _headerScrollView.ScrollToAsync(e.ScrollX, _headerScrollView.ScrollY, false);
            if (e.ScrollX > 0 && e.ScrollY == 0)
                _headerScrollView.ScrollToAsync(e.ScrollX, 0, false);
        }

        private void PopulateHeaderGrid(List<string> headers)
        {
            _headerGrid.ColumnDefinitions.Clear();
            _headerGrid.Children.Clear();

            for (int col = 0; col < headers.Count; col++)
            {
                _headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 100});
                var headerLabel = new Label
                {
                    Text = headers[col],
                    FontAttributes = FontAttributes.Bold,
                    Padding = new Thickness(5)
                };
                _headerGrid.Add(headerLabel, col, 0);
            }
        }

        private void PopulateContentGrid(List<List<string>> rows)
        {
            _contentGrid.ColumnDefinitions.Clear();
            _contentGrid.RowDefinitions.Clear();
            _contentGrid.Children.Clear();

            if (rows.Count > 0)
            {
                for (int col = 0; col < rows[0].Count; col++)
                {
                    _contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 100 });
                }

                for (int row = 0; row < rows.Count; row++)
                {
                    _contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    for (int col = 0; col < rows[row].Count; col++)
                    {
                        var cellLabel = new Label
                        {
                            Text = rows[row][col],
                            Padding = new Thickness(5)
                        };
                        //var stk = new StackLayout()
                        //{
                        //    cellLabel,
                        //    new Entry
                        //    {
                        //        BackgroundColor= Colors.Red,
                        //    }
                        //};
                        _contentGrid.Add(cellLabel, col, row);
                    }
                }
            }
        }
    }
}
