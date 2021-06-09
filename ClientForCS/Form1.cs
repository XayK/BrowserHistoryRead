using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientForCS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            for(int i =0;i<12;i++)
                dataGridView1.Rows.Add();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string original = textBox1.Text;
            

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
                    //добавляем дисциплину в базу
                    using (var command = new NpgsqlCommand("INSERT INTO public.discipline (name_disp, text_of_disp, timeofAdd) VALUES (@nd, @tod,@tadd)", conn))
                    {
                        command.Parameters.AddWithValue("nd", textBox2.Text.ToString());
                        command.Parameters.AddWithValue("tod", textBox1.Text.ToString());
                        command.Parameters.AddWithValue("tadd", DateTime.Now);
                        command.ExecuteNonQuery();
                    }///добавляем запрос на анализ
                   var SELcommand = new NpgsqlCommand("Select id_disp from public.discipline Where name_disp= '" + textBox2.Text.ToString() +"'", conn);
                    NpgsqlDataReader readerSel = SELcommand.ExecuteReader();
                    
                    int tmpID = 0; // если дисциплина получаем ее id, иначе 0
                    if(readerSel.Read())
                    {
                        tmpID = int.Parse(readerSel[0].ToString());
                    }
                    readerSel.Close();

                    ///заполнение расписания пар
                    string tmpstr = "";
                    {
                        ///1 неделя
                        if (CB_N_1_D_1_P_1.Checked)tmpstr += "Н1Д1П1 "; if (CB_N_1_D_1_P_2.Checked)tmpstr += "Н1Д1П2 "; if (CB_N_1_D_1_P_3.Checked)tmpstr += "Н1Д1П3 "; if (CB_N_1_D_1_P_4.Checked)tmpstr += "Н1Д1П4 ";
                        if (CB_N_1_D_1_P_5.Checked)tmpstr += "Н1Д1П5 "; if (CB_N_1_D_1_P_6.Checked)tmpstr += "Н1Д1П6 "; if (CB_N_1_D_1_P_7.Checked)tmpstr += "Н1Д1П7 ";

                        if (CB_N_1_D_2_P_1.Checked) tmpstr += "Н1Д2П1 "; if (CB_N_1_D_2_P_2.Checked) tmpstr += "Н1Д2П2 "; if (CB_N_1_D_2_P_3.Checked) tmpstr += "Н1Д2П3 "; if (CB_N_1_D_2_P_4.Checked) tmpstr += "Н1Д2П4 ";
                        if (CB_N_1_D_2_P_5.Checked) tmpstr += "Н1Д2П5 "; if (CB_N_1_D_2_P_6.Checked) tmpstr += "Н1Д2П6 "; if (CB_N_1_D_2_P_7.Checked) tmpstr += "Н1Д2П7 ";
                            
                        if (CB_N_1_D_3_P_1.Checked) tmpstr += "Н1Д3П1 "; if (CB_N_1_D_3_P_2.Checked) tmpstr += "Н1Д3П2 "; if (CB_N_1_D_3_P_3.Checked) tmpstr += "Н1Д3П3 "; if (CB_N_1_D_3_P_4.Checked) tmpstr += "Н1Д3П4 ";
                        if (CB_N_1_D_3_P_5.Checked) tmpstr += "Н1Д3П5 "; if (CB_N_1_D_3_P_6.Checked) tmpstr += "Н1Д3П6 "; if (CB_N_1_D_3_P_7.Checked) tmpstr += "Н1Д3П7 ";

                        if (CB_N_1_D_4_P_1.Checked) tmpstr += "Н1Д4П1 "; if (CB_N_1_D_4_P_2.Checked) tmpstr += "Н1Д4П2 "; if (CB_N_1_D_4_P_3.Checked) tmpstr += "Н1Д4П3 "; if (CB_N_1_D_4_P_4.Checked) tmpstr += "Н1Д4П4 ";
                        if (CB_N_1_D_4_P_5.Checked) tmpstr += "Н1Д4П5 "; if (CB_N_1_D_4_P_6.Checked) tmpstr += "Н1Д4П6 "; if (CB_N_1_D_4_P_7.Checked) tmpstr += "Н1Д4П7 ";

                        if (CB_N_1_D_5_P_1.Checked) tmpstr += "Н1Д5П1 "; if (CB_N_1_D_5_P_2.Checked) tmpstr += "Н1Д5П2 "; if (CB_N_1_D_5_P_3.Checked) tmpstr += "Н1Д5П3 "; if (CB_N_1_D_5_P_4.Checked) tmpstr += "Н1Д5П4 ";
                        if (CB_N_1_D_5_P_5.Checked) tmpstr += "Н1Д5П5 "; if (CB_N_1_D_5_P_6.Checked) tmpstr += "Н1Д5П6 "; if (CB_N_1_D_5_P_7.Checked) tmpstr += "Н1Д5П7 ";

                        if (CB_N_1_D_6_P_1.Checked) tmpstr += "Н1Д6П1 "; if (CB_N_1_D_6_P_2.Checked) tmpstr += "Н1Д6П2 "; if (CB_N_1_D_6_P_3.Checked) tmpstr += "Н1Д6П3 "; if (CB_N_1_D_6_P_4.Checked) tmpstr += "Н1Д6П4 ";
                        if (CB_N_1_D_6_P_5.Checked) tmpstr += "Н1Д6П5 "; if (CB_N_1_D_6_P_6.Checked) tmpstr += "Н1Д6П6 "; if (CB_N_1_D_6_P_7.Checked) tmpstr += "Н1Д6П7 ";
                        ///

                        ///2 неделя
                        if (CB_N_2_D_1_P_1.Checked) tmpstr += "Н2Д1П1 "; if (CB_N_2_D_1_P_2.Checked) tmpstr += "Н2Д1П2 "; if (CB_N_2_D_1_P_3.Checked) tmpstr += "Н2Д1П3 "; if (CB_N_2_D_1_P_4.Checked) tmpstr += "Н2Д1П4 ";
                        if (CB_N_2_D_1_P_5.Checked) tmpstr += "Н2Д1П5 "; if (CB_N_2_D_1_P_6.Checked) tmpstr += "Н2Д1П6 "; if (CB_N_2_D_1_P_7.Checked) tmpstr += "Н2Д1П7 ";

                        if (CB_N_2_D_2_P_1.Checked) tmpstr += "Н2Д2П1 "; if (CB_N_2_D_2_P_2.Checked) tmpstr += "Н2Д2П2 "; if (CB_N_2_D_2_P_3.Checked) tmpstr += "Н2Д2П3 "; if (CB_N_2_D_2_P_4.Checked) tmpstr += "Н2Д2П4 ";
                        if (CB_N_2_D_2_P_5.Checked) tmpstr += "Н2Д2П5 "; if (CB_N_2_D_2_P_6.Checked) tmpstr += "Н2Д2П6 "; if (CB_N_2_D_2_P_7.Checked) tmpstr += "Н2Д2П7 ";

                        if (CB_N_2_D_3_P_1.Checked) tmpstr += "Н2Д3П1 "; if (CB_N_2_D_3_P_2.Checked) tmpstr += "Н2Д3П2 "; if (CB_N_2_D_3_P_3.Checked) tmpstr += "Н2Д3П3 "; if (CB_N_2_D_3_P_4.Checked) tmpstr += "Н2Д3П4 ";
                        if (CB_N_2_D_3_P_5.Checked) tmpstr += "Н2Д3П5 "; if (CB_N_2_D_3_P_6.Checked) tmpstr += "Н2Д3П6 "; if (CB_N_2_D_3_P_7.Checked) tmpstr += "Н2Д3П7 ";

                        if (CB_N_2_D_4_P_1.Checked) tmpstr += "Н2Д4П1 "; if (CB_N_2_D_4_P_2.Checked) tmpstr += "Н2Д4П2 "; if (CB_N_2_D_4_P_3.Checked) tmpstr += "Н2Д4П3 "; if (CB_N_2_D_4_P_4.Checked) tmpstr += "Н2Д4П4 ";
                        if (CB_N_2_D_4_P_5.Checked) tmpstr += "Н2Д4П5 "; if (CB_N_2_D_4_P_6.Checked) tmpstr += "Н2Д4П6 "; if (CB_N_2_D_4_P_7.Checked) tmpstr += "Н2Д4П7 ";

                        if (CB_N_2_D_5_P_1.Checked) tmpstr += "Н2Д5П1 "; if (CB_N_2_D_5_P_2.Checked) tmpstr += "Н2Д5П2 "; if (CB_N_2_D_5_P_3.Checked) tmpstr += "Н2Д5П3 "; if (CB_N_2_D_5_P_4.Checked) tmpstr += "Н2Д5П4 ";
                        if (CB_N_2_D_5_P_5.Checked) tmpstr += "Н2Д5П5 "; if (CB_N_2_D_5_P_6.Checked) tmpstr += "Н2Д5П6 "; if (CB_N_2_D_5_P_7.Checked) tmpstr += "Н2Д5П7 ";

                        if (CB_N_2_D_6_P_1.Checked) tmpstr += "Н2Д6П1 "; if (CB_N_2_D_6_P_2.Checked) tmpstr += "Н2Д6П2 "; if (CB_N_2_D_6_P_3.Checked) tmpstr += "Н2Д6П3 "; if (CB_N_2_D_6_P_4.Checked) tmpstr += "Н2Д6П4 ";
                        if (CB_N_2_D_6_P_5.Checked) tmpstr += "Н2Д6П5 "; if (CB_N_2_D_6_P_6.Checked) tmpstr += "Н2Д6П6 "; if (CB_N_2_D_6_P_7.Checked) tmpstr += "Н2Д6П7 ";
                        ///
                    }

                    using (var command = new NpgsqlCommand("INSERT INTO public.requests (id_disp, time_of_para, id_auditor) VALUES (@iddi, @tmpar, @idaud)", conn))
                    {
                        command.Parameters.AddWithValue("iddi", tmpID);                      
                        command.Parameters.AddWithValue("tmpar", tmpstr);
                        command.Parameters.AddWithValue("idaud", Environment.MachineName.ToString());
                        command.ExecuteNonQuery();
                    }///добавляем запрос на анализ

                    //Закрытие
                    conn.Close();
                    MessageBox.Show("Запрос удачно отправлен");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //схожесть с исходным
        private void button3_Click(object sender, EventArgs e)
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
                    var SELcommand = new NpgsqlCommand("SELECT output_usefull.percentage, output_usefull.id_req, cs_rawdata.url" +
                        " FROM public.output_usefull INNER JOIN public.cs_rawdata" +
                        " ON output_usefull.id_cs=cs_rawdata.id", conn);
                    NpgsqlDataReader readerSel = SELcommand.ExecuteReader();

                    string answer = "";
                    while(readerSel.Read())
                    {
                        answer += readerSel[0].ToString() + " " + readerSel[1].ToString() + " " + readerSel[2].ToString();

                    }
                    MessageBox.Show(answer);
     
                    //Закрытие
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        //интересные/рекомендуемые сайта
        private void button4_Click(object sender, EventArgs e)
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
                    conn.Open(); ////// отличие от предидущего в том, что здесь в запросе есть WHRERE
                    var SELcommand = new NpgsqlCommand("SELECT output_usefull.percentage, output_usefull.id_req, cs_rawdata.url " +
                        "FROM public.output_usefull INNER JOIN public.cs_rawdata " +
                        "ON output_usefull.id_cs=cs_rawdata.id " +
                        "WHERE cs_rawdata.visit_count>2 AND output_usefull.percentage>50;", conn);//полпуряные и посещяемые сайты
                    NpgsqlDataReader readerSel = SELcommand.ExecuteReader();

                    string answer = "";
                    while (readerSel.Read())
                    {
                       answer += readerSel[0].ToString() + " " + readerSel[1].ToString() + " " + readerSel[2].ToString();
                    }
                    MessageBox.Show(answer);

                    //Закрытие
                    conn.Close();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            CB_N_1_D_1_P_1.Checked = !CB_N_1_D_1_P_1.Checked;
            CB_N_1_D_1_P_2.Checked = !CB_N_1_D_1_P_2.Checked;
            CB_N_1_D_1_P_3.Checked = !CB_N_1_D_1_P_3.Checked;
            CB_N_1_D_1_P_4.Checked = !CB_N_1_D_1_P_4.Checked;
            CB_N_1_D_1_P_5.Checked = !CB_N_1_D_1_P_5.Checked;
            CB_N_1_D_1_P_6.Checked = !CB_N_1_D_1_P_6.Checked;
            CB_N_1_D_1_P_7.Checked = !CB_N_1_D_1_P_7.Checked;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CB_N_1_D_2_P_1.Checked = !CB_N_1_D_2_P_1.Checked;
            CB_N_1_D_2_P_2.Checked = !CB_N_1_D_2_P_2.Checked;
            CB_N_1_D_2_P_3.Checked = !CB_N_1_D_2_P_3.Checked;
            CB_N_1_D_2_P_4.Checked = !CB_N_1_D_2_P_4.Checked;
            CB_N_1_D_2_P_5.Checked = !CB_N_1_D_2_P_5.Checked;
            CB_N_1_D_2_P_6.Checked = !CB_N_1_D_2_P_6.Checked;
            CB_N_1_D_2_P_7.Checked = !CB_N_1_D_2_P_7.Checked;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CB_N_1_D_3_P_1.Checked = !CB_N_1_D_3_P_1.Checked;
            CB_N_1_D_3_P_2.Checked = !CB_N_1_D_3_P_2.Checked;
            CB_N_1_D_3_P_3.Checked = !CB_N_1_D_3_P_3.Checked;
            CB_N_1_D_3_P_4.Checked = !CB_N_1_D_3_P_4.Checked;
            CB_N_1_D_3_P_5.Checked = !CB_N_1_D_3_P_5.Checked;
            CB_N_1_D_3_P_6.Checked = !CB_N_1_D_3_P_6.Checked;
            CB_N_1_D_3_P_7.Checked = !CB_N_1_D_3_P_7.Checked;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CB_N_1_D_4_P_1.Checked = !CB_N_1_D_4_P_1.Checked;
            CB_N_1_D_4_P_2.Checked = !CB_N_1_D_4_P_2.Checked;
            CB_N_1_D_4_P_3.Checked = !CB_N_1_D_4_P_3.Checked;
            CB_N_1_D_4_P_4.Checked = !CB_N_1_D_4_P_4.Checked;
            CB_N_1_D_4_P_5.Checked = !CB_N_1_D_4_P_5.Checked;
            CB_N_1_D_4_P_6.Checked = !CB_N_1_D_4_P_6.Checked;
            CB_N_1_D_4_P_7.Checked = !CB_N_1_D_4_P_7.Checked;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            CB_N_1_D_5_P_1.Checked = !CB_N_1_D_5_P_1.Checked;
            CB_N_1_D_5_P_2.Checked = !CB_N_1_D_5_P_2.Checked;
            CB_N_1_D_5_P_3.Checked = !CB_N_1_D_5_P_3.Checked;
            CB_N_1_D_5_P_4.Checked = !CB_N_1_D_5_P_4.Checked;
            CB_N_1_D_5_P_5.Checked = !CB_N_1_D_5_P_5.Checked;
            CB_N_1_D_5_P_6.Checked = !CB_N_1_D_5_P_6.Checked;
            CB_N_1_D_5_P_7.Checked = !CB_N_1_D_5_P_7.Checked;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            CB_N_1_D_6_P_1.Checked = !CB_N_1_D_6_P_1.Checked;
            CB_N_1_D_6_P_2.Checked = !CB_N_1_D_6_P_2.Checked;
            CB_N_1_D_6_P_3.Checked = !CB_N_1_D_6_P_3.Checked;
            CB_N_1_D_6_P_4.Checked = !CB_N_1_D_6_P_4.Checked;
            CB_N_1_D_6_P_5.Checked = !CB_N_1_D_6_P_5.Checked;
            CB_N_1_D_6_P_6.Checked = !CB_N_1_D_6_P_6.Checked;
            CB_N_1_D_6_P_7.Checked = !CB_N_1_D_6_P_7.Checked;
        }
        //2 ned
        private void button16_Click(object sender, EventArgs e)
        {
            CB_N_2_D_1_P_1.Checked = !CB_N_2_D_1_P_1.Checked;
            CB_N_2_D_1_P_2.Checked = !CB_N_2_D_1_P_2.Checked;
            CB_N_2_D_1_P_3.Checked = !CB_N_2_D_1_P_3.Checked;
            CB_N_2_D_1_P_4.Checked = !CB_N_2_D_1_P_4.Checked;
            CB_N_2_D_1_P_5.Checked = !CB_N_2_D_1_P_5.Checked;
            CB_N_2_D_1_P_6.Checked = !CB_N_2_D_1_P_6.Checked;
            CB_N_2_D_1_P_7.Checked = !CB_N_2_D_1_P_7.Checked;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            CB_N_2_D_2_P_1.Checked = !CB_N_2_D_2_P_1.Checked;
            CB_N_2_D_2_P_2.Checked = !CB_N_2_D_2_P_2.Checked;
            CB_N_2_D_2_P_3.Checked = !CB_N_2_D_2_P_3.Checked;
            CB_N_2_D_2_P_4.Checked = !CB_N_2_D_2_P_4.Checked;
            CB_N_2_D_2_P_5.Checked = !CB_N_2_D_2_P_5.Checked;
            CB_N_2_D_2_P_6.Checked = !CB_N_2_D_2_P_6.Checked;
            CB_N_2_D_2_P_7.Checked = !CB_N_2_D_2_P_7.Checked;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            CB_N_2_D_3_P_1.Checked = !CB_N_2_D_3_P_1.Checked;
            CB_N_2_D_3_P_2.Checked = !CB_N_2_D_3_P_2.Checked;
            CB_N_2_D_3_P_3.Checked = !CB_N_2_D_3_P_3.Checked;
            CB_N_2_D_3_P_4.Checked = !CB_N_2_D_3_P_4.Checked;
            CB_N_2_D_3_P_5.Checked = !CB_N_2_D_3_P_5.Checked;
            CB_N_2_D_3_P_6.Checked = !CB_N_2_D_3_P_6.Checked;
            CB_N_2_D_3_P_7.Checked = !CB_N_2_D_3_P_7.Checked;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            CB_N_2_D_4_P_1.Checked = !CB_N_2_D_4_P_1.Checked;
            CB_N_2_D_4_P_2.Checked = !CB_N_2_D_4_P_2.Checked;
            CB_N_2_D_4_P_3.Checked = !CB_N_2_D_4_P_3.Checked;
            CB_N_2_D_4_P_4.Checked = !CB_N_2_D_4_P_4.Checked;
            CB_N_2_D_4_P_5.Checked = !CB_N_2_D_4_P_5.Checked;
            CB_N_2_D_4_P_6.Checked = !CB_N_2_D_4_P_6.Checked;
            CB_N_2_D_4_P_7.Checked = !CB_N_2_D_4_P_7.Checked;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            CB_N_2_D_5_P_1.Checked = !CB_N_2_D_5_P_1.Checked;
            CB_N_2_D_5_P_2.Checked = !CB_N_2_D_5_P_2.Checked;
            CB_N_2_D_5_P_3.Checked = !CB_N_2_D_5_P_3.Checked;
            CB_N_2_D_5_P_4.Checked = !CB_N_2_D_5_P_4.Checked;
            CB_N_2_D_5_P_5.Checked = !CB_N_2_D_5_P_5.Checked;
            CB_N_2_D_5_P_6.Checked = !CB_N_2_D_5_P_6.Checked;
            CB_N_2_D_5_P_7.Checked = !CB_N_2_D_5_P_7.Checked;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            CB_N_2_D_6_P_1.Checked = !CB_N_2_D_6_P_1.Checked;
            CB_N_2_D_6_P_2.Checked = !CB_N_2_D_6_P_2.Checked;
            CB_N_2_D_6_P_3.Checked = !CB_N_2_D_6_P_3.Checked;
            CB_N_2_D_6_P_4.Checked = !CB_N_2_D_6_P_4.Checked;
            CB_N_2_D_6_P_5.Checked = !CB_N_2_D_6_P_5.Checked;
            CB_N_2_D_6_P_6.Checked = !CB_N_2_D_6_P_6.Checked;
            CB_N_2_D_6_P_7.Checked = !CB_N_2_D_6_P_7.Checked;
        }
    }
}
