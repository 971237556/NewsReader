using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using NewsReader.Models;
using NewsReader.Services.Database;
using Windows.UI.Xaml;
using NewsReader.Services.APIServices;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.Media.Playback;

namespace NewsReader.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                
            }

        }

        private ObservableCollection<Channel> _ChannelsList = new ObservableCollection<Channel>();
        public ObservableCollection<Channel> ChannelsList { get { return _ChannelsList; } set { Set(ref _ChannelsList, value); } }

        private ObservableCollection<News> _NewsList = new ObservableCollection<News>();
        public ObservableCollection<News> NewsList { get { return _NewsList; } set { Set(ref _NewsList, value); } }

        public void getcollectedChannels()
        {
            ChannelsList.Clear();
            var all = new Channel("0", "全部");
            ChannelsList.Add(all);
            List<string[]> NewsList = DataBase.GetAllchannels();
            foreach (string[] element in NewsList)
            {
                var temp = new Channel(element[0], element[1]);
                ChannelsList.Add(temp);
            }
        }
        private DelegateCommand<object> _NewsClick;
        public DelegateCommand<object> NewsClick
            => _NewsClick ?? (_NewsClick = new DelegateCommand<object>((sender) =>
            {
                var tar = sender as News;
                Debug.WriteLine(tar);
                this.CollectNews(tar);
                Debug.WriteLine(tar.Collected);
            }));
        public void CollectNews(News news)
        {
            bool flag = DataBase.GetNews(news.id);
            Debug.WriteLine(news.Collected);
            Debug.WriteLine(flag);
            if (flag)
            {
                Debug.WriteLine(news.Collected);
                news.Collected = false;
                DataBase.DeleteNews(news.id);
            }
            else
            {
                Debug.WriteLine(news.Collected);
                news.Collected = true;
                DataBase.InsertNews(news.id, news.pubDate, news.channelname, news.title, news.description, news.img, news.source, news.link);
            }

            Debug.WriteLine(news.Collected);
        }
        private DelegateCommand<object> _ChannelClick;
        public DelegateCommand<object> ChannelClick
            => _ChannelClick ?? (_ChannelClick = new DelegateCommand<object>((sender) =>
            {
                var tar = sender as Channel;

                this.Subscribe(tar);
            }));
        public void Subscribe(Channel channel)
        {
            if (channel.channelId == "0") return;
            bool flag = DataBase.Getchannel(channel.channelId);
            if (flag)
            {
                DataBase.Deletechannel(channel.channelId);
                channel.IsSubscribed = false;
                ChannelsList.Remove(channel);
            }
            else
            {
                DataBase.Insertchannel(channel.channelId, channel.name);
                channel.IsSubscribed = true;
                ChannelsList.Add(channel);
            }
        }
        

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (mode != NavigationMode.Back)
            {
                if (suspensionState.Any())
                {
                    //
                    for (int i = 0; i < (int)suspensionState["count"]; i++)
                    {
                        NewsList.Add(new News(suspensionState["id" + i].ToString(), suspensionState["pubDate" + i].ToString(), suspensionState["channelname" + i].ToString(), suspensionState["title" + i].ToString(), suspensionState["description" + i].ToString(), suspensionState["img" + i].ToString(), suspensionState["source" + i].ToString(), suspensionState["link" + i].ToString(), (bool)suspensionState["Collected" + i]));
                    }
                }
                await Task.CompletedTask;
                getcollectedChannels();
                
            }
             
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                //suspensionState[nameof(Value)] = Value;
                int count = 0;
                foreach (var item in NewsList)
                {
                    suspensionState["id" + count] = item.id;
                    suspensionState["pubDate" + count] = item.pubDate;
                    suspensionState["channelname" + count] = item.channelname;
                    suspensionState["title" + count] = item.title;
                    suspensionState["description" + count] = item.description;
                    suspensionState["img" + count] = item.img;
                    suspensionState["source" + count] = item.source;
                    suspensionState["link" + count] = item.link;
                    suspensionState["Collected" + count] = item.Collected;
                    count++;
                }
                suspensionState["count"] = count;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoDetailsPage(object sender, ItemClickEventArgs e)
        {
            var Target = e.ClickedItem as News;
            if (SessionState.ContainsKey("News"))
            {
                SessionState.Remove("News");
            }
            SessionState.Add("News", Target);
            NavigationService.Navigate(typeof(Views.DetailPage), Target);
        }

    public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        //按时间排序
        public void sort()
        {
            List<News> sortedList = NewsList.OrderByDescending(item => item.pubDate).ToList();
            for (int i = 0; i < sortedList.Count(); i++)
            {
                    NewsList.Move(NewsList.IndexOf(sortedList[i]), i);
            }
            Debug.WriteLine("sort");
        }

        public async void getChannelNews(object sender, ItemClickEventArgs e)
        {
            var channel = e.ClickedItem as Channel;
            Debug.WriteLine(channel.channelId);
            if(channel.channelId == "0")
            {
                NewsList.Clear();
                List<string[]> newsList = DataBase.GetAllchannels();
                foreach (string[] element in newsList)
                {
                    await GetChannelNews(element[0]);
                }
                sort();
                //删除多的元素
                while(NewsList.Count() > 20)
                {
                    NewsList.RemoveAt(NewsList.IndexOf(NewsList.Last()));
                }
            }
            else
            {
                NewsList.Clear();
                await GetChannelNews(channel.channelId);
            }
        }

        public async Task GetChannelNews(string channelId, int page = 1)
        {
            try
            {
                var News = await API.GetChannelsNewsbyapi(channelId, "", page);
                var result = News.showapi_res_body;
                var num = result.pagebean.maxResult;
                var list = result.pagebean.contentlist;
                for (int i = 0; i < num && i < list.Count; i++)
                {
                    string uri = "";
                    if (list[i].havePic)
                    {
                        uri = list[i].imageurls[0].url;
                    }
                    else
                    {
                        uri = "ms-appx:///SroreLogo.png";
                    }

                    BitmapImage image = new BitmapImage(new Uri(uri, UriKind.Absolute));
                    if (DataBase.GetNews(list[i].id))
                        NewsList.Add(new News(list[i].id, list[i].pubDate, list[i].channelName, list[i].title, list[i].desc, uri, list[i].source, list[i].link, true));
                    else NewsList.Add(new News(list[i].id, list[i].pubDate, list[i].channelName, list[i].title, list[i].desc, uri, list[i].source, list[i].link,false));
                }
                //MaxPage = result.pagebean.allPages;
            }
            catch
            {
                Debug.Write("网络异常");
            }
        }

    }
}
