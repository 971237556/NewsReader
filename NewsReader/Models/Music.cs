using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NewsReader.Models
{
    [DataContract]
    public class Song_list
    {
        /// <summary>
        /// 88
        /// </summary>
        [DataMember]
        public string artist_id { get; set; }
        /// <summary>
        /// http://qukufile2.qianqian.com/data2/pic/2ff52a51c69037d87ec6725cd9cceb0b/591579112/591579112.jpg@s_1,w_90,h_90
        /// </summary>
        [DataMember]
        public string pic_small { get; set; }
        /// <summary>
        /// 591579114
        /// </summary>
        [DataMember]
        public string song_id { get; set; }
        /// <summary>
        /// 肆无忌惮
        /// </summary>
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string artist_name { get; set; }
    }
    
    [DataContract (Name = "Root" )]
    public class jsonMusicList
    {
        /// <summary>
        /// Song_list
        /// </summary>
        [DataMember]
        public List<Song_list> song_list { get; set; }
        [DataMember]
        public int error_code { get; set; }
    }
    [DataContract]
    public class Songinfo
    {


        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string author { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string song_id { get; set; }

        /// <summary>
        /// 海阔天空
        /// </summary>
        [DataMember]
        public string title { get; set; }


        [DataMember]
        public string lrclink { get; set; }


        [DataMember]
        public string pic_small { get; set; }


    }


    [DataContract]
    public class Bitrate
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string show_link { get; set; }


        public int song_file_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int file_size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string file_extension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int file_duration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int file_bitrate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string file_link { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string hash { get; set; }

    }


    [DataContract (Name ="Root")]
    public class jsonMusic
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Songinfo songinfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Bitrate bitrate { get; set; }

    }

}
