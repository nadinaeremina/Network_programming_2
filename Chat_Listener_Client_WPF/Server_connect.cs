using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Listener_Client_WPF
{
    internal class Server_connect
    {
        private TcpListener listener;

        public TcpListener Listener
        {
            get { return listener; }
            set { listener = value; }
        }

        private IPEndPoint ipEndPoint;

        public IPEndPoint IpEndPoint
        {
            get { return ipEndPoint; }
            set { ipEndPoint = value; }
        }

        private NetworkStream stream;

        public NetworkStream Stream
        {
            get { return stream; }
            set { stream = value; }
        }

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

        // запускаем 'listener', который будет слушать входящие соед-ия
        public void Start()
        {
            listener.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
