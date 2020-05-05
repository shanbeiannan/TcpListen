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
using System.Threading;

namespace Tcpclient
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread sendMsg = new Thread(SendMessage);
            sendMsg.IsBackground = true;
            sendMsg.Start();

            if (textBox3.Text.Equals("$END$"))
            {
                button1.Enabled = true;
                txtIP.Enabled = true;
                txtPort.Enabled = true;
                textBox3.Enabled = false;
                button3.Enabled = false;
                label3.Text = "已关闭连接！\n";

            }
        }

        TcpClient client = null;

        private void button1_Click(object sender, EventArgs e)
        {
            client = new TcpClient(txtIP.Text.Trim(), Convert.ToInt32(txtPort.Text));
            string msg = string.Format("已连接入服务器:{0},端口号:{1}\n", txtIP.Text, txtPort.Text);
            label3.Text = msg;
            button1.Enabled = false;
            txtIP.Enabled = false;
            txtPort.Enabled = false;
            textBox3.Enabled = true;
            button3.Enabled = true;
        }

        private void SendMessage()
        {
            try
            {
                string msg = textBox3.Text;
                if (client == null) return;
                if (client.Connected == false) return;

                byte[] data = Encoding.UTF8.GetBytes(msg);

                int len = data.Length;
                byte[] buffer = BitConverter.GetBytes(len);

                client.GetStream().Write(buffer, 0, 4);
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                //byte[] data = Encoding.UTF8.GetBytes(ex.Message);

                //int len = data.Length;
                //byte[] buffer = BitConverter.GetBytes(len);

                //client.GetStream().Write(buffer, 0, 4);
                //client.GetStream().Write(data, 0, data.Length);
                throw;
            }
        }
    }
}
