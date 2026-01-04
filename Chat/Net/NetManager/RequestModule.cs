using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Markup;


namespace Chat.Net.NetManager
{
    public class RequestModule
    {
        public void REQUEST_SESSION_LIST()
        {
            const int headerSize = 8;
            int payloadSize = Marshal.SizeOf<Chat.Net.Protocol.REQUEST_SESSION_LIST>();
            int totalSize = headerSize + payloadSize;

            byte[] buffer = new byte[totalSize];

            BitConverter.TryWriteBytes(buffer.AsSpan(0, 4), (int)Chat.Net.Protocol.Protocol.MSG.MSG_REQUEST_SESSION_LIST);
            BitConverter.TryWriteBytes(buffer.AsSpan(4, 4), totalSize);

            buffer[headerSize] = 0;

            NetManager.Send(buffer);
        }
        
        public void REQUEST_CREATE_SESSION(string sessionName, UInt16 maxUserCount)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(sessionName);

            const int headerSize = 8;
            int payloadSize = Marshal.SizeOf<Chat.Net.Protocol.REQUEST_CREATE_SESSION>();
            int totalSize = headerSize + payloadSize + sessionName.Length;

            byte[] buffer = new byte[totalSize];

            BitConverter.TryWriteBytes(buffer.AsSpan(0, 4), (int)Chat.Net.Protocol.Protocol.MSG.MSG_REQUEST_CREATE_SESSION);
            BitConverter.TryWriteBytes(buffer.AsSpan(4, 4), totalSize);

            BitConverter.TryWriteBytes(buffer.AsSpan(8, 2), maxUserCount);
            BitConverter.TryWriteBytes(buffer.AsSpan(10, 2), (ushort)sessionName.Length);

            nameBytes.CopyTo(buffer.AsSpan(12));

            NetManager.Send(buffer);
        }

        public void REQUEST_JOIN_SESSION(UInt32 sessionID, string joinedUserNickname)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(joinedUserNickname);

            const int headerSize = 8;
            int payloadSize = Marshal.SizeOf<Chat.Net.Protocol.REQUEST_JOIN_SESSION>();
            int totalSize = headerSize + payloadSize + joinedUserNickname.Length;

            byte[] buffer = new byte[totalSize];

            BitConverter.TryWriteBytes(buffer.AsSpan(0, 4), (int)Chat.Net.Protocol.Protocol.MSG.MSG_REQUEST_JOIN_SESSION);
            BitConverter.TryWriteBytes(buffer.AsSpan(4, 4), totalSize);

            BitConverter.TryWriteBytes(buffer.AsSpan(8, 4), sessionID);
            BitConverter.TryWriteBytes(buffer.AsSpan(12, 2), (ushort)joinedUserNickname.Length);

            nameBytes.CopyTo(buffer.AsSpan(14));

            NetManager.Send(buffer);
        }

        public void REQUEST_CHAT_MESSAGE(UInt32 sessionID, string chatMessage)
        {
            byte[] chatBytes = Encoding.UTF8.GetBytes(chatMessage);

            const int headerSize = 8;
            int payloadSize = Marshal.SizeOf<Chat.Net.Protocol.REQUEST_CHAT_MESSAGE>();
            int totalSize = headerSize + payloadSize + chatMessage.Length;

            byte[] buffer = new byte[totalSize];

            BitConverter.TryWriteBytes(buffer.AsSpan(0, 4), (int)Chat.Net.Protocol.Protocol.MSG.MSG_REQUEST_CHAT_MESSAGE);
            BitConverter.TryWriteBytes(buffer.AsSpan(4, 4), totalSize);

            BitConverter.TryWriteBytes(buffer.AsSpan(8, 4), sessionID);
            BitConverter.TryWriteBytes(buffer.AsSpan(12, 2), (ushort)chatMessage.Length);

            chatBytes.CopyTo(buffer.AsSpan(14));

            NetManager.Send(buffer);
        }
    }
}
