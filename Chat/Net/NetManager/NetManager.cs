using Chat.Net.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.NetManager
{
    public static class NetManager
    {
        #region Variable

        private static TCPSimpleClient _tcpClient;
        private static Queue<Tuple<Protocol.Protocol.MSG, byte[]>> _packet_queue;

        private static Dictionary<Protocol.Protocol.MSG, List<Action<byte[]>>> _packet_handler;

        public static RequestModule RequestModule;

        #endregion

        static NetManager()
        {
            RequestModule = new RequestModule();

            _tcpClient = new TCPSimpleClient(System.Net.IPAddress.Parse("127.0.0.1"), 3550);
            _packet_queue = new Queue<Tuple<Protocol.Protocol.MSG, byte[]>>();

            _packet_handler = new Dictionary<Protocol.Protocol.MSG, List<Action<byte[]>>>();
        }

        public static void Connect()
        {
            _tcpClient.ConnectToAsync();
        }

        public static void AddHandler(Protocol.Protocol.MSG msgType, Action<byte[]> action)
        {
            if (!_packet_handler.TryGetValue(msgType, out var handlers))
            {
                List<Action<byte[]>> newHandlerList = new List<Action<byte[]>>();
                newHandlerList.Add(action);

                _packet_handler.Add(msgType, newHandlerList);
            }
            else
                handlers.Add(action);
        }

        public static void AddMSG(Tuple<Protocol.Protocol.MSG, byte[]> queueData)
        {
            _packet_queue.Enqueue(queueData);
        }

        public static void Dispatch()
        {
            for (int i = 0; i < _packet_queue.Count; i++)
            {
                var data = _packet_queue.Dequeue();

                if (_packet_handler.TryGetValue(data.Item1, out var handlers))
                {
                    foreach(var handler in handlers)
                        handler.Invoke(data.Item2);
                }
            }
        }

        public static void Send(byte[] buffer)
        {
            _tcpClient.Send(buffer);
        }
    }
}
