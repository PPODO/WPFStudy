using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Chat.Net.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BasicHeader
    {
        public Chat.Net.Protocol.Protocol.MSG _messageType;
        public int _totalPacketSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_SESSION_LIST
    {
        public byte _dummy;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NOTICE_SESSION_LIST
    {
        public UInt32 _sessionID;
        public UInt16 _joinedUserCount;
        public UInt16 _maxUserCount;
        public string _sessionName;
        public static NOTICE_SESSION_LIST GetMessage(byte[] buffer)
        {
            return Parse(buffer.AsSpan(), out _);
        }
        
        public static NOTICE_SESSION_LIST Parse(ReadOnlySpan<byte> payload, out int bytesRead)
        {
            bytesRead = 0;

            if (payload.Length < 10)
                throw new ArgumentException("NOTICE_SESSION_LIST payload too short.");

            uint sessionId = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(0, 4));
            ushort joined = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(4, 2));
            ushort max = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(6, 2));
            ushort nameLen = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(8, 2));

            int headerSize = 10;
            if (payload.Length < headerSize + nameLen)
                throw new ArgumentException("NOTICE_SESSION_LIST payload missing sessionName bytes.");

            string name = Encoding.UTF8.GetString(payload.Slice(headerSize, nameLen));

            bytesRead = headerSize + nameLen;
            
            return new NOTICE_SESSION_LIST
            {
                _sessionID = sessionId,
                _joinedUserCount = joined,
                _maxUserCount = max,
                _sessionName = name
            };
        }
    }

    public class Protocol
    {
        public enum MSG : int
        {
            MSG_NONE = 0,
            MSG_REQUEST_SESSION_LIST = 1,
            MSG_NOTICE_SESSION_LIST = 2,
            MSG_RESPONSE_SESSION_LIST = 3,
        }
    }
}
