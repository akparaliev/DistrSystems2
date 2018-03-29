using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;


namespace DistrSystems2
{
    [Serializable]
    public class Client
    {
  

        private bool _stop;
        public void Run()
        {
            Console.WriteLine("type host");
            string host = Console.ReadLine();
            
            TcpChannel m_TcpChan = new TcpChannel(0);
            ChannelServices.RegisterChannel(m_TcpChan, false);

            // Create the object for calling into the server
            ICallsToServer m_RemoteObject = (ICallsToServer)
                Activator.GetObject(typeof(ICallsToServer),
                $"tcp://{host}:123/RemoteServer");
            // Define sink for events
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(NotifySink),
                "ServerEvents",
                WellKnownObjectMode.Singleton);
            NotifySink sink = new NotifySink();

            // Assign the callback from the server to here
            var (id, name, timespan, accountId) = GetPrivateVariablesValues();
            m_RemoteObject.Notify += new NotifyCallback(sink.FireNotifyCallback);

           




            m_RemoteObject.CreateNewAccount(id, name, accountId);
  


            Console.WriteLine(" type 'w 1' for withdraw 1 amount to account");
            Console.WriteLine(" type 'd 1' for deposit 1 amount to account");

            while (!_stop)
            {
                var result = Console.ReadLine();
                var parametres = result.Split(' ');
                var createdDate = DateTime.Now.Ticks;
                switch (parametres[0])
                {
                    case "w":
                        Task.Delay(timespan).ContinueWith(x => m_RemoteObject.Withdraw(id, accountId, createdDate, Int32.Parse(parametres[1])));          
                        break;
                    case "d":
                        Task.Delay(timespan).ContinueWith(x => m_RemoteObject.Deposit(id, accountId, createdDate, Int32.Parse(parametres[1])));
                        break;

                }
            }

        }

        

        private (string, string, int , string) GetPrivateVariablesValues()
        {
            Console.WriteLine("type timespan in seconds");

            if (!Int32.TryParse(Console.ReadLine(), out int timespan))
            {
                Console.WriteLine("type integer, exit .....");
                Environment.Exit(0);
            }
            Console.WriteLine("type name");
            string name = Console.ReadLine();

            Console.WriteLine("type accountId");
            string accountId = Console.ReadLine();

            string id = Guid.NewGuid().ToString();
            return (id, name, timespan, accountId);
        }
    }
    class NotifySink : NotifyCallbackSink
    {
        /// <summary>
        /// Events from the server call into here. This is not in the GUI thread.
        /// </summary>
        /// <param name="s">Pass a string for testing</param>
        protected override void OnNotifyCallback(string s)
        {
            Console.WriteLine("Message from the server : {0}", s);
        }
    }
}
