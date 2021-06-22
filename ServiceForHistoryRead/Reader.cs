using Npgsql;
using System.Data.SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Diagnostics;

namespace ServiceForHistoryRead
{
    static class Reader
    {
        static List<string> urlList;
        //static List<string> titlesList;
        static List<int> visitCountList;
        static List<DateTime> visitTimeList;
        static EventLog logger;

        public static void DoReading()
        {
            logger = new EventLog();
            ////////////////////
            logger.Source = "Reader of History";
            logger.Log = "HService";
            logger.WriteEntry("Started reading sequince.", EventLogEntryType.Information);

            LoadHistory();
            SaveToDatabase();

            //string path = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Google\Chrome\User Data\Default\HistoryCopy";
            string path = @"C:\Users\Error\AppData\Local\Google\Chrome\User Data\Default\HistoryCopy";
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                try
                {
                    fileInf.Delete();
                }
                catch { }
            }

            logger.WriteEntry("Ended reading sequince.", EventLogEntryType.Information);

        }

        private static void LoadHistory()
        {
            try
            {
                urlList = new List<string>();

                //titlesList = new List<string>();
                visitCountList = new List<int>();
                visitTimeList = new List<DateTime>();

                ///////////
                //string path = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\HistoryCopy";
                string path = @"C:\Users\Error\AppData\Local\Google\Chrome\User Data\Default\HistoryCopy";
                FileInfo fileInf = new FileInfo(path);
                if (fileInf.Exists)
                {
                    fileInf.Delete();
                }
                //удаляем копию и копируем новую базу для работы
                //path = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Google\Chrome\User Data\Default\History";
                path = @"C:\Users\Error\AppData\Local\Google\Chrome\User Data\Default\History";

                //string newPath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\HistoryCopy";
                string newPath = @"C:\Users\Error\AppData\Local\Google\Chrome\User Data\Default\HistoryCopy";
                fileInf = new FileInfo(path);
                fileInf.CopyTo(newPath, true);
                ////////

                
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source= C:\Users\Error\AppData\Local\Google\Chrome\User Data\Default\HistoryCopy; Version=3;"))
                {
                    Connect.Open();

                    DateTime epoch = new DateTime(1601, 1, 1, 0, 0, 0, 0);
                    TimeSpan span = (DateTime.Today - epoch);

                    var Command = new SQLiteCommand
                    {
                        Connection = Connect,
                        //CommandText = "SELECT * FROM [urls] " +
                        //                "WHERE [hidden]==0 AND [last_visit_time]>=1000*" + span.TotalMilliseconds.ToString()
                        CommandText = "SELECT * FROM [urls] " +
                                        "WHERE [hidden]==0 "
                    };
                    SQLiteDataReader sqlReader = Command.ExecuteReader();
                    string dbUrlSite = null;
                    //string dbTitleSite = null;
                    int dbVisitCount = 0;
                    ulong dbVisitTime = 0;
                    while (sqlReader.Read())
                    {
                        dbUrlSite = (string)sqlReader["url"];
                        urlList.Add(dbUrlSite);

                        dbVisitCount = int.Parse(sqlReader["visit_count"].ToString());
                        visitCountList.Add(dbVisitCount);

                        //1209600000000 - 14 дней   13254192000000000 - 4-1-21
                        dbVisitTime = ulong.Parse(sqlReader["last_visit_time"].ToString());
                        if (dbVisitTime != 0)
                        {
                            while (dbVisitTime > (long)13254192000000000)
                                dbVisitTime -= (long)1209600000000;
                            dbVisitTime += (long)1209600000000;
                        }
                        else dbVisitTime = (long)13254228000000000;
                        epoch = DateTime.FromFileTime((long)dbVisitTime * 10);
                        visitTimeList.Add(epoch);
                    }
                    Connect.Close();

                }

            }
            catch (Exception ex)
            {
                logger.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }
        }
        private static void SaveToDatabase()
        {
            ///Запись данных в БД
            try
            {
                string connString = "Server=192.168.56.129;User ID=postgres;Database=db_urls;Port=5432;Password=password;SSLMode=Prefer";
                /////////////////////////////
                using (var conn = new NpgsqlConnection(connString))
                {
                    //открытие соединения
                    conn.Open();
                    logger.WriteEntry(urlList.Count.ToString()+" к записи в БД", EventLogEntryType.Warning);
                    for (int i = 0; i < urlList.Count; i++)
                    {
                        //logger.WriteEntry("Записана одна строка", EventLogEntryType.Warning);
                        using (var command = new NpgsqlCommand("INSERT INTO public.cs_rawData (url, visit_count, last_visit_time, aud) VALUES (@u1, @vc1, @lvt1, @a1)", conn))
                        {
                            command.Parameters.AddWithValue("u1", urlList[i]);
                            command.Parameters.AddWithValue("vc1", visitCountList[i]);
                            command.Parameters.AddWithValue("lvt1", visitTimeList[i]);
                            command.Parameters.AddWithValue("a1", Environment.MachineName.ToString());

                            command.ExecuteNonQuery();
                        }

                    }
                    //Закрытие
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                logger.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                //Console.WriteLine("Ошибка в отправке данных в БД {0}", ex.ToString());
                //Console.ReadKey();
            }
        }
    }
}
