using Chat.Net.Protocol;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Windows;
using static Chat.Net.Protocol.Protocol;

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
        private int _remainBufferBytes;

        #endregion

        #region AsyncCallback

        private void OnCompleteConnect(IAsyncResult result)
        {
            var tcpClient = (TcpClient?)result.AsyncState;

            if (tcpClient == null)
            {
                MessageBox.Show("CONNECT FAILED - TCP CLIENT IS NULL");
                return;
            }

            if (!tcpClient.Connected)
            {
                MessageBox.Show("CONNECT FAILED!");
                return;
            }

            tcpClient.EndConnect(result);
            tcpClient.GetStream().BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, OnCompleteRead, tcpClient);

            _connected = true;
        }

        private void OnCompleteRead(IAsyncResult result)
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

            _remainBufferBytes += readBytes;

            while (_remainBufferBytes > 0)
            {
                var outPacketBuffer = PacketProcess(_receiveBuffer, _remainBufferBytes, out var msgType, out var totalPacketSize);
                if (outPacketBuffer == null) break;

                _remainBufferBytes -= totalPacketSize;

                NetManager.NetManager.AddMSG(new Tuple<MSG, byte[]>(msgType, outPacketBuffer));
            }

            NetManager.NetManager.Dispatch();

            tcpClient.GetStream().BeginRead(_receiveBuffer, _remainBufferBytes, _receiveBuffer.Length, OnCompleteRead, tcpClient);
        }

        private void OnCompleteWrite(IAsyncResult result)
        {
            var tcpClient = (TcpClient?)result.AsyncState;

            if (tcpClient == null)
            {
                MessageBox.Show("WRITE FAILED - TCP CLIENT IS NULL");
                return;
            }

            tcpClient.GetStream().EndWrite(result);
        }

        #endregion

        #region PacketProcess

        private unsafe byte[]? PacketProcess(byte[] buffer, int receivedBytes, out Protocol.Protocol.MSG msgType, out int totalPacketSize)
        {
            msgType = MSG.MSG_NONE;
            totalPacketSize = 0;

            const int MaxPayload = 1024;

            int headerSize = Marshal.SizeOf<BasicHeader>();
            if (receivedBytes < headerSize)
                return null;

            BasicHeader header;
            fixed (byte* p = buffer)
            {
                header = Marshal.PtrToStructure<BasicHeader>((IntPtr)p);
            }

            if (header._totalPacketSize < headerSize)
                return null;

            int payloadSize = header._totalPacketSize - headerSize;
            if (payloadSize < 0 || payloadSize > MaxPayload)
                return null;

            if (receivedBytes < header._totalPacketSize)
                return null;

            byte[] returnBuffer = new byte[MaxPayload];

            if (payloadSize > 0)
                Buffer.BlockCopy(buffer, headerSize, returnBuffer, 0, payloadSize);

            msgType = header._messageType;
            totalPacketSize = header._totalPacketSize;

            return returnBuffer;
        }

        #endregion

        public TCPSimpleClient(IPAddress ipAddress, UInt16 servicePort, int bufferSize = 2048)
        {
            _tcpClient = new TcpClient();

            _receiveBuffer = new byte[bufferSize];
            _remainBufferBytes = 0;

            _ipAddress = ipAddress;
            _servicePort = servicePort;
            _connected = false;
        }

        public void ConnectToAsync()
        {
            if (_connected) return;

            _tcpClient.BeginConnect(_ipAddress, _servicePort, OnCompleteConnect, _tcpClient);
        }

        public void Send(byte[] buffer)
        {
            if (!_connected) return;

            _tcpClient.GetStream().BeginWrite(buffer, 0, buffer.Length, OnCompleteWrite, _tcpClient);
        }
    }
}
