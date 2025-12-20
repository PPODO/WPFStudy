using Chat.Net.Protocol;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Policy;
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
                var outPacket = PacketProcess(_receiveBuffer, _remainBufferBytes);
                if (outPacket == null) break;

                _remainBufferBytes -= outPacket._totalPacketSize;

                NetManager.NetManager.AddMSG(outPacket);
            }

            NetManager.NetManager.Dispatch();

            tcpClient.GetStream().BeginRead(_receiveBuffer, _remainBufferBytes, _receiveBuffer.Length, OnCompleteRead, tcpClient);
        }

        #endregion

        #region PacketProcess

        private Protocol.BasicProtocol? PacketProcess(byte[] buffer, int receivedBytes)
        {
            int basicProtocolSize = Marshal.SizeOf<Protocol.BasicProtocol>();

            if (receivedBytes < basicProtocolSize)
                return null;

            IntPtr basicPacketPtr = Marshal.AllocHGlobal(basicProtocolSize);
            
            Marshal.Copy(buffer, 0, basicPacketPtr, basicProtocolSize);

            Protocol.BasicProtocol basicPacket = Marshal.PtrToStructure<Protocol.BasicProtocol>(basicPacketPtr);

            Marshal.FreeHGlobal(basicPacketPtr);

            if (receivedBytes < basicPacket._totalPacketSize)
                return null;

            IntPtr packetPtr = Marshal.AllocHGlobal(basicPacket._totalPacketSize);
            Marshal.Copy(buffer, 0, packetPtr, basicPacket._totalPacketSize);

            Protocol.BasicProtocol? packetObject = Protocol.Protocol.GetMarshalProtocolByMSG(basicPacket._messageType, packetPtr);

            Marshal.FreeHGlobal(packetPtr);

            return packetObject;
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
    }
}
