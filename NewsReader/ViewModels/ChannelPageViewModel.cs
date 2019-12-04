using News_reader.Models;
using News_reader.Services.APIServices;
using News_reader.Services.Database;
using News_reader.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace News_reader.ViewModels
{
    class ChannelPageViewModel : ViewModelBase
    {

        private ObservableCollection<Channel> _ChannelsList=new ObservableCollection<Channel>();
        public ObservableCollection<Channel> ChannelsList { get { return _ChannelsList; } set { Set(ref _ChannelsList, value); } }

        DelegateCommand<object> _LinkClick;
        public DelegateCommand<object> LinkClick
            => _LinkClick ?? (_LinkClick = new DelegateCommand<object>((sender) =>
            {
                var tar = sender as Channel;

                this.Subscribe(tar);
            }));

        public ChannelPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                
            }
            getChannelsAsync();
        }


        public async void getChannelsAsync()
        {
            try
            {
                var jsonChannels = await API.GetChannelsbyapi();
                var channelList = jsonChannels.showapi_res_body.channelList;
                Debug.WriteLine(channelList.Count);
                foreach (var i in channelList)
                {
                    if(DataBase.Getchannel(i.channelId))
                    ChannelsList.Add(new Channel(i, true));
                    else ChannelsList.Add(new Channel(i));
                }
            }
            catch
            {
                Busy.SetBusy(true, "Network anomaly...");
                Debug.Write("网络异常");
                await Task.Delay(500);
                Busy.SetBusy(false);
            }
            return;
        }
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                //Channel = suspensionState[nameof(Channel)]?.ToString();
            }
            await Task.CompletedTask;
            foreach(var item in ChannelsList)
            {
                if (DataBase.Getchannel(item.channelId))
                {
                    item.IsSubscribed = true;
                }
                else
                {
                    item.IsSubscribed = false;
                }
            }
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                //suspensionState[nameof(Channel)] = Channel;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }
        public void Subscribe(object sender, RoutedEventArgs e)
        {
            var tar = sender as Channel;

            Subscribe(tar);
        }
        public void Subscribe(Channel channel)
        {
            bool flag = DataBase.Getchannel(channel.channelId);
            if (flag)
            {
                DataBase.Deletechannel(channel.channelId);
                channel.IsSubscribed = false;
            }
            else
            {
                DataBase.Insertchannel(channel.channelId, channel.name);
                channel.IsSubscribed = true;
            }
        }
        public void JumpToChannelNews(object sender, ItemClickEventArgs e)
        {
            if (SessionState.ContainsKey("Channel"))
            {
                SessionState.Remove("Channel");
            }
            var channel = e.ClickedItem as Channel;
            Debug.WriteLine(channel.channelId);
            SessionState.Add("Channel", channel);
            NavigationService.Navigate(typeof(NewsPage));
        }
    }
}
