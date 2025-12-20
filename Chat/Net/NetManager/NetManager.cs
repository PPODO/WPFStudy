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
        private static Queue<BasicProtocol> _packet_queue;

        private static Dictionary<Protocol.Protocol.MSG, List<Action<BasicProtocol>>> _packet_handler;

        #endregion

        static NetManager()
        {
            _tcpClient = new TCPSimpleClient(System.Net.IPAddress.Parse("127.0.0.1"), 3550);
            _packet_queue = new Queue<BasicProtocol>();

            _packet_handler = new Dictionary<Protocol.Protocol.MSG, List<Action<BasicProtocol>>>();
        }

        public static void Connect()
        {
            _tcpClient.ConnectToAsync();
        }

        public static void AddHandler(Protocol.Protocol.MSG msgType, Action<BasicProtocol> action)
        {
            if (!_packet_handler.TryGetValue(msgType, out var handlers))
            {
                List<Action<BasicProtocol>> newHandlerList = new List<Action<BasicProtocol>>();
                newHandlerList.Add(action);

                _packet_handler.Add(msgType, newHandlerList);
            }
            else
                handlers.Add(action);
        }

        public static void AddMSG(BasicProtocol basicProtocol)
        {
            _packet_queue.Enqueue(basicProtocol);
        }

        public static void Dispatch()
        {
            for (int i = 0; i < _packet_queue.Count; i++)
            {
                var packet = _packet_queue.Dequeue();

                if (_packet_handler.TryGetValue(packet._messageType, out var handlers))
                {
                    foreach(var handler in handlers)
                        handler.Invoke(packet);
                }
            }
        }
    }
}
