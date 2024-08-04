namespace VirtualGridSample.View;

public partial class DemoGrid : ContentPage
{
	public DemoGrid()
	{
		InitializeComponent();
        //headerScrollView.Scrolled += OnHeaderScrolled;
    }

    //private async void OnHeaderScrolled(object sender, ScrolledEventArgs e)
    //{
    //    // Synchronize horizontal scrolling
    //   await contentScrollView.ScrollToAsync(e.ScrollX, contentScrollView.ScrollY, false);
    //}

    //private async void OnContentScrolled(object sender, ScrolledEventArgs e)
    //{
    //    // Synchronize horizontal scrolling
    //  await  headerScrollView.ScrollToAsync(e.ScrollX, headerScrollView.ScrollY, false);
    //}
}