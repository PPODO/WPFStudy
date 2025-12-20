using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Chat.Net
{
    public class TCPSimpleClient
    {
        #region Variable

        private int _servicePort;
        private IPAddress _ipAddress;

        private TcpClient _tcpClient;

        private bool _connected;

        private byte[] _receiveBuffer;

        #endregion

        #region AsyncCallback

        void OnCompleteConnect(IAsyncResult result)
        {
            var tcpClient = (TcpClient?)result.AsyncState;

            if (tcpClient == null)
            {
                MessageBox.Show("CONNECT FAILED - TCP CLIENT IS NULL");
                return;
            }

            tcpClient.EndConnect(result);
            tcpClient.GetStream().BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, OnCompleteRead, tcpClient);

            _connected = true;
        }

        void OnCompleteRead(IAsyncResult result)
        {
            var tcpClient = (TcpClient?)result.AsyncState;

            if (tcpClient == null)
            {
                MessageBox.Show("READ FAILED - TCP CLIENT IS NULL");
                return;
            }

           int readBytes = tcpClient.GetStream().EndRead(result);
            if (readBytes == 0)
            {
                MessageBox.Show("READ FAILED - CONNECTION BROKEN!");
                return;
            }

            /*
            strReceived = Encoding.ASCII.GetString(_receiveBuffer, 0, readBytes);
            */

            tcpClient.GetStream().BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, OnCompleteRead, tcpClient);
        }

        #endregion

        public TCPSimpleClient(IPAddress ipAddress, UInt16 servicePort, int bufferSize = 2048)
        {
            _tcpClient = new TcpClient();

            _receiveBuffer = new byte[bufferSize];

            _ipAddress = ipAddress;
            _servicePort = servicePort;
            _connected = false;
        }

        public void ConnectToAsync()
        {
            if (_connected) return;

            _tcpClient.BeginConnect(_ipAddress, _servicePort, OnCompleteConnect, _tcpClient);
        }
    }
}
