using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Socket T;
        string User;

        private void button1_Click(object sender, EventArgs e)
        {
            string IP = textBox1.Text;  //伺服器IP
            int Port = int.Parse(textBox2.Text);  //伺服器Port
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port);  //伺服器的連線端點資訊
            //建立可以雙向通訊的TCP連線
            T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            User = textBox3.Text;  //使用者名稱

            try
            {
                T.Connect(EP);  //連上伺服器的端點EP(類似撥號給電話總機)
                Send("0" + User);  //連線後隨即傳送自己的名稱給伺服器
            }
            catch
            {
                MessageBox.Show("無法連上伺服器！"); //連線失敗時顯示訊息
                return;
            }
        }

        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);  //翻譯字串Str為Byte陣列B
            int ret = T.Send(B, 0, B.Length, SocketFlags.None);  //使用連線物件傳送資料
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (button1.Enabled == false)
            {
                Send("9" + User); //傳送自己的離線訊息給伺服器
                T.Close();        //關閉網路通訊器T
            }
        }
    }
}
