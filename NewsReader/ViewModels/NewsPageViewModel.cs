using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using NewsReader.Models;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using NewsReader.Services.APIServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Common;
using NewsReader.Services.Database;
using NewsReader.Views;
using Windows.Storage;

namespace NewsReader.ViewModels
{
    class NewsPageViewModel : ViewModelBase
    {
        public NewsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Mode = 0;
            }
            
        }
        public override IStateItems SessionState
        {
            get;
            set;
        }
        private ObservableCollection<News> _NewsList = new ObservableCollection<News>();
        public ObservableCollection<News> NewsList { get { return _NewsList; } set { Set(ref _NewsList, value); } }

        private int _Mode = 0;
        public int Mode { get { return _Mode; } set { Set(ref _Mode, value); } }

        private int _MaxPage = 1;
        public int MaxPage { get { return _MaxPage; } set { Set(ref _MaxPage, value); } }

        private int _CurrentPage = 1;
        public int CurrentPage { get { return _CurrentPage; } set { Set(ref _CurrentPage, value); } }

        private string _Value = "";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }
        private Channel _Channel;
        public Channel Channel { get { return _Channel; } set { Set(ref _Channel, value); } }
        public News Target;

        DelegateCommand <object>_LinkClick;
        public DelegateCommand<object> LinkClick
            => _LinkClick ?? (_LinkClick = new DelegateCommand<object>((sender) =>
            {
                var tar = sender as News;

                CollectNews(tar);
            }));
        
        public void CollectNews(News news)
        {
            bool flag = DataBase.GetNews(news.id);
            if (flag)
            {
                DataBase.DeleteNews(news.id);
                news.Collected = false;
            }
            else
            {
                DataBase.InsertNews(news.id, news.pubDate, news.channelname, news.title, news.description, news.img.ToString(), news.source, news.link);
                news.Collected = true;
            }
        }
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            
            if (mode != NavigationMode.Back)
            {
                if (suspensionState.Any())
                {
                    Debug.WriteLine("susppended");
                    for (int i = 0; i < (int)suspensionState["count"]; i++)
                    {
                        NewsList.Add(new News(suspensionState["id" + i].ToString(), suspensionState["pubDate" + i].ToString(), suspensionState["channelname" + i].ToString(), suspensionState["title" + i].ToString(), suspensionState["description" + i].ToString(), suspensionState["img" + i].ToString(), suspensionState["source" + i].ToString(), suspensionState["link" + i].ToString(), (bool)suspensionState["Collected" + i]));
                    }
                    Value = suspensionState["value"].ToString();
                }
                else
                { //跳转到newPage
                    Debug.WriteLine("Turn to news page ");
                    if (SessionState.ContainsKey("Message"))
                    {
                        Value = SessionState.Get<string>("Message");
                        SessionState.Remove("Message");
                        Mode = 0;

                    }
                    else
                    {
                        Debug.WriteLine("message unreceived ");

                    }
                    if (SessionState.ContainsKey("Channel"))
                    {
                        Channel = SessionState.Get<Channel>("Channel");
                        SessionState.Remove("Channel");
                        Mode = 1;

                    }
                    else
                    {
                        Debug.WriteLine("Channel unreceived ");

                    }
                    try
                    {
                        if (Mode == 0)
                        {
                            CreatList_Of_NewsSearch_Result(Value);
                        }

                        else
                        {
                            Value = "";
                            GetChannelNews(Channel.channelId, Value);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine(Channel);
                    }

                }
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Mode)] = Mode;
                //挂起
                Debug.WriteLine("susppending");
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
                suspensionState["value"] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void getNews(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;
            if (Mode == 1) GetChannelNews(Channel.channelId, Value);
            else CreatList_Of_NewsSearch_Result(Value);
        }
        public async void getPrevPageNews(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1) CurrentPage--;
            else
            {
                Busy.SetBusy(true, "It's already the first page");
                await Task.Delay(500);
                Busy.SetBusy(false);
                return;
            }
            if (Mode == 1)
                GetChannelNews(Channel.channelId, Value, CurrentPage);
            else
                CreatList_Of_NewsSearch_Result(Value, CurrentPage);
        }

        public async void getNextPageNews(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < MaxPage) CurrentPage++;
            else
            {
                Busy.SetBusy(true, "No more results");
                await Task.Delay(500);
                Busy.SetBusy(false);
                return;
            }
            if (Mode == 1)
                GetChannelNews(Channel.channelId, Value, CurrentPage);
            else
                CreatList_Of_NewsSearch_Result(Value, CurrentPage);
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
        public async void CreatList_Of_NewsSearch_Result(string search, int page = 1)
        {
            try
            {
                Busy.SetBusy(true,"Please Waiting...");
                NewsList.Clear();
                var News = await API.GetNewsbyapi(search, page);
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

                    //BitmapImage image = new BitmapImage(new Uri(uri, UriKind.Absolute));
                    if (DataBase.GetNews(list[i].id))
                        NewsList.Add(new News(list[i].id, list[i].pubDate, list[i].channelName, list[i].title, list[i].desc, uri, list[i].source, list[i].link, true));
                    else NewsList.Add(new News(list[i].id, list[i].pubDate, list[i].channelName, list[i].title, list[i].desc, uri, list[i].source, list[i].link));
                }
                MaxPage = result.pagebean.allPages;
                await Task.Delay(500);
                Busy.SetBusy(false);
            }
            catch
            {
                Busy.SetBusy(true, "Network anomaly...");
                Debug.Write("网络异常");
                await Task.Delay(500);
                Busy.SetBusy(false);
            }
        }
        public async void GetChannelNews(string channelId, string search, int page = 1)
        {
            try
            {
                Busy.SetBusy(true,"Please Waiting...");
                NewsList.Clear();
                var News = await API.GetChannelsNewsbyapi(channelId, search, page);
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

                    //BitmapImage image = new BitmapImage(new Uri(uri, UriKind.Absolute));
                    if (DataBase.GetNews(list[i].id))
                        NewsList.Add(new News(list[i].id, list[i].pubDate, list[i].channelName, list[i].title, list[i].desc, uri, list[i].source, list[i].link, true));
                    else NewsList.Add(new News(list[i].id, list[i].pubDate, list[i].channelName, list[i].title, list[i].desc, uri, list[i].source, list[i].link));
                }
                MaxPage = result.pagebean.allPages;
                await Task.Delay(500);
                Busy.SetBusy(false);
            }
            catch
            {
                
                Debug.Write("网络异常");
                await Task.Delay(500);
                Busy.SetBusy(false);
            }
        }

    }
}
