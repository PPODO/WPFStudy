using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


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



    }
}
