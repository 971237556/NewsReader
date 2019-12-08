using NewsReader.Services.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace NewsReader.Models
{
    [DataContract]
    public class Imageurls
    {
        [DataMember]
        public string height { get; set; }
        [DataMember]
        public string width { get; set; }
        [DataMember]
        public string url { get; set; }
    }

    [DataContract]
    public class Contentlist
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string pubDate { get; set; }
        [DataMember]
        public bool havePic { get; set; }
        [DataMember]
        public string channelName { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
        [DataMember]
        public List<Imageurls> imageurls { get; set; }
        [DataMember]
        public string source { get; set; }
        [DataMember]
        public string channelId { get; set; }
        [DataMember]
        public string nid { get; set; }
        [DataMember]
        public string link { get; set; }
    }
    [DataContract]
    public class Pagebean
    {
        [DataMember]
        public int allPages { get; set; }
        [DataMember]
        public List<Contentlist> contentlist { get; set; }
        [DataMember]
        public int currentPage { get; set; }
        [DataMember]
        public int allNum { get; set; }
        [DataMember]
        public int maxResult { get; set; }
    }
    [DataContract]
    public class NewsBody
    {
        [DataMember]
        public int ret_code { get; set; }
        [DataMember]
        public Pagebean pagebean { get; set; }
    }
    [DataContract(Name = "News")]
    public class jsonNews
    {
        [DataMember]
        public string showapi_res_error { get; set; }
        [DataMember]
        public int showapi_res_code { get; set; }
        [DataMember]
        public NewsBody showapi_res_body { get; set; }
    }
    public class News: INotifyPropertyChanged
    {
        public string id;

        public string pubDate { get; set; }

        public string channelname { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string img { get; set; }

        public string source { get; set; }

        public string link { get; set; }

        private bool _Collected;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        public bool Collected
        {
            get { return _Collected; }
            set
            {
                _Collected = value;
                NotifyPropertyChanged("Collected");
            }
        }

        public News()
        {

        }
        public News(string id,string date, string channelname, string title, string description, string image, string source, string link, bool Collected = false)
        {
            this.id = id;
            this.pubDate = date;
            this.channelname = channelname;
            this.title = title;
            this.description = description;
            this.img = image;
            this.source = source;
            this.link = link;
            this.Collected = Collected;
        }
    }
}
