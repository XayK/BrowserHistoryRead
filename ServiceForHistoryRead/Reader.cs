using Npgsql;
using System.Data.SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace ServiceForHistoryRead
{
    static class Reader
    {
        static List<string> urlList;
        //static List<string> titlesList;
        static List<int> visitCountList;
        static List<DateTime> visitTimeList;
        private static Logger logger;
        public static void DoReading()
        {
            logger = LogManager.GetCurrentClassLogger();
            LoadHistory();
            SaveToDatabase();

            string path = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Google\Chrome\User Data\Default\HistoryCopy";
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                try
                {
                    fileInf.Delete();
                }
                catch { }
            }

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
                string path = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\HistoryCopy";
                FileInfo fileInf = new FileInfo(path);
                if (fileInf.Exists)
                {
                    fileInf.Delete();
                }
                //удаляем копию и копируем новую базу для работы
                path = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Google\Chrome\User Data\Default\History";
                //string newPath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Google\Chrome\User Data\Default\HistoryCopy";
                string newPath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\HistoryCopy";
                fileInf = new FileInfo(path);
                fileInf.CopyTo(newPath, true);
                ////////

                //   using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source= "+ System.Environment.SpecialFolder.UserProfile +@"\AppData\Local\Google\Chrome\User Data\Default\HistoryCopy; Version=3;"))
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source= " + Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + @"\HistoryCopy; Version=3;"))
                {
                    Connect.Open();

                    DateTime epoch = new DateTime(1601, 1, 1, 0, 0, 0, 0);
                    TimeSpan span = (DateTime.Today - epoch);

                    var Command = new SQLiteCommand
                    {
                        Connection = Connect,
                        CommandText = "SELECT * FROM [urls] " +
                                        "WHERE [hidden]==0 AND [last_visit_time]>=1000*" + span.TotalMilliseconds.ToString()
                    };
                    SQLiteDataReader sqlReader = Command.ExecuteReader();
                    string dbUrlSite = null;
                    //string dbTitleSite = null;
                    int dbVisitCount = 0;
                    UInt64 dbVisitTime = 0;
                    while (sqlReader.Read())
                    {
                        dbUrlSite = (string)sqlReader["url"];
                        urlList.Add(dbUrlSite);

                        //dbTitleSite = sqlReader["title"].ToString();
                        //titlesList.Add(dbTitleSite);

                        dbVisitCount = int.Parse(sqlReader["visit_count"].ToString());
                        visitCountList.Add(dbVisitCount);

                        dbVisitTime = UInt64.Parse(sqlReader["last_visit_time"].ToString());
                        epoch = DateTime.FromFileTime((long)dbVisitTime * 10);
                        //epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        //epoch.AddMilliseconds((dbVisitTime / 1000000) - 11644473600);
                        visitTimeList.Add(epoch);
                        //visitTimeList.Add(new DateTime(2000, 1, 1, (int)((dbVisitTime / 3600) % 24), (int)((dbVisitTime / 60) % 60), (int)(dbVisitTime % 60)));
                    }
                    Connect.Close();

                }

                //Console.WriteLine("GOOD");
            }
            catch (Exception ex)
            {

                logger.Debug(ex.ToString());
                //Console.WriteLine("Ошибка в открытии файла истории данных {0}", ex.ToString());
                //Console.ReadKey();
            }
        }
        private static void SaveToDatabase()
        {
            ///Запись данных в БД
            try
            {
                string Host = "192.168.56.129";
                string User = "postgres";
                string DBname = "db_urls";
                string Password = "password";
                string Port = "5432";
                string connString =
                String.Format(
                       "Server={0};User ID={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                       Host, User, DBname, Port, Password);
                /////////////////////////////
                using (var conn = new NpgsqlConnection(connString))
                {
                    //открытие соединения
                    conn.Open();
                    for (int i = 0; i < urlList.Count; i++)
                    {
                        using (var command = new NpgsqlCommand("INSERT INTO public.cs_rawData (url, visit_count, last_visit_time, aud) VALUES (@u1, @vc1, @lvt1, @a1)", conn))
                        {
                            command.Parameters.AddWithValue("u1", urlList[i]);
                            //command.Parameters.AddWithValue("t1", titlesList[i]);
                            command.Parameters.AddWithValue("vc1", visitCountList[i]);
                            command.Parameters.AddWithValue("lvt1", visitTimeList[i]);
                            command.Parameters.AddWithValue("a1", Environment.MachineName.ToString());

                            //int nRows = command.ExecuteNonQuery();//Выполенние команды ввода
                            //Console.Out.WriteLine(String.Format("Number of rows inserted={0}", nRows));
                            command.ExecuteNonQuery();
                        }

                    }
                    //Закрытие
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                logger.Debug(ex.ToString());
                //Console.WriteLine("Ошибка в отправке данных в БД {0}", ex.ToString());
                //Console.ReadKey();
            }
        }
    }
}
