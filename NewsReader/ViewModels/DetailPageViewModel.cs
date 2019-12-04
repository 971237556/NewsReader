using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using News_reader.Models;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace News_reader.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        public DetailPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Target = new News();
            }
        }

        private News _Target;
        public News Target { get { return _Target; } set { Set(ref _Target, value); } }
        public override IStateItems SessionState
        {
            get;
            set;
        }
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Debug.WriteLine("suspended");
                //Target= (suspensionState.ContainsKey(nameof(Target))) ? (News)suspensionState[nameof(Target)] : (News)parameter;
                Target = new News(suspensionState["id"].ToString(), suspensionState["pubDate"].ToString(), suspensionState["channelname"].ToString(), suspensionState["title"].ToString(), suspensionState["description"].ToString(), suspensionState["img"].ToString(), suspensionState["source"].ToString(), suspensionState["link"].ToString(), (bool)suspensionState["Collected"]);
            }
            else
            {
                if (SessionState.ContainsKey("News")) {
                    Target = SessionState.Get<News>("News");
                }
                
            }

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                Debug.WriteLine("suspenging");
                suspensionState["id"] = Target.id;
                suspensionState["pubDate"] = Target.pubDate;
                suspensionState["channelname"] = Target.channelname;
                suspensionState["title"] = Target.title;
                suspensionState["description"] = Target.description;
                suspensionState["img"] = Target.img;
                suspensionState["source"] = Target.source;
                suspensionState["link"] = Target.link;
                suspensionState["Collected"] = Target.Collected;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void ShareNews()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;

            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            StringBuilder text = new StringBuilder("      ");
            text.AppendLine(Target.description);
            text.Append("From : ");
            text.AppendLine(Target.source);
            text.Append(Target.pubDate);
            //text.AppendLine(Target.link);


            request.Data.SetText(text.ToString());
            request.Data.Properties.Title = Target.title;
            request.Data.Properties.Description = "A demonstration on how to share";

            request.Data.SetWebLink(new Uri(Target.link));
            try
            {
                request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(Target.img)));
            }
            catch (Exception)
            {

            }
           
        }

        public void webView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            //Get the paramter from javascript
            string parameter = e.Value;
            Debug.WriteLine(parameter);
        }
    }
}
