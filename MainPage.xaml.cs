using VirtualGridSample.View;

namespace VirtualGridSample
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VirtualGridPage());
        }

        private void OnCounterClicked1(object sender, EventArgs e)
        {

            Navigation.PushAsync(new DemoGrid());
        }

        private void OnCounterClicked2(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UnlimitedGridPage());
        }
    }

}
