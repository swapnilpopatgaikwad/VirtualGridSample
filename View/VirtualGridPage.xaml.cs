using VirtualGridSample.ViewModel;

namespace VirtualGridSample.View;

public partial class VirtualGridPage : ContentPage
{
	public VirtualGridPage()
	{
		InitializeComponent();
		BindingContext = new VirtualGridViewModel();
    }
}