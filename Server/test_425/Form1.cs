using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_425
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpListener Server; //伺服端網路監聽器
        Socket Client; //給客戶用的連線物件
        Thread Th_Svr; //伺服器監聽用執行緒
        Thread Th_Clt; //客戶用的通話執行緒
        Hashtable HT = new Hashtable();

        private void ServerSub() 
        {
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
            Server = new TcpListener(EP);
            Server.Start(100);
            while (true)
            {
                Client = Server.AcceptSocket();
                Th_Clt = new Thread(Listen); //建立監聽這個客戶連線的獨立執行緒
                Th_Clt.IsBackground = true; //設定為背景執行緒
                Th_Clt.Start(); //開始執行緒的運作
            }
        }

        private void Listen()
        {
            Socket sck = Client;//複製Client通訊物件到個別客戶專用物件Sck
            Thread Th = Th_Clt;//複製執行緒Th_Clt到區域變數Th
            while (true)  //持續監聽客戶傳來的訊息
            {
                try //用 Sck 來接收此客戶訊息，inLen 是接收訊息的 Byte 數目
                {
                    byte[] B = new byte[1023];    //建立接收資料用的陣列，長度須大於可能的訊息
                    int inLen = sck.Receive(B); //接收網路資訊(Byte陣列)
                    string Msg = Encoding.Default.GetString(B, 0, inLen); //翻譯實際訊息(長度inLen)
                    string Cmd = Msg.Substring(0, 1); //取出命令碼 (第一個字)
                    string Str = Msg.Substring(1);    //取出命令碼之後的訊息
                    switch (Cmd) //依據命令碼執行功能
                    {
                        case "0"://有新使用者上線：新增使用者到名單中
                            HT.Add(Str, sck); //連線加入雜湊表，Key:使用者，Value:連線物件(Socket)
                            listBox1.Items.Add(Str); //加入上線者名單
                            break;
                        case "9":
                            HT.Remove(Str); //移除使用者名稱為Name的連線物件
                            listBox1.Items.Remove(Str); //自上線者名單移除Name
                            Th.Abort(); //結束此客戶的監聽執行緒
                            break;
                    }
                }
                catch (Exception)
                {
                    //有錯誤時忽略，通常是客戶端無預警強制關閉程式，測試階段常發生
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Th_Svr = new Thread(ServerSub); //宣告監聽執行緒(ServerSub)
            Th_Svr.IsBackground = true; //設定為背景執行緒
            Th_Svr.Start(); //啟動監聽執行緒
            button1.Enabled = false; //讓按鍵無法使用(不能重複啟動伺服器)
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread(); //關閉所有執行緒 
        }
    }
}
