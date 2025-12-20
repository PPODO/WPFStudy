using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Net.Protocol
{
    #region Protocol

    public class BasicProtocol
    {
        public Protocol.MSG _messageType;
        public int _totalPacketSize;

        public BasicProtocol(Protocol.MSG messageType = 0, int totalPacketSize = 0)
        {
            _messageType = messageType;
            _totalPacketSize = totalPacketSize;
        }
    }

    public class NOTICE_SESSION_LIST : BasicProtocol
    {


    }

    #endregion

    public class Protocol
    {
        public enum MSG
        {
            MSG_NONE = 0,
            MSG_REQUEST_SESSION_LIST = 1,
            MSG_NOTICE_SESSION_LIST = 2,
            MSG_RESPONSE_SESSION_LIST = 3,


        }

        public static BasicProtocol? GetMarshalProtocolByMSG(MSG type, IntPtr packetPtr)
        {
            switch (type)
            {
                case MSG.MSG_NOTICE_SESSION_LIST:
                    return Marshal.PtrToStructure<NOTICE_SESSION_LIST>(packetPtr);
                default:
                    return null;
            }
        }
    }
}
