using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using News_reader.Models;
using News_reader.Views;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace News_reader.Services.APIServices
{
    static class API
    {
        public static string filename="";
        public async static Task<jsonChannels> GetChannelsbyapi()
        {
            Busy.SetBusy(true, "Please Waiting...");
            var http = new HttpClient();
            var response = await http.GetAsync("http://route.showapi.com/109-34?showapi_appid=65739&showapi_sign=274dd8d6a80a43b1bf73c4be1467d13d");
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(jsonChannels));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (jsonChannels)serializer.ReadObject(ms);
            Busy.SetBusy(false);
            return data;

        }
        public async static Task<jsonNews> GetChannelsNewsbyapi(string channelId, string title, int page)
        {
            Busy.SetBusy(true, "Please Waiting...");
            var http = new HttpClient();
            var response = await http.GetAsync("http://route.showapi.com/109-35?showapi_appid=65739&channelId=" + channelId + "&channelName=&title=" + title + "&page=" + page.ToString() + "&needContent=0&needHtml=0&needAllList=0&maxResult=20&id=&showapi_sign=274dd8d6a80a43b1bf73c4be1467d13d");
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(jsonNews));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (jsonNews)serializer.ReadObject(ms);
            Debug.WriteLine(result);
            Busy.SetBusy(false);
            return data;
        }
        public async static Task<jsonNews> GetNewsbyapi(string title, int page)
        {
            Busy.SetBusy(true, "Please Waiting...");
            var http = new HttpClient();
            var response = await http.GetAsync("http://route.showapi.com/109-35?showapi_appid=65739&channelId=&channelName=&title=" + title + "&page=" + page.ToString() + "&needContent=0&needHtml=0&needAllList=0&maxResult=20&id=&showapi_sign=274dd8d6a80a43b1bf73c4be1467d13d");

            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(jsonNews));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (jsonNews)serializer.ReadObject(ms);
            Debug.WriteLine(result);
            Busy.SetBusy(false);
            return data;
        }
        public async static Task<jsonMusicList> GetMusicList(int list)
        {

            var http = new HttpClient();
            var response = await http.GetAsync("http://tingapi.ting.baidu.com/v1/restserver/ting?method=baidu.ting.billboard.billList&type=1&size=10&offset="+list.ToString());
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(jsonMusicList));

            Debug.WriteLine(result);
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (jsonMusicList)serializer.ReadObject(ms);
            return data;

        }

        public async static Task<jsonMusic> GetsMusic(string id)
        {

            var http = new HttpClient();
            var response = await http.GetAsync(new Uri("http://tingapi.ting.baidu.com/v1/restserver/ting?method=baidu.ting.song.play&songid="+id));
            var result = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(result);
            var serializer = new DataContractJsonSerializer(typeof(jsonMusic));

            Debug.WriteLine(result);
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (jsonMusic)serializer.ReadObject(ms);

            Debug.WriteLine(result);
            return data;
            
        }

        
    }
    

}
