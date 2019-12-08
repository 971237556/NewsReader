using NewsReader.Services.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NewsReader.Models
{
    [DataContract]
    public class ChannelList
    {
        [DataMember]
        public string channelId { get; set; }
        [DataMember]
        public string name { get; set; }
    }
    [DataContract]
    public class ChannelsBody
    {
        [DataMember]
        public int totalNum { get; set; }
        [DataMember]
        public int ret_code { get; set; }
        [DataMember]
        public List<ChannelList> channelList { get; set; }
    }
    [DataContract (Name = "Channels")  ]
    public class jsonChannels
    {
        [DataMember]
        public string showapi_res_error { get; set; }
        [DataMember]
        public int showapi_res_code { get; set; }
        [DataMember]
        public ChannelsBody showapi_res_body { get; set; }
    }
    public class Channel:INotifyPropertyChanged
    {
        public string channelId { get; set; }
        public string name { get; set; }
        public string pic_path { get; set; }
        public bool _IsSubscribed { get; set; }
        public bool _IsUnSubscribed { get; set; }
        public bool IsSubscribed {
            get { return _IsSubscribed; }
            set {
                if (value != _IsSubscribed)
                { _IsSubscribed = value;
                    IsUnSubscribed = !value;
                    NotifyPropertyChanged("IsSubscribed");
                }}}
        public bool IsUnSubscribed { get { return _IsUnSubscribed; } set { if (value != _IsUnSubscribed) { _IsUnSubscribed = value; IsSubscribed = !value; NotifyPropertyChanged("IsUnSubscribed"); } } }
        public bool UnSubscribed { get; set; }

        public Channel(ChannelList channel, bool Collected = false)
        {
            UnSubscribed = true;
            _IsSubscribed = Collected;
            _IsUnSubscribed = !Collected;
            channelId = channel.channelId;
            name = channel.name;
            pic_path = "ms-appx:///Assets/" + name + ".png";
            
        }
        public Channel(string id, string channelname, bool Collected = false)
        {
            if (id == "0")
            {
                UnSubscribed = false;
            }
            else
            {
                _IsSubscribed = Collected;
                _IsUnSubscribed = !Collected;
                UnSubscribed = true;
            }
            channelId = id;
            name = channelname;
            pic_path = "ms-appx:///Assets/" + name + ".png";
            if (DataBase.Getchannel(channelId))
            {
                _IsSubscribed = true;
                _IsUnSubscribed = false;
            }
        }
        public void Subscribe()
        {
            IsSubscribed = !IsSubscribed;
        }
        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
