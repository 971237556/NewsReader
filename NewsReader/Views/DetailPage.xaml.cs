using NewsReader.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using System;

namespace NewsReader.Views
{
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            Busy.SetBusy(true, "Loading...");
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void NewsDetail_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Busy.SetBusy(false, "ok");
        }
    }
}
