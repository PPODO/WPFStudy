using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Chat.Util
{
    public static class DispatcherService
    {
        public static void Invoke(Action<byte[]> packetAction, byte[] packet)
        {
            Dispatcher? dispatchObject = Application.Current != null ? Application.Current.Dispatcher : null;

            if (dispatchObject == null)
                packetAction(packet);
            else
                dispatchObject.Invoke(packetAction, packet);
        }
    }
}
