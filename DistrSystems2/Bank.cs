using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace DistrSystems2
{
    public class Bank
    {
       
        public void Run()
        {
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;

            // Creating the IDictionary to set the port on the channel instance.
            IDictionary props = new Hashtable();
            props["port"] = 123;                   // This must match number on client

            // Pass the properties for the port setting and the server provider
            TcpChannel m_tcpChannel = new TcpChannel(props, null, provider);
            ChannelServices.RegisterChannel(m_tcpChannel, false);

            // Create the server object for clients to connect to
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ClientComms),
                "RemoteServer",
                WellKnownObjectMode.Singleton);


            Console.ReadLine();
        }
    }
}
