using News_reader.Models;
using News_reader.Services.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace News_reader.ViewModels
{
    class LikesPageViewModel : ViewModelBase
    {
        public LikesPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = 0;
            }
            
        }
        private ObservableCollection<News> _NewsList = new ObservableCollection<News>();
        public ObservableCollection<News> NewsList { get { return _NewsList; } set { Set(ref _NewsList, value); } }

        public void AddNews(string id, string date, string channlname, string title, string description, string image, string source, string link)
        {
            NewsList.Add(new News(id, date, channlname, title, description, image, source, link, true));
        }

        private int _Value = 0;
        public int Value { get { return _Value; } set { Set(ref _Value, value); } }

        public News Target;

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = (int)suspensionState[nameof(Value)];
            }
            await Task.CompletedTask;
            GetNews();
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }
        DelegateCommand<object> _LinkClick;
        public DelegateCommand<object> LinkClick
            => _LinkClick ?? (_LinkClick = new DelegateCommand<object>((sender) =>
            {
                var tar = sender as News;

                CollectNews(tar);
            }));
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }
        
        public void GotoDetailsPage(object sender, ItemClickEventArgs e)
        {
            Target = e.ClickedItem as News;
            if (SessionState.ContainsKey("News"))
            {
                SessionState.Remove("News");
            }
            SessionState.Add("News", Target);
            NavigationService.Navigate(typeof(Views.DetailPage), Target);
        }

        
        public void CollectNews(News news)
        {
            bool flag = DataBase.GetNews(news.id);
            if (flag)
            {
                DataBase.DeleteNews(news.id);
                NewsList.Remove(news);
            }
            else
            {
                DataBase.InsertNews(news.id, news.pubDate, news.channelname, news.title, news.description, news.img, news.source, news.link);
                NewsList.Add(news);
            }
        }
        public void GetNews()
        {
            NewsList.Clear();
            List<string[]> newsList = DataBase.GetAllNews();
            foreach (string[] element in newsList)
            {
                string uri = element[5];
                //BitmapImage image = new BitmapImage(new Uri(uri, UriKind.Absolute));
                AddNews(element[0], element[1], element[2], element[3], element[4], uri, element[6], element[7]);
            }
        }

    }

}
