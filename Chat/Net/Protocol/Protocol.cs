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
    public struct NOTICE_SESSION
    {
        public UInt32 _sessionID;
        public UInt16 _joinedUserCount;
        public UInt16 _maxUserCount;
        public string _sessionName;

        public static NOTICE_SESSION GetMessage(byte[] buffer)
        {
            return Parse(buffer.AsSpan(), out _);
        }
        
        public static NOTICE_SESSION Parse(ReadOnlySpan<byte> payload, out int bytesRead)
        {
            bytesRead = 0;

            if (payload.Length < 10)
                throw new ArgumentException("NOTICE_SESSION  payload too short.");

            uint sessionId = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(0, 4));
            ushort joined = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(4, 2));
            ushort max = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(6, 2));
            ushort nameLen = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(8, 2));

            int headerSize = 10;
            if (payload.Length < headerSize + nameLen)
                throw new ArgumentException("NOTICE_SESSION  payload missing sessionName bytes.");

            string name = Encoding.UTF8.GetString(payload.Slice(headerSize, nameLen));

            bytesRead = headerSize + nameLen;
            
            return new NOTICE_SESSION
            {
                _sessionID = sessionId,
                _joinedUserCount = joined,
                _maxUserCount = max,
                _sessionName = name
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_CREATE_SESSION
    {
        public UInt16 _max_user_count;

        public UInt16 _session_name_length;
        public string _session_name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RESPONSE_CREATE_SESSION
    {
        public UInt32 _session_id;
        public UInt16 _feedback;

        public static RESPONSE_CREATE_SESSION GetMessage(byte[] buffer)
        {
            return Parse(buffer.AsSpan(), out _);
        }

        public static RESPONSE_CREATE_SESSION Parse(ReadOnlySpan<byte> payload, out int bytesRead)
        {
            bytesRead = 0;

            if (payload.Length < 10)
                throw new ArgumentException("RESPONSE_CREATE_SESSION  payload too short.");

            uint sessionId = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(0, 4));
            ushort feedback = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(4, 2));

            bytesRead = 6;

            return new RESPONSE_CREATE_SESSION
            {
                _session_id = sessionId,
                _feedback = feedback
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_JOIN_SESSION
    {
        public UInt32 _session_id;

        public UInt16 _joined_user_nickname_length;
        public string _joined_user_nickname;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RESPONSE_JOIN_SESSION
    {
        public UInt16 _feedback;
        public UInt32 _session_id;

        public static RESPONSE_JOIN_SESSION GetMessage(byte[] buffer)
        {
            return Parse(buffer.AsSpan(), out _);
        }

        public static RESPONSE_JOIN_SESSION Parse(ReadOnlySpan<byte> payload, out int bytesRead)
        {
            bytesRead = 0;

            if (payload.Length < 10)
                throw new ArgumentException("RESPONSE_JOIN_SESSION  payload too short.");

            ushort feedback = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(0, 2));
            uint sessionId = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(2, 4));

            bytesRead = 6;

            return new RESPONSE_JOIN_SESSION
            {
                _feedback = feedback,
                _session_id = sessionId
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NOTICE_JOIN_SESSION
    {
        public string _joined_user_nickname;

        public static NOTICE_JOIN_SESSION GetMessage(byte[] buffer)
        {
            return Parse(buffer.AsSpan(), out _);
        }

        public static NOTICE_JOIN_SESSION Parse(ReadOnlySpan<byte> payload, out int bytesRead)
        {
            bytesRead = 0;

            if (payload.Length < 0)
                throw new ArgumentException("NOTICE_JOIN_SESSION payload too short.");

            ushort nameLen = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(0, 2));

            int headerSize = 2;
            if (payload.Length < headerSize + nameLen)
                throw new ArgumentException("NOTICE_SESSION payload missing sessionName bytes.");

            string sessionName = Encoding.UTF8.GetString(payload.Slice(headerSize, nameLen));

            bytesRead = headerSize + nameLen;

            return new NOTICE_JOIN_SESSION
            {
                _joined_user_nickname = sessionName
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_CHAT_MESSAGE
    {
        public UInt32 _session_id;

        public UInt16 _chat_message_length;
        public string _chat_message;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NOTICE_CHAT_MESSAGE
    {
        public string _joined_user_nickname;
        public string _chat_message;

        public static NOTICE_CHAT_MESSAGE GetMessage(byte[] buffer)
        {
            return Parse(buffer.AsSpan(), out _);
        }

        public static NOTICE_CHAT_MESSAGE Parse(ReadOnlySpan<byte> payload, out int bytesRead)
        {
            bytesRead = 0;

            if (payload.Length < 0)
                throw new ArgumentException("NOTICE_CHAT_MESSAGE payload too short.");

            ushort nameLen = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(0, 2));

            int headerSize = 2;
            if (payload.Length < headerSize + nameLen)
                throw new ArgumentException("NOTICE_CHAT_MESSAGE payload missing sessionName bytes.");

            string sessionName = Encoding.UTF8.GetString(payload.Slice(headerSize, nameLen));

            ushort chatLen = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(nameLen + 2, 2));

            if (payload.Length < headerSize + nameLen + chatLen)
                throw new ArgumentException("NOTICE_CHAT_MESSAGE payload missing chat message bytes.");

            string chatMessage = Encoding.UTF8.GetString(payload.Slice(headerSize + nameLen + 2, chatLen));

            bytesRead = headerSize + nameLen + chatLen;

            return new NOTICE_CHAT_MESSAGE
            {
                _joined_user_nickname = sessionName,
                _chat_message = chatMessage
            };
        }
    }

    public class Protocol
    {
        public enum MSG : int
        {
            MSG_NONE = 0,
            MSG_REQUEST_SESSION_LIST = 1,
            MSG_NOTICE_SESSION = 2,
            MSG_RESPONSE_SESSION_LIST = 3,

            MSG_REQUEST_CREATE_SESSION = 4,
            MSG_RESPONSE_CREATE_SESSION = 5,

            MSG_REQUEST_JOIN_SESSION = 6,
            MSG_RESPONSE_JOIN_SESSION = 7,
            MSG_NOTICE_JOIN_SESSION = 8,

            MSG_REQUEST_LEAVE_SESSION = 9,
            MSG_RESPONSE_LEAVE_SESSION = 10,
            MSG_NOTICE_LEAVE_SESSION = 11,

            MSG_REQUEST_CHAT_MESSAGE = 12,
            MSG_NOTICE_CHAT_MESSAGE = 13,
        }
    }
}
