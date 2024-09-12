using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Chat_Listener_Client_WPF
{
    class Client_connect
    {
        private TcpClient client;

        public TcpClient Client
        {
            get { return client; }
            set { client = value; }
        }

        private IPEndPoint ipEndPoint;

        public IPEndPoint IpEndPoint
        {
            get { return ipEndPoint; }
            set { ipEndPoint = value; }
        }

        //private NetworkStream stream;

        //public NetworkStream Stream_
        //{
        //    get { return stream; }
        //    set { stream = value; }
        //}

        private byte[] buffer;

        public byte[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        private int buffer_length;

        public int Buffer_length
        {
            get { return buffer_length; }
            set { buffer_length = value; }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        // подсоединяемся к серверу
        public async void Connect()
        {
            await client.ConnectAsync(ipEndPoint);
        }
    }
}
