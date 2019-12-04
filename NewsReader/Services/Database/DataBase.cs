using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_reader.Services.Database
{
    class DataBase
    {
        public static void LoadDatabase()
        {
            // Get a reference to the SQLite database
            var conn = new SQLiteConnection("Collected.db");
            string Newssql = @"CREATE TABLE IF NOT EXISTS
                                  CollectedNews (id             VARCHAR(100) PRIMARY KEY NOT NULL,
                                                pubDate         VARCHAR(100),
                                                channel         VARCHAR(100),
                                                title           VARCHAR(100),
                                                description     VARCHAR(200),
                                                img             VARCHAR(100),
                                                source          VARCHAR(100),
                                                link            VARCHAR(100)
                                                );";
            using (var statement = conn.Prepare(Newssql))
            {
                statement.Step();
            }
            Newssql = @"CREATE TABLE IF NOT EXISTS
                            Collectedchannels (id             VARCHAR(100) PRIMARY KEY NOT NULL,
                                              channelname     VARCHAR(100)
                                               );";
            using (var statement = conn.Prepare(Newssql))
            {
                statement.Step();
            }
        }

        public static bool GetNews(string id)
        {
            bool flag = false;
            var conn = new SQLiteConnection("Collected.db");
            try
            {
                using (var statement = conn.Prepare("SELECT title FROM CollectedNews WHERE id = ?"))
                {
                    statement.Bind(1, id);
                    if(statement.Step() == SQLiteResult.ROW)
                    {
                        flag = true;
                    }
                }
            }
            catch
            {
                
            }
            return flag;
        }

        public static void InsertNews(string id, string pubDate, string channel, string title, string description, string imgurl, string source, string link)
        {
            var conn = new SQLiteConnection("Collected.db");
            try
            {
                using (var Newsdata = conn.Prepare("INSERT INTO CollectedNews (id, pubDate, channel, title, description, img, source, link) VALUES (?, ?, ?, ?, ?, ?, ?, ?)"))
                {
                    Newsdata.Bind(1, id);
                    Newsdata.Bind(2, pubDate);
                    Newsdata.Bind(3, channel);
                    Newsdata.Bind(4, title);
                    Newsdata.Bind(5, description);
                    Newsdata.Bind(6, imgurl);
                    Newsdata.Bind(7, source);
                    Newsdata.Bind(8, link);
                    Newsdata.Step();
                }
            }
            catch
            {

            }
        }

        public static void DeleteNews(string id)
        {
            var conn = new SQLiteConnection("Collected.db");
            using (var todostmt = conn.Prepare("DELETE FROM CollectedNews WHERE id = ?"))
            {
                todostmt.Bind(1, id);
                todostmt.Step();
            }
        }

        public static List<string[]> GetAllNews()
        {
            var conn = new SQLiteConnection("Collected.db");
            List<string[]> Newslist = new List<string[]>();
            try
            {
                using (var statement = conn.Prepare("SELECT id, pubDate, channel, title, description, img, source, link FROM CollectedNews"))
                {
                    while (statement.Step() == SQLiteResult.ROW)
                    {
                        string[] News = { (string)statement[0], (string)statement[1], (string)statement[2], (string)statement[3], (string)statement[4], (string)statement[5], (string)statement[6], (string)statement[7] };
                        Newslist.Add(News);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return Newslist;
        }

        public static bool Getchannel(string id)
        {
            bool flag = false;
            var conn = new SQLiteConnection("Collected.db");
            try
            {
                using (var statement = conn.Prepare("SELECT channelname FROM Collectedchannels WHERE id = ?"))
                {
                    statement.Bind(1, id);
                    if (statement.Step() == SQLiteResult.ROW)
                    {
                        flag = true;
                    }
                }
            }
            catch
            {

            }
            return flag;
        }

        public static long Insertchannel(string id, string channelname)
        {
            var conn = new SQLiteConnection("Collected.db");
            try
            {
                using (var Newsdata = conn.Prepare("INSERT INTO Collectedchannels (id, channelname) VALUES (?, ?)"))
                {
                    Newsdata.Bind(1, id);
                    Newsdata.Bind(2, channelname);
                    Newsdata.Step();
                }
            }
            catch
            {

            }
            return conn.LastInsertRowId();
        }

        public static void Deletechannel(string id)
        {
            var conn = new SQLiteConnection("Collected.db");
            using (var todostmt = conn.Prepare("DELETE FROM Collectedchannels WHERE id = ?"))
            {
                todostmt.Bind(1, id);
                todostmt.Step();
            }
        }

        public static List<string[]> GetAllchannels()
        {
            var conn = new SQLiteConnection("Collected.db");
            List<string[]> channelslist = new List<string[]>();
            try
            {
                using (var statement = conn.Prepare("SELECT id, channelname FROM Collectedchannels"))
                {
                    while (statement.Step() == SQLiteResult.ROW)
                    {
                        string[] channels = { (string)statement[0], (string)statement[1] };
                        channelslist.Add(channels);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return channelslist;
        }
    }
}
