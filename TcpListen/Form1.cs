using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace TcpListen
{
    
    public partial class Form1 : Form
    {
        private const int LOCAL_PORT = 13;
        TcpListener listener = null;

        public Form1()
        {
            InitializeComponent();
            listener=new TcpListener(IPAddress.Any,LOCAL_PORT);
            listener.Start();
            txtRecMsgs.AppendText("正在监听:" + IPAddress.Any + ":" + LOCAL_PORT.ToString()+"\n");
            listener.BeginAcceptTcpClient(new AsyncCallback(acceptCallback), listener);
        }

        private void acceptCallback(IAsyncResult ar)
        {
            TcpListener lstn = (TcpListener)ar.AsyncState;
            TcpClient client = lstn.EndAcceptTcpClient(ar);
            
            Task.Run(() =>
            {
                string host = client.Client.RemoteEndPoint.ToString();
                NetworkStream stream = client.GetStream();
                while (true)
                {
                    byte[] buffer = new byte[4];
                    stream.Read(buffer, 0, 4);
                    int len = BitConverter.ToInt32(buffer, 0);

                    buffer = new byte[len];
                    stream.Read(buffer, 0, len);
                    string recMsg = Encoding.UTF8.GetString(buffer);
                    if (recMsg == "$END$")
                    {
                        string message = "客户端" + host + "发送了退出指令\n";
                        txtRecMsgs.Invoke(new Action(() => txtRecMsgs.AppendText(message)));
                        break;
                    }

                    else
                    {
                        txtRecMsgs.Invoke((Action)delegate ()
                        {
                            string message = string.Format("来自{0}的消息：{1}\n", host, recMsg);
                            txtRecMsgs.AppendText(message);
                        });
                    }
                }
                client.Close();
            });
            lstn.BeginAcceptTcpClient(new AsyncCallback(acceptCallback), lstn);
        }
    }
}
