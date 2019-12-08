using NewsReader.Models;
using NewsReader.Services.APIServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace NewsReader.Services.MusicServices
{
    public static class MusicService
    {
        public static jsonMusicList MusicList;
        public static int count = 0;
        public static int list = 0;
        public static MediaPlayer MediaPlayer=new MediaPlayer();
        public static async void PlayerOnline()
        {
            if (MusicList == null || count == 9)
            {
                
                Debug.WriteLine(count);
                
                try
                {
                    MusicList = await API.GetMusicList(list);
                    count = 0;
                    list++;
                }
                catch
                {
                    Debug.WriteLine("网络异常");
                    return;
                }
                
            }else 
            {

                count++;

            }
            string id = MusicList.song_list[count].song_id;

            var temp = await API.GetsMusic(id);
            var urlstring = temp.bitrate.show_link;
            urlstring.Replace("\\", "");
            Debug.WriteLine(urlstring);
            MediaPlayer.SetUriSource(new Uri(urlstring));
            MediaPlayer.Play();
            MediaPlayer.MediaEnded += MediaElement_MediaEnded;
        }
        private static void MediaElement_MediaEnded(MediaPlayer sender, object e)
        {
            PlayerOnline();
        }
        

        public static async Task PlayerCacheAsync()
        {
            if (MusicList == null || count == 10)
                MusicList = await API.GetMusicList(list);
            if (count == 10)
            {
                list++;
                count = 0;
            }
            string id= MusicList.song_list[count].song_id;
            Debug.WriteLine(id);

            var temp = await API.GetsMusic(id);
            var urlstring = temp.bitrate.show_link;
            urlstring.Replace("\\", "");
            Debug.WriteLine(urlstring);
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            var buffer = await httpClient.GetBufferAsync(new Uri(urlstring));
            Debug.WriteLine(buffer);
            if (buffer == null) return;




            //创建对应本地资源  
            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
            fileSavePicker.FileTypeChoices.Add("校歌", new List<string>() { ".mp3" });
            var storageFile = await fileSavePicker.PickSaveFileAsync();
            if (storageFile == null) return;


            //将网络资源写入本地资源中  
            CachedFileManager.DeferUpdates(storageFile);
            await FileIO.WriteBufferAsync(storageFile, buffer);
            await CachedFileManager.CompleteUpdatesAsync(storageFile);


            //资源的流写入MediaElement中  
            var stream = await storageFile.OpenAsync(FileAccessMode.Read);
            MediaPlayer.SetStreamSource(stream);
            MediaPlayer.Play();

        }
    }
}
