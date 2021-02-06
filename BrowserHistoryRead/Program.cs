using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.IO;
using System.Diagnostics;
using Npgsql;

namespace BrowserHistoryRead
{
    class Program
    {

        static List<string> urlList;
        static List<string> titlesList;
        static List<int> visitCountList;
        static List<DateTime> visitTimeList;

        static void Main(string[] args)
        {
            ///////////Запрос к SQLite из Google Chrome
            try
            {
                urlList = new List<string>();
                titlesList = new List<string>();
                visitCountList = new List<int>();
                visitTimeList = new List<DateTime>();
                using (SQLiteConnection Connect = new SQLiteConnection("Data Source=D:/History; Version=3;"))
                {
                    Connect.Open();
                    SQLiteCommand Command = new SQLiteCommand
                    {
                        Connection = Connect,
                        CommandText = "SELECT * FROM [urls] where [hidden]==0"
                    };
                    SQLiteDataReader sqlReader = Command.ExecuteReader();
                    string dbUrlSite = null;
                    string dbTitleSite = null;
                    int dbVisitCount = 0;
                    UInt64 dbVisitTime = 0;
                    while (sqlReader.Read())
                    {
                        dbUrlSite = (string)sqlReader["url"];
                        urlList.Add(dbUrlSite);
                        dbTitleSite = sqlReader["title"].ToString();
                        titlesList.Add(dbTitleSite);
                        dbVisitCount = int.Parse(sqlReader["visit_count"].ToString());
                        visitCountList.Add(dbVisitCount);
                        dbVisitTime = UInt64.Parse(sqlReader["last_visit_time"].ToString()); 
                        visitTimeList.Add(new DateTime(2000, 1, 1, (int)((dbVisitTime / 3600) % 24), (int)((dbVisitTime / 60) % 60), (int)(dbVisitTime % 60)));
                    }
                    Connect.Close();
                    
                }

                //Console.WriteLine("GOOD");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка в открытии файла истории данных {0}",ex.ToString());
                Console.ReadKey();
            }

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
                           using (var command = new NpgsqlCommand("INSERT INTO public.rawData (url, title, visit_count, last_visit_time) VALUES (@u1, @t1, @vc1, @lvt1)", conn))
                           {
                               command.Parameters.AddWithValue("u1", urlList[i]);
                               command.Parameters.AddWithValue("t1", titlesList[i]);
                               command.Parameters.AddWithValue("vc1", visitCountList[i]);                            
                               command.Parameters.AddWithValue("lvt1", visitTimeList[i]);

                               int nRows = command.ExecuteNonQuery();//Выполенние команды ввода
                               Console.Out.WriteLine(String.Format("Number of rows inserted={0}", nRows));
                           }
                       }
                       //Закрытие
                       conn.Close();
                   } 

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка в отправке данных в БД {0}", ex.ToString());
                Console.ReadKey();
            }

            //////////END
        }
    }
}
